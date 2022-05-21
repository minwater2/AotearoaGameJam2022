using System.Collections;
using Photon.Pun;
using UnityEngine;

public class WolfHandler : MonoBehaviourPun
{
    [SerializeField] private GameObject _wolfModel;
    [SerializeField] private GameObject _sheepModel;
    [SerializeField] private LayerMask _sheepLayer;
    [SerializeField] private float _distanceToKill = 5f;
    [SerializeField] private float _attackCooldown = 5f;

    private PhotonView _photonView;
    
    private bool _isWolf;
    private bool _onCooldown;

    private void Awake()
    {
        _photonView = PhotonView.Get(this);
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
        
        _photonView.RPC(nameof(CmdAttack), RpcTarget.MasterClient, PhotonView.Get(closestSheep).ViewID);

        StartCoroutine(StartAttackCooldown());
    }

    [PunRPC]
    private void CmdAttack(int id)
    {
        var deadSheep = PhotonView.Find(id).gameObject;
        FlockHandler.Sheepsss.Remove(deadSheep.transform);
        PhotonNetwork.Destroy(deadSheep);
    }
    
    private IEnumerator StartAttackCooldown()
    {
        _onCooldown = true;
        yield return new WaitForSeconds(_attackCooldown);
        _onCooldown = false;
    }
}
