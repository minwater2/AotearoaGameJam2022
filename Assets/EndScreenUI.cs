using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team {Shepherd,Wolf};
public class EndScreenUI : MonoBehaviour
{
    [SerializeField] private GameObject _endGameUI;
    [SerializeField] private GameObject _shepherdWin;
    [SerializeField] private GameObject _wolfWin;
    
    public void WinScreen(Team winner)
    {
        _endGameUI.SetActive(true);
        if (winner == Team.Shepherd)_shepherdWin.SetActive(true);
        else _wolfWin.SetActive(true);
        
        //Show stats here
    }
}
