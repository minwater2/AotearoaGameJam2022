using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private Vector3 _velocity;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Move(Vector3 velocity)
    {
        _velocity = velocity;
    }

    public void LookAt(Vector3 point)
    {
        var correctHeightPoint = new Vector3(point.x, transform.position.y, point.z);
        transform.LookAt(correctHeightPoint);
    }
    
    private void FixedUpdate()
    {
        _rigidbody.MovePosition(_rigidbody.position + _velocity * Time.deltaTime);
    }
}