using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using TMPro;

public class WinConditions : MonoBehaviourPunCallbacks
{
    public static WinConditions Instance { get; private set; }

    private const string _SHEEPAMOUNT = "SheepAmount";
    private const string _WOLFKILLED = "WolfAmount";
    private const string _TIMER = "Timer";
    
    public int SheepAmountStart = 50;
    public int WolfsKilled;
    public float Timer;

    private Hashtable _roomProperties;

    public TMP_Text SheepCountText;
    public TMP_Text WolfCountText;
    
    private void Awake()
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
                [_SHEEPAMOUNT] = SheepAmountStart,
                [_WOLFKILLED] = 2,//Get wolfs from playerspawner
            };
        
            PhotonNetwork.CurrentRoom.SetCustomProperties(_roomProperties);    
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

    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            SetSheepCount(-1);
            SetWolfKill();
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);

        if (propertiesThatChanged.TryGetValue(_SHEEPAMOUNT, out var sheep))
        {
            if ((int)sheep >= 0) SheepCountText.text = "Sheep: " + sheep + "/" + SheepAmountStart;
        }

        if (propertiesThatChanged.TryGetValue(_WOLFKILLED, out var wolf))
        {
            if ((int)wolf >= 0) WolfCountText.text = "Wolf: " + wolf + "/" + 4;
        }
        
        if (propertiesThatChanged.TryGetValue(_TIMER, out var timer))
        {
            //Debug.LogError(timer);
        }
    }
}
