using System;
using Photon.Pun;
using UnityEngine;

public class GunHandler : MonoBehaviourPun
{
    [SerializeField] private Shotgun _shotgun;

    private PlayerController _controller;

    private void Awake()
    {
        _controller = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            if (_shotgun.CanShoot && _controller.DisableMovement)
                _controller.DisableMovement = false;
            
            if (Input.GetMouseButtonDown(0))
            {
                _controller.DisableMovement = true;
                _shotgun.Shoot();
            }
        }
     
    }
}