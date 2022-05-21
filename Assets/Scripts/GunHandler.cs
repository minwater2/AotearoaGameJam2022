using System;
using UnityEngine;

public class GunHandler : MonoBehaviour
{
    [SerializeField] private Shotgun _shotgun;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _shotgun.Shoot();
        }
    }
}