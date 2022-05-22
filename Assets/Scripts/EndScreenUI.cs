using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum Team {Shepherd,Wolf};
public class EndScreenUI : MonoBehaviourPun
{
    [SerializeField] private GameObject _endGameUI;
    [SerializeField] private GameObject _shepherdWin;
    [SerializeField] private GameObject _wolfWin;
    [SerializeField] private Button _quitButton;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        _quitButton.onClick.AddListener(OnQuitButtonClicked);
    }
    
    private void OnDestroy()
    {
        _quitButton.onClick.RemoveAllListeners();
    }
    private void OnQuitButtonClicked() => Application.Quit();

    public void WinScreen(Team winner)
    {
        _endGameUI.SetActive(true);
        if (winner == Team.Shepherd) _shepherdWin.SetActive(true);
        if (winner == Team.Wolf) _wolfWin.SetActive(true);
        
        //TODO: Show stats here
    }

    public void RematchButton()
    {
        photonView.RPC(nameof(CmdRematch), RpcTarget.MasterClient);
    }

    [PunRPC]
    private void CmdRematch()
    {
        FlockHandler.Sheepsss.Clear();
        FlockHandler.WolvesToAvoid.Clear();
        FlockHandler.ShepardsToAvoid.Clear();
        PhotonNetwork.LoadLevel("LobbyScene");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            RematchButton();
        }
    }
}
