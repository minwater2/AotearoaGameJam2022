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
    
    private PhotonView _photonView;
    private DamageHandler _damageHandler;
    private Player _player;
    private Rigidbody _rigidbody;
    
    private float _sheepSpeed;
    
    private bool _isWolf;
    private bool _onCooldown;
    private bool _isDead;
    private bool _canShift = true;

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
        
        if (PhotonNetwork.IsMasterClient)
            WinConditions.Instance.SetWolfKill();
    }

    void Update()
    {
        if (!_photonView.IsMine) return;
        
        if (Input.GetKeyDown(KeyCode.Space)) ShapeShift();
        else if (Input.GetMouseButtonDown(0)) Attack();
    }
    
    private void ShapeShift()
    {
        if (_isDead) return;
        if (!_canShift) return;
        
        _isWolf = !_isWolf;
        
        _player.MoveSpeed = _isWolf ? _wolfSpeed : _sheepSpeed;

        StopCoroutine(WolfTimeout());
        
        if (_isWolf)
        {
            StartCoroutine(WolfTimeout());
            FlockHandler.PlayersToAvoid.Add(transform);
        }
        else
        {
            StartCoroutine(ShiftCooldown());
            FlockHandler.PlayersToAvoid.Remove(transform);
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
    }

    private void HandleSheepAttack(GameObject sheep)
    {
        if(sheep.TryGetComponent<DamageHandler>(out var damageHandler))
            damageHandler.ProcessDamage();
    }

    [PunRPC]
    private void CmdHandleWolfDeath(int viewId)
    {
        if (viewId == photonView.ViewID)
        {
            _wolfModel.SetActive(false);
            _sheepModel.SetActive(false);
            _isDead = true;
        }
    }
    
    private IEnumerator StartAttackCooldown()
    {
        _onCooldown = true;
        yield return new WaitForSeconds(_attackCooldown);
        _onCooldown = false;
    }
}
