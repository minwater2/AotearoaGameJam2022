using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private int _maxPlayers = 5;
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _forceStartButton;
    [SerializeField] private TextMeshProUGUI _waitingText;
    [SerializeField] private string _mainSceneName;
    
    private void Awake()
    {
        _playButton.onClick.AddListener(OnPlayButtonClicked);
        _forceStartButton.onClick.AddListener(OnForceStartButtonClicked);
    }

    private void OnDestroy()
    {
        _playButton.onClick.RemoveAllListeners();
        _forceStartButton.onClick.RemoveAllListeners();
    }

    // Start is called before the first frame update
    void Start()
    {
        _waitingText.gameObject.SetActive(false);
        _forceStartButton.gameObject.SetActive(false);
        _playButton.interactable = false;
        
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.OfflineMode = false;
            PhotonNetwork.GameVersion = "0.0.1";
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    
    private void Update()
    {
        if (!PhotonNetwork.IsConnected) return;
        if (PhotonNetwork.CurrentRoom == null) return;
        
        _waitingText.text = 
            $"Waiting for players ... {(int)PhotonNetwork.CurrentRoom.PlayerCount}/{(int)PhotonNetwork.CurrentRoom.MaxPlayers}";
    }
    
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        _playButton.interactable = true;
    }

    private void OnPlayButtonClicked() => Connect();
    
    private void OnForceStartButtonClicked()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.LoadLevel(_mainSceneName);
    }

    public void Connect()
    {
        if (!PhotonNetwork.IsConnected) return;
        
        PhotonNetwork.JoinRandomOrCreateRoom(null, 
            0, 
            MatchmakingMode.FillRoom,
            null,
            null,
            null,
            new RoomOptions{MaxPlayers = (byte)_maxPlayers});
        
        _playButton.gameObject.SetActive(false);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        
        _waitingText.gameObject.SetActive(true);
        _forceStartButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        if (PhotonNetwork.CurrentRoom.PlayerCount != PhotonNetwork.CurrentRoom.MaxPlayers) return;

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.LoadLevel(_mainSceneName);
        }
    }

}
