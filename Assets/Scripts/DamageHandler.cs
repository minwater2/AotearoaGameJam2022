using System;
using Photon.Pun;

public class DamageHandler : MonoBehaviourPun
{
    private PhotonView _photonView;

    public event Action OnDeath;

    private void Awake()
    {
        _photonView = PhotonView.Get(this);
    }

    public void ProcessDamage()
    {
        _photonView.RPC(nameof(CmdHandleDeath), RpcTarget.MasterClient, _photonView.ViewID);
    }
    
    
    [PunRPC]
    private void CmdHandleDeath(int viewId)
    {
        if (viewId == _photonView.ViewID)
        {
            OnDeath?.Invoke();
        }
    }
    
    //
    // [PunRPC]
    // private void CmdAttack(int id)
    // {
    //     var deadSheep = PhotonView.Find(id).gameObject;
    //     FlockHandler.Sheepsss.Remove(deadSheep.transform);
    //     PhotonNetwork.Destroy(deadSheep);
    //     WinConditions.Instance.SetSheepCount(-1);
    // }
}