using System.Collections;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(DamageHandler))]
public class SheepController : MonoBehaviourPun
{
    [SerializeField] private float _speed = 20f;
    [SerializeField] private float _momentum = 20f;
    [SerializeField] private float _coherence = 10f;
    [SerializeField] private float _separation = 50f;
    [SerializeField] private float _avoidance = 10f;
    [SerializeField] private float _alignment = 20f;
    [SerializeField] private float _shepardAvoidance = 3f;
    [SerializeField] private float _wolfAvoidance = 3f;
    [SerializeField] private float _viewDistance = 10f;
    [SerializeField] private float _avoidanceDistance = 2f;
    [SerializeField] private float _grazeChance = 2f;
    [SerializeField] private float _grazeTime = 2f;
    [SerializeField] private LayerMask _obstacleLayer;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _heightOffset = 1f;
    
    private Rigidbody _rigidbody;
    private Collider _collider;
    private MeshRenderer _meshRenderer;
    private DamageHandler _damageHandler;
    
    private bool _grazing;
    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _damageHandler = GetComponent<DamageHandler>();
        _collider = GetComponent<Collider>();
        _meshRenderer = GetComponentInChildren<MeshRenderer>();
        _damageHandler.OnDeath += OnDeath;
    }

    private void OnDestroy()
    {
        _damageHandler.OnDeath -= OnDeath;
    }

    private void OnDeath()
    {
        _meshRenderer.enabled = false;
        _collider.enabled = false;
        _rigidbody.isKinematic = true;
        enabled = false;

        if (!PhotonNetwork.IsMasterClient) return;
        
        FlockHandler.Sheepsss.Remove(transform);
        WinConditions.Instance.SetSheepCount(-1);
    }

    private void LateUpdate()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 10f, _groundLayer))
        {
            transform.position = hit.point + new Vector3(0, _heightOffset, 0);
        }
    }

    private void FixedUpdate()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (_grazing) return;

        if (Random.Range(0f, 100f) <= _grazeChance)
        {
            StartCoroutine(Grazing());
        }
        
        // coherence
        var coherenceSheepVector = Vector3.zero;
        var separationSheepVector = Vector3.zero;
        var alignmentSheepVector = Vector3.zero;
        foreach (var sheep in FlockHandler.Sheepsss)
        {
            float distance = Vector3.Distance(sheep.position, transform.position);
            if (distance > _viewDistance) continue;
            var direction = sheep.position - transform.position;
            //if (Vector3.Dot(transform.forward, direction) < 0) continue;
            
            coherenceSheepVector += direction.normalized;
            alignmentSheepVector += sheep.forward;
            
            if (distance > _avoidanceDistance) continue;

            separationSheepVector += direction.normalized;
        }

        var shepardAvoidanceVector = Vector3.zero;
        
        foreach (var shepard in FlockHandler.ShepardsToAvoid)
        {
            float distance = Vector3.Distance(shepard.position, transform.position);
            if (distance > _viewDistance) continue;
            shepardAvoidanceVector += shepard.position - transform.position;
        }

        var wolfAvoidanceVector = Vector3.zero;

        foreach (var wolf in FlockHandler.WolvesToAvoid)
        {
            float distance = Vector3.Distance(wolf.position, transform.position);
            if (distance > _viewDistance) continue;
            wolfAvoidanceVector += wolf.position - transform.position;
        }
        
        // coherence
        coherenceSheepVector.y = 0;
        coherenceSheepVector = coherenceSheepVector.normalized;

        // separation
        separationSheepVector.y = 0;
        separationSheepVector = -separationSheepVector.normalized;
        
        // alignment
        alignmentSheepVector.y = 0;
        alignmentSheepVector = alignmentSheepVector.normalized;
        
        // shepard avoidance
        shepardAvoidanceVector.y = 0;
        shepardAvoidanceVector = -shepardAvoidanceVector.normalized;
        
        // wolf avoidance
        wolfAvoidanceVector.y = 0;
        wolfAvoidanceVector = -wolfAvoidanceVector.normalized;
        
        var obstacleAvoidanceVector = Vector3.zero;
        
        // obstacle avoidance
        var left = (transform.forward + -transform.right) / 2;
        var right = (transform.forward + transform.right) / 2;
        if (Physics.Raycast(transform.position, left, _viewDistance, _obstacleLayer))
        {
            obstacleAvoidanceVector = -left;
            Debug.DrawRay(transform.position, left);
        }
        else if (Physics.Raycast(transform.position, right, _viewDistance, _obstacleLayer))
        {
            obstacleAvoidanceVector = -right;
            Debug.DrawRay(transform.position, right);
        }
        
        
        // set rotation and speed
        transform.forward = (coherenceSheepVector * _coherence + separationSheepVector * _separation +
                             alignmentSheepVector * _alignment + shepardAvoidanceVector * _shepardAvoidance +
                             wolfAvoidanceVector * _wolfAvoidance + obstacleAvoidanceVector * _avoidance + 
                             transform.forward * _momentum).normalized;
        float speed;

        if (wolfAvoidanceVector == Vector3.zero)
        {
            speed = _speed;
        }
        else
        {
            speed =  _speed * 2;
            StopCoroutine(Grazing());
            _grazing = false;
            _rigidbody.isKinematic = false;
        }
        
        _rigidbody.velocity = transform.forward * speed;
    }

    private IEnumerator Grazing()
    {
        _grazing = true;
        _rigidbody.isKinematic = true;
        yield return new WaitForSeconds(_grazeTime);
        _grazing = false;
        _rigidbody.isKinematic = false;
    }
}
