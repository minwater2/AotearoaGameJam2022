using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Team {Shepherd,Wolf};
public class EndScreenUI : MonoBehaviourPun
{
    [SerializeField] private GameObject _endGameUI;
    [SerializeField] private GameObject _shepherdWin;
    [SerializeField] private GameObject _wolfWin;
    
    public void WinScreen(Team winner)
    {
        _endGameUI.SetActive(true);
        if (winner == Team.Shepherd) _shepherdWin.SetActive(true);
        if (winner == Team.Wolf) _wolfWin.SetActive(true);
        
        //Show stats here
    }

    public void RematchButton()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("MainLevel");    
        }
    }
}
