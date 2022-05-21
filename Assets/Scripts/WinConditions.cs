using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using TMPro;

public class WinConditions : MonoBehaviourPunCallbacks
{
    public static WinConditions Instance { get; private set; }
    
    private int sheeptotal = 0;
    private int wolfTotal = 0;
    
    private const string _SHEEPAMOUNT = "SheepAmount";
    private const string _WOLFKILLED = "WolfAmount";
    private const string _TIMER = "Timer";

    private Hashtable _roomProperties;

    public TMP_Text SheepCountText;
    public TMP_Text WolfCountText;
    public TMP_Text TimerText;
    
    private EndScreenUI endScreen;

    private float timeLeft = 60*5;
    
    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(gameObject);

        if (PhotonNetwork.IsMasterClient)
        {
            _roomProperties = new Hashtable
            {
                [_SHEEPAMOUNT] = GetSheepDifficulty(PlayerSpawner.WolfCount),
                [_WOLFKILLED] = PlayerSpawner.WolfCount,//Get wolfs from playerspawner
                [_TIMER] = timeLeft,
            };
        
            PhotonNetwork.CurrentRoom.SetCustomProperties(_roomProperties);
        }
        endScreen = GetComponent<EndScreenUI>();
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (timeLeft > 0)
            {
                timeLeft -= Time.deltaTime;
                UpdateTimer(timeLeft);
                _roomProperties[_TIMER] = timeLeft;
                PhotonNetwork.CurrentRoom.SetCustomProperties(_roomProperties);
            }
            else
            {
                endScreen.WinScreen(Team.Shepherd);
                timeLeft = 0;
            }
        }
    }

    private void UpdateTimer(float currentTime)
    {
        currentTime += 1;
        float min = Mathf.FloorToInt(currentTime/60);
        float sec = Mathf.FloorToInt(currentTime%60);

        TimerText.text = string.Format("{0:00} :{1:00}", min, sec);
    }
    
    public int GetSheepDifficulty(int PlayerCount)
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
        if (PhotonNetwork.IsMasterClient)
        {
            int newCount = (int)_roomProperties[_SHEEPAMOUNT];
            newCount += change;
            _roomProperties[_SHEEPAMOUNT] = newCount;
            PhotonNetwork.CurrentRoom.SetCustomProperties(_roomProperties);
        }
    }
    
    public void SetWolfKill()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            int newCount = (int)_roomProperties[_WOLFKILLED];
            newCount--;
            _roomProperties[_WOLFKILLED] = newCount;
            PhotonNetwork.CurrentRoom.SetCustomProperties(_roomProperties);
        }
    }
    
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);

        if (propertiesThatChanged.TryGetValue(_SHEEPAMOUNT, out var sheep))
        {
            if (sheeptotal == 0) sheeptotal = (int)sheep; 
            if ((int)sheep <= 0) endScreen.WinScreen(Team.Shepherd);
            if ((int)sheep >= 0) SheepCountText.text = "Sheep: " + sheep + "/" + sheeptotal;
        }

        if (propertiesThatChanged.TryGetValue(_WOLFKILLED, out var wolf))
        {
            if (wolfTotal == 0) wolfTotal = (int)wolf; 
            if ((int)wolf <= 0) endScreen.WinScreen(Team.Wolf);
            if ((int)wolf >= 0) WolfCountText.text = "Wolf: " + wolf + "/" + wolfTotal;
        }
        
        if (propertiesThatChanged.TryGetValue(_TIMER, out var timer))
        {
            UpdateTimer((float) timer);

            if ((float)timer <= 0)
            {
                endScreen.WinScreen(Team.Shepherd);
            }
            //Debug.LogError(timer);
        }
    }
}
