using System.Collections.Generic;
using UnityEngine;

public class FlockHandler : MonoBehaviour
{
    private const int _SHEEP_HEIGHT = 1;

    public static readonly List<Transform> Sheepsss = new();
    
    [SerializeField] private GameObject _sheep;
    [SerializeField] private Vector2Int _minMaxNumSheep;
    [SerializeField] private int _mapSize;

    private void Start()
    {
        SpawnSheep();
    }

    private void SpawnSheep()
    {
        int numSheep = Random.Range(_minMaxNumSheep.x, _minMaxNumSheep.y);

        for (int i = 0; i < numSheep; i++)
        {
            var sheep = Instantiate(_sheep, transform);
            int x = Random.Range(0, _mapSize) - _mapSize / 2;
            int z = Random.Range(0, _mapSize) - _mapSize / 2;
            sheep.transform.position = new Vector3(x, _SHEEP_HEIGHT, z);

            float yRotation = Random.Range(0, 360);
            var rotation = Quaternion.Euler(new Vector3(0, yRotation, 0));
            sheep.transform.rotation = rotation;
            
            Sheepsss.Add(sheep.transform);
        }
    }
}
