using System;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviourPun
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Animator _animator2;
    private Rigidbody _rigidbody;
    private Vector3 _velocity;

    private bool _isWolf;
    public bool DisableMovement;
    [SerializeField] private GameObject[] _runningParticles;
    
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _isWolf = TryGetComponent<WolfHandler>(out _);

        if (photonView.AmOwner)
        {
            UITimer.Instance.Init(_isWolf);
        }
        
        if (!PhotonNetwork.IsMasterClient) return;
        if (!_isWolf) FlockHandler.ShepardsToAvoid.Add(transform);
    }

    public void Move(Vector3 velocity)
    {
        _velocity = velocity;

        if (!_isWolf && _velocity.magnitude > 0)
        {
            _runningParticles[0].gameObject.GetComponent<ParticleSystem>().enableEmission = true;
        }
    }

    public void LookAt(Vector3 point)
    {
        var correctHeightPoint = new Vector3(point.x, transform.position.y, point.z);
        transform.LookAt(correctHeightPoint);
    }
    
    private void FixedUpdate()
    {
        if (_velocity.magnitude <= 0)
        {
            if (!_isWolf)
            {
                _runningParticles[0].GetComponent<ParticleSystem>().enableEmission = false;
            }
        }
        
        if (DisableMovement)
        {
            if(_animator)
                _animator.SetFloat("MoveSpeed", 0f);
            
            if(_animator2)
                _animator2.SetFloat("MoveSpeed", 0f);
            return;
        }

        Vector3 deltaPosition;
        if (_isWolf) deltaPosition = _velocity.z * transform.forward;
        else deltaPosition = _velocity;

        if(_animator)
            _animator.SetFloat("MoveSpeed", _velocity.magnitude);
        
        if(_animator2)
            _animator2.SetFloat("MoveSpeed", _velocity.magnitude);
        
        _rigidbody.MovePosition(transform.position + deltaPosition * Time.deltaTime);
    }
}