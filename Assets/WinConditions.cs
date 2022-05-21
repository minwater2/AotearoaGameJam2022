using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using TMPro;
using Unity.VisualScripting;

public class WinConditions : MonoBehaviourPunCallbacks
{
    public static WinConditions Instance { get; private set; }
    
    private int sheeptotal = 0;
    private int wolfTotal = 0;
    
    private const string _SHEEPAMOUNT = "SheepAmount";
    private const string _WOLFKILLED = "WolfAmount";
    private const string _WOLFTOTAL = "WolfTotal";
    private const string _SHEEPTOTAL = "SheepTotal";
    private const string _TIMER = "Timer";

    private Hashtable _roomProperties;

    public TMP_Text SheepCountText;
    public TMP_Text WolfCountText;

    private EndScreenUI endScreen;
    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
        
        if (PhotonNetwork.IsMasterClient)
        {
            
            _roomProperties = new Hashtable
            {
                [_SHEEPAMOUNT] = GetSheepDifficulty(PlayerSpawner.WolfCount),
                [_WOLFKILLED] = PlayerSpawner.WolfCount,//Get wolfs from playerspawner
            };
        
            PhotonNetwork.CurrentRoom.SetCustomProperties(_roomProperties);    
        }
        endScreen = GetComponent<EndScreenUI>();
    }
    
    public int GetSheepDifficulty(int PlayerCount)
    {
        switch (PlayerCount)
        {
            case 1: return FlockHandler.Sheepsss.Count/4;
            case 2: return FlockHandler.Sheepsss.Count/3;
            case 3: return FlockHandler.Sheepsss.Count/2;
            case 4: return FlockHandler.Sheepsss.Count/2;
            default: return FlockHandler.Sheepsss.Count/2;
        }
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
            //Debug.LogError(timer);
        }
    }
}
