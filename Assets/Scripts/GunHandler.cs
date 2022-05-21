using System;
using Photon.Pun;
using UnityEngine;

public class GunHandler : MonoBehaviourPun
{
    [SerializeField] private Shotgun _shotgun;
    [SerializeField] private Animator _animator;

    private PlayerController _controller;

    private void Awake()
    {
        _controller = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (_shotgun.CanShoot)
                {
                    _controller.DisableMovement = true;
                    _animator.SetTrigger("Shoot");
                }
            }
        }
    }
}