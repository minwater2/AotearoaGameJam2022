using System;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    private Rigidbody _rigidbody;
    private Vector3 _velocity;

    private bool _isWolf;
    public bool DisableMovement;
    
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _isWolf = TryGetComponent<WolfHandler>(out _);

        if (!PhotonNetwork.IsMasterClient) return;
        if (!_isWolf) FlockHandler.ShepardsToAvoid.Add(transform);
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
        if (DisableMovement)
        {
            if(_animator)
                _animator.SetFloat("MoveSpeed", 0f);
            return;
        }

        Vector3 deltaPosition;
        if (_isWolf) deltaPosition = _velocity.z * transform.forward;
        else deltaPosition = _velocity;

        if(_animator)
            _animator.SetFloat("MoveSpeed", _velocity.magnitude);
        _rigidbody.MovePosition(transform.position + deltaPosition * Time.deltaTime);
    }
}