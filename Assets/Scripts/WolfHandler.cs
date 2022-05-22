using System;
using System.Collections;
using System.Linq;
using Photon.Pun;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(DamageHandler))]
public class WolfHandler : MonoBehaviourPun
{
    private const string _WOLF_PROPERTY = "Wolves";
    
    [SerializeField] private GameObject _wolfModel;
    [SerializeField] private GameObject _sheepModel;
    [SerializeField] private LayerMask _sheepLayer;
    [SerializeField] private float _distanceToKill = 5f;
    [SerializeField] private float _attackCooldown = 5f;
    [SerializeField] private float _effectsTiming = 0.5f;
    [SerializeField] private float _stunTiming = 1f;
    [SerializeField] private float _wolfTimeout = 5f;
    [SerializeField] private float _shiftCooldown = 5f;
    [SerializeField] GameObject _particles;
    [SerializeField] private float _wolfSpeed = 10f;
    [SerializeField] private NamePlate _namePlate;
    [SerializeField] private Animator _wolfAnimator;
    [SerializeField] private Animator _sheepAnimator;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _wolfAttack;


    private PhotonView _photonView;
    private DamageHandler _damageHandler;
    private Player _player;
    private Rigidbody _rigidbody;
    
    private float _sheepSpeed;
    
    private bool _isWolf;
    private bool _onCooldown;
    private bool _isDead;
    private bool _canShift = true;
    private bool _isGrazing;

    private Coroutine _wolfTimeoutCoroutine;
    private Coroutine _grazeCoroutine;

    private void Awake()
    {
        _photonView = PhotonView.Get(this);
        _damageHandler = GetComponent<DamageHandler>();
        _player = GetComponent<Player>();
        _rigidbody = GetComponent<Rigidbody>();
        _sheepSpeed = _player.MoveSpeed;
        _damageHandler.OnDeath += OnDeath;
    }

    private void Start()
    {
        if (!PhotonNetwork.IsConnected) return;
        if (PhotonNetwork.CurrentRoom == null) return;

        var wolves = (int[]) PhotonNetwork.CurrentRoom.CustomProperties[_WOLF_PROPERTY];
        var isWolf = wolves.ToList().Contains(PhotonNetwork.LocalPlayer.ActorNumber);
        
        if(isWolf)
            _namePlate.Init(photonView.Owner.NickName);
    }

    private void OnDestroy()
    {
        _damageHandler.OnDeath -= OnDeath;
    }

    private void OnDeath()
    {
        if (_isDead) return;
        _wolfModel.SetActive(false);
        _sheepModel.SetActive(false);
        _isDead = true;
        gameObject.layer = LayerMask.NameToLayer("WolfDead");
        _player.MoveSpeed = _wolfSpeed;
        
        if (PhotonNetwork.IsMasterClient)
            WinConditions.Instance.SetWolfKill();
    }

    void Update()
    {
        if (!_photonView.IsMine) return;
        
        if (Input.GetKeyDown(KeyCode.Space)) ShapeShift();
        else if (Input.GetMouseButtonDown(0))
        {
            if(_isWolf)
                Attack();
            else
                Graze();
        }
    }
    
    private void ShapeShift()
    {
        if (_isDead) return;
        if (!_canShift) return;

        _isWolf = !_isWolf;
        
        _player.MoveSpeed = _isWolf ? _wolfSpeed : _sheepSpeed;

        if (_wolfTimeoutCoroutine != null)
        {
            StopCoroutine(_wolfTimeoutCoroutine);
            UITimer.Instance.StopWolfTimeout();
            _wolfTimeoutCoroutine = null;
        }

        if (_grazeCoroutine != null)
        {
            StopCoroutine(_grazeCoroutine);
            _isGrazing = false;
            _player.PlayerController.DisableMovement = false;
            _grazeCoroutine = null;
            _sheepAnimator.SetBool("Graze", false);
        }

        if (_isWolf)
        {
            UITimer.Instance.StartWolfTimeout(_wolfTimeout);
            _wolfTimeoutCoroutine = StartCoroutine(WolfTimeout());
        }
        else
        {
            UITimer.Instance.StartTransitionCooldown(_shiftCooldown);
            StartCoroutine(ShiftCooldown());
            FlockHandler.WolvesToAvoid.Remove(transform);
        }
        
        StartCoroutine(ShapeShiftStun());
        _photonView.RPC(nameof(RpcShapeShift), RpcTarget.All, _isWolf);
    }

