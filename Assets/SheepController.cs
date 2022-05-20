using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SheepController : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _viewDistance = 5f;
    
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        var totalVisibleSheepVector = Vector3.zero;
        foreach (var sheep in FlockHandler.Sheepsss)
        {
            if (Vector3.Distance(sheep.position, transform.position) > _viewDistance) continue;
            var direction = sheep.position - transform.position;
            if (Vector3.Dot(transform.forward, direction) < 0) continue;

            totalVisibleSheepVector += direction.normalized;
        }

        var avoidSheepVector = -totalVisibleSheepVector.normalized;
        float avoidSheepAngle = Mathf.Rad2Deg * Mathf.Atan2(avoidSheepVector.z, avoidSheepVector.x);
        
        transform.rotation = Quaternion.Euler(new Vector3(0, avoidSheepAngle, 0));
        
        _rigidbody.velocity = _speed * transform.forward;
    }
}
