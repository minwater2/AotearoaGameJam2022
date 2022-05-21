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
    [SerializeField] private ParticleSystem _particles;

    private PhotonView _photonView;
    private DamageHandler _damageHandler;
    
    private bool _isWolf;
    private bool _onCooldown;

    private void Awake()
    {
        _photonView = PhotonView.Get(this);
        
        _damageHandler = GetComponent<DamageHandler>();
        _damageHandler.OnDeath += OnDeath;
    }

    private void OnDestroy()
    {
        _damageHandler.OnDeath -= OnDeath;
    }

    private void OnDeath()
    {
        PhotonNetwork.Destroy(gameObject);
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
        _isWolf = !_isWolf;
        _photonView.RPC(nameof(RpcShapeShift), RpcTarget.All, _isWolf);
    }
    
    [PunRPC]
    private void RpcShapeShift(bool isWolf)
    {
        _particles.Play();
        _wolfModel.SetActive(isWolf);
        _sheepModel.SetActive(!isWolf);
    }

    private void Attack()
    {
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

    private IEnumerator StartAttackCooldown()
    {
        _onCooldown = true;
        yield return new WaitForSeconds(_attackCooldown);
        _onCooldown = false;
    }
}
