using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class CloudGenerator : MonoBehaviourPun
{
    [SerializeField] private GameObject[] Clouds;
    [SerializeField] private Transform[] SpawnPoints;
    // Start is called before the first frame update
 
    IEnumerator CloudSpawning()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            yield return new WaitForSeconds(2.5f);
            photonView.RPC("RPCSpawnCloud", RpcTarget.All,Random.Range(0, Clouds.Length),Random.Range(0, SpawnPoints.Length));
            yield return StartCoroutine("CloudSpawning");
        }
    }

    IEnumerator Start()
    {
        yield return StartCoroutine("CloudSpawning");
    }

    [PunRPC]
    void RPCSpawnCloud(int Cloudindex ,int SpawnIndex)
    {
        Instantiate(Clouds[Cloudindex],SpawnPoints[SpawnIndex]);
    }
    
}
