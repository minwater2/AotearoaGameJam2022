using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class FlockHandler : MonoBehaviour
{
    private const float _SHEEP_HEIGHT = 10f;

    public static readonly List<Transform> Sheepsss = new();
    private static int _totalSheep;
    
    [SerializeField] private string _sheepName = "Sheep";
    [SerializeField] private Vector2Int _minMaxNumSheep;
    [SerializeField] private int _mapSize;

    private void Awake()
    {
        SpawnSheep();
    }

    private void SpawnSheep()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        _totalSheep = Random.Range(_minMaxNumSheep.x, _minMaxNumSheep.y);

        for (int i = 0; i < _totalSheep; i++)
        {
            int x = Random.Range(0, _mapSize) - _mapSize / 2;
            int z = Random.Range(0, _mapSize) - _mapSize / 2;
            var position = new Vector3(x + transform.position.x, _SHEEP_HEIGHT, z + transform.position.z);
            
            float yRotation = Random.Range(0, 360);
            var rotation = Quaternion.Euler(new Vector3(0, yRotation, 0));

            var sheep = PhotonNetwork.Instantiate(_sheepName, position, rotation);
            
            Sheepsss.Add(sheep.transform);
        }
    }
}
