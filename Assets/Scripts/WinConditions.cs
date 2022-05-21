using System.Collections;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using TMPro;

public class WinConditions : MonoBehaviourPun
{
    public static WinConditions Instance { get; private set; }
    
    private int _sheepTotal;
    private int _wolfTotal;

    private int _currentSheep;
    private int _currentWolves;
    
    public TMP_Text SheepCountText;
    public TMP_Text WolfCountText;
    public TMP_Text TimerText;
    
    private EndScreenUI endScreen;

    private float timeLeft = 60*5;
    
    private IEnumerator Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(gameObject);
        
        endScreen = GetComponent<EndScreenUI>();

        yield return new WaitForEndOfFrame(); //wait for sheep to spawn

        if (!PhotonNetwork.IsMasterClient) yield break;
        _sheepTotal = GetSheepCountForPlayers(PlayerSpawner.WolfCount);
        _wolfTotal = PlayerSpawner.WolfCount;
        _currentSheep = _sheepTotal;
        _currentWolves = _wolfTotal;
        
        WolfCountText.text = "Wolf: " + _wolfTotal + "/" + _wolfTotal;
        SheepCountText.text = "Sheep: " + _sheepTotal + "/" + _sheepTotal;
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        if (timeLeft > 0) timeLeft -= Time.deltaTime;
        photonView.RPC(nameof(RpcTime), RpcTarget.All, timeLeft);
    }

    private void UpdateTimer(float currentTime)
    {
        currentTime += 1;
        float min = Mathf.FloorToInt(currentTime/60);
        float sec = Mathf.FloorToInt(currentTime%60);

        TimerText.text = string.Format("{0:00} :{1:00}", min, sec);
    }
    
    public int GetSheepCountForPlayers(int PlayerCount)
    { 
        return PlayerCount switch
        {
            1 => FlockHandler.Sheepsss.Count / 4,
            2 => FlockHandler.Sheepsss.Count / 3,
            3 => FlockHandler.Sheepsss.Count / 2,
            4 => FlockHandler.Sheepsss.Count / 2,
            _ => FlockHandler.Sheepsss.Count / 2
        };
    }
    
    public void SetSheepCount(int change)
    {
        _currentSheep += change;
        photonView.RPC(nameof(RpcShepard), RpcTarget.All, _currentSheep);
    }
    
    public void SetWolfKill()
    {
        _currentWolves--;
        photonView.RPC(nameof(RpcWolves), RpcTarget.All, _currentWolves);
    }

    [PunRPC]
    private void RpcWolves(int count)
    {
        WolfCountText.text = "Wolf: " + count + "/" + _wolfTotal;
        if (count <= 0)
        {
            endScreen.WinScreen(Team.Shepherd);
        }
    }

    [PunRPC]
    private void RpcShepard(int count)
    {
        SheepCountText.text = "Sheep: " + count + "/" + _sheepTotal;
        if (count <= 0)
        {
            endScreen.WinScreen(Team.Wolf);
        }
    }

    [PunRPC]
    private void RpcTime(float time)
    {
        UpdateTimer(time);
        if (time <= 0)
        {
            endScreen.WinScreen(Team.Shepherd);
        }
    }
}
