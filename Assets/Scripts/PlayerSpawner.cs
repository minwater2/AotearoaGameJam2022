using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    private const string _SHEPHERD_PROPERTY = "Shepherd";
    private const string _WOLF_PROPERTY = "Wolves";
    
    [SerializeField] private GameObject _shepherdPrefab;
    [SerializeField] private GameObject _wolfPrefab;

    [SerializeField] private Vector3 _shepherdSpawn;
    [SerializeField] private Transform[] _wolvesSpawnPoints;
    
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
            AssignRoles();
    }

    private void AssignRoles()
    {
        var players = PhotonNetwork.CurrentRoom.Players.Keys.ToList();
        
        var shepherdId = players[Random.Range(0, PhotonNetwork.CurrentRoom.PlayerCount)];

        players.Remove(shepherdId);
        
        var roomProperties = new Hashtable
        {
            [_SHEPHERD_PROPERTY] = shepherdId,
            [_WOLF_PROPERTY] = players.ToArray(),
        };
        
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);
        if (propertiesThatChanged.TryGetValue(_SHEPHERD_PROPERTY, out object actorNumber))
        {
            if ((int)actorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
                PhotonNetwork.Instantiate(_shepherdPrefab.name, _shepherdSpawn, Quaternion.identity);
        }
        if (propertiesThatChanged.TryGetValue(_WOLF_PROPERTY, out object actorNumbers))
        {
            var wolves = ((int[]) actorNumbers).ToList();
            if (!wolves.Contains(PhotonNetwork.LocalPlayer.ActorNumber)) return;
            
            var randomSpawnPos = _wolvesSpawnPoints[Random.Range(0, _wolvesSpawnPoints.Length)].position;
            PhotonNetwork.Instantiate(_wolfPrefab.name, randomSpawnPos, Quaternion.identity);
        }
    }
}