    private IEnumerator ShapeShiftStun()
    {
        _rigidbody.isKinematic = true;
        yield return new WaitForSeconds(_stunTiming);
        _rigidbody.isKinematic = false;
    }

    private IEnumerator WolfTimeout()
    {
        yield return new WaitForSeconds(_wolfTimeout);
        ShapeShift();
    }

    private IEnumerator ShiftCooldown()
    {
        _canShift = false;
        yield return new WaitForSeconds(_shiftCooldown);
        _canShift = true;
    }
    
    [PunRPC]
    private void RpcShapeShift(bool isWolf)
    {
        Instantiate(_particles, transform.position + Vector3.up * 1f, Quaternion.identity);
        StartCoroutine(ShapeShiftModelSwitch(isWolf));
        
        if (!PhotonNetwork.IsMasterClient) return;
        if (isWolf) FlockHandler.WolvesToAvoid.Add(transform);
        else FlockHandler.WolvesToAvoid.Remove(transform);
    }

    private IEnumerator ShapeShiftModelSwitch(bool isWolf)
    {
        yield return new WaitForSeconds(_effectsTiming);
        _wolfModel.SetActive(isWolf);
        _sheepModel.SetActive(!isWolf);
    }

    private void Attack()
    {
        if (_isDead) return;
        if (!_isWolf) return;
        if (_onCooldown) return;
        
        if(_wolfAnimator)
            _wolfAnimator.SetTrigger("Attack");
        
        var results = Physics.OverlapSphere(transform.position, _distanceToKill, _sheepLayer);
        
        if (results.Length == 0) return;
        
        float minDistance = float.MaxValue;
        var closestSheep = results[0].gameObject;
        foreach (var sheepCol in results)
        {
            float distance = Vector3.Distance(transform.position, sheepCol.transform.position);
            if (!(distance < minDistance)) continue;
            minDistance = distance;
            closestSheep = sheepCol.gameObject;
        }

        HandleSheepAttack(closestSheep);
        //_photonView.RPC(nameof(CmdAttack), RpcTarget.MasterClient, PhotonView.Get(closestSheep).ViewID);

        StartCoroutine(StartAttackCooldown());
        UITimer.Instance.StartWolfAttackTimeout(_attackCooldown);
    }

    private void HandleSheepAttack(GameObject sheep)
    {
        if(sheep.TryGetComponent<DamageHandler>(out var damageHandler))
            damageHandler.ProcessDamage();
    }

    private void Graze()
    {
        if (_isGrazing) return;
        
        if(_grazeCoroutine != null)
            StopCoroutine(_grazeCoroutine);
        
        _grazeCoroutine = StartCoroutine(ProcessGraze());
    }
    
    private IEnumerator StartAttackCooldown()
    {
        _onCooldown = true;
        yield return new WaitForSeconds(_attackCooldown);
        _onCooldown = false;
    }
    
    private IEnumerator ProcessGraze()
    {
        _isGrazing = true;
        _player.PlayerController.DisableMovement = true;
        _sheepAnimator.SetBool("Graze", true);
        
        yield return new WaitForSeconds(3f);
        
        _isGrazing = false;
        _player.PlayerController.DisableMovement = false;
        _grazeCoroutine = null;
        _sheepAnimator.SetBool("Graze", false);
    }
}
