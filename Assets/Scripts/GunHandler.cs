using System.Collections;
using Photon.Pun;
using UnityEngine;

public class GunHandler : MonoBehaviourPun
{
    [SerializeField] private Shotgun _shotgun;
    [SerializeField] private float _shotCooldown = 1.25f;
    [SerializeField] private Animator _animator;
    
    private PlayerController _controller;
    private bool _canShoot = true;
    
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
                if (!_canShoot) return;
                
                _controller.DisableMovement = true;
                _animator.SetTrigger("Shoot");
                StartCoroutine(ProcessCooldown());
                UITimer.Instance.StartShepardAttackTimeout(_shotCooldown);
            }
        }
    }
    
    
    private IEnumerator ProcessCooldown()
    {
        _canShoot = false;

        yield return new WaitForSeconds(_shotCooldown);
        
        _canShoot = true;
    }
}