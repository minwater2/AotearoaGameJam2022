
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

public class PlayerSpawner : MonoBehaviourPun
{
    private const string _SHEPHERD_PROPERTY = "Shepherd";
    
    [SerializeField] private GameObject _shepherdPrefab;
    [SerializeField] private GameObject _wolfPrefab;

    [SerializeField] private Vector3 _shepherdSpawn;
    [SerializeField] private Vector3[] _wolvesSpawnPoints;
    
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
            RandomlySetShepherd();
        
        var shepherdId = (int)PhotonNetwork.CurrentRoom.CustomProperties[_SHEPHERD_PROPERTY];
        
        // check if shepherdId == playerID
        if (shepherdId == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            PhotonNetwork.Instantiate(_shepherdPrefab.name, _shepherdSpawn, Quaternion.identity);
        }
        else
        {
            var randomSpawnPos = _wolvesSpawnPoints[Random.Range(0, _wolvesSpawnPoints.Length)];
            PhotonNetwork.Instantiate(_wolfPrefab.name, randomSpawnPos, Quaternion.identity);
        }
    }

    private void RandomlySetShepherd()
    {
        var shepherdId =
            PhotonNetwork.CurrentRoom.Players.Keys.ToList()[Random.Range(0, PhotonNetwork.CurrentRoom.PlayerCount)];

        var roomProperties = new Hashtable {[_SHEPHERD_PROPERTY] = shepherdId};
        
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
    }
}
