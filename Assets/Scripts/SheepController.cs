using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SheepController : MonoBehaviour
{
    [SerializeField] private float _coherence = 10f;
    [SerializeField] private float _separation = 50f;
    [SerializeField] private float _viewDistance = 10f;
    [SerializeField] private float _avoidanceDistance = 2f;
    
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // coherence
        var coherenceSheepVector = Vector3.zero;
        var separationSheepVector = Vector3.zero;
        foreach (var sheep in FlockHandler.Sheepsss)
        {
            float distance = Vector3.Distance(sheep.position, transform.position);
            if (distance > _viewDistance) continue;
            var direction = sheep.position - transform.position;
            if (Vector3.Dot(transform.forward, direction) < 0) continue;
            
            coherenceSheepVector += direction.normalized;

            if (distance > _avoidanceDistance) continue;

            separationSheepVector += direction.normalized;
        }
        
        // coherence
        coherenceSheepVector.y = 0;
        coherenceSheepVector = coherenceSheepVector.normalized;

        _rigidbody.AddForce(_coherence * Time.deltaTime * coherenceSheepVector);

        // separation
        separationSheepVector.y = 0;
        separationSheepVector = -separationSheepVector.normalized;
        
        _rigidbody.AddForce(_separation * Time.deltaTime * separationSheepVector);
        
        Debug.DrawRay(transform.position, coherenceSheepVector * 2, Color.red, Time.deltaTime);
        Debug.DrawRay(transform.position, separationSheepVector, Color.magenta, Time.deltaTime);
    }
}
