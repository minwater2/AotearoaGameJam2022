using System;
using Photon.Pun;
using UnityEngine;

public class GunHandler : MonoBehaviourPun
{
    [SerializeField] private Shotgun _shotgun;
    
    private void Update()
    {
        if (photonView.IsMine)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _shotgun.Shoot();
            }
        }
     
    }
}