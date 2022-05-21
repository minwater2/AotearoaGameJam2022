using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(DamageHandler))]
public class WolfHandler : MonoBehaviourPun
{
    [SerializeField] private GameObject _wolfModel;
    [SerializeField] private GameObject _sheepModel;
    [SerializeField] private LayerMask _sheepLayer;
    [SerializeField] private float _distanceToKill = 5f;
    [SerializeField] private float _attackCooldown = 5f;
    [SerializeField] private float _effectsTiming = 0.5f;
    [SerializeField] private float _stunTiming = 1f;
    [SerializeField] private float _wolfTimeout = 5f;
    [SerializeField] private float _shiftCooldown = 5f;
    [SerializeField] ParticleSystem _particles;
    [SerializeField] private float _wolfSpeed = 10f;
    
    private PhotonView _photonView;
    private DamageHandler _damageHandler;
    private Player _player;

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
        _sheepSpeed = _player.MoveSpeed;
        _damageHandler.OnDeath += OnDeath;
    }

    private void OnDestroy()
    {
        _damageHandler.OnDeath -= OnDeath;
    }

    private void OnDeath()
    {
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
        
        if (_isWolf) StartCoroutine(WolfTimeout());
        else
        {
            StopCoroutine(WolfTimeout());
            StartCoroutine(ShiftCooldown());
        }
        
        StartCoroutine(ShapeShiftSpeed());
        _photonView.RPC(nameof(RpcShapeShift), RpcTarget.All, _isWolf);
    }

    private IEnumerator ShapeShiftSpeed()
    {
        _player.MoveSpeed = 0;
        yield return new WaitForSeconds(_stunTiming);
        _player.MoveSpeed = _isWolf ? _wolfSpeed : _sheepSpeed;
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
        _particles.Play();
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
