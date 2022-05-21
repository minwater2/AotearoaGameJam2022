using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootEvent : MonoBehaviour
{
    [SerializeField] private Shotgun _gun;
    [SerializeField] private PlayerController _controller;
    
    public void Shoot() => _gun.Shoot();

    public void EnableMovement() => _controller.DisableMovement = false;
}
