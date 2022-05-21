using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    private const string _PLAYER_NAME = "PLAYER_NAME";
    [SerializeField] private int _maxPlayers = 5;
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _forceStartButton;
    [SerializeField] private Button _cancelPlayButton;
    [SerializeField] private TextMeshProUGUI _waitingText;
    [SerializeField] private TMP_InputField _nameField;
    [SerializeField] private string _mainSceneName;
    
    private void Awake()
    {
        _playButton.onClick.AddListener(OnPlayButtonClicked);
        _forceStartButton.onClick.AddListener(OnForceStartButtonClicked);
        _cancelPlayButton.onClick.AddListener(OnCancelPlayButtonClicked);
        
        _nameField.onEndEdit.AddListener(OnNameEntered);
    }

    private void OnDestroy()
    {
        _playButton.onClick.RemoveAllListeners();
        _forceStartButton.onClick.RemoveAllListeners();
        _cancelPlayButton.onClick.RemoveAllListeners();
        _nameField.onEndEdit.RemoveAllListeners();
    }

    // Start is called before the first frame update
    void Start()
    {
        _waitingText.gameObject.SetActive(false);
        _forceStartButton.gameObject.SetActive(false);
        _cancelPlayButton.gameObject.SetActive(false);
        _playButton.interactable = false;

        if (PlayerPrefs.HasKey(_PLAYER_NAME))
        {
            PhotonNetwork.NickName = PlayerPrefs.GetString(_PLAYER_NAME, "Wolf");
            _nameField.text = PhotonNetwork.NickName;
        }

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

    private void OnPlayButtonClicked()
    {
        _nameField.gameObject.SetActive(false);
        Connect();
    }

    private void OnForceStartButtonClicked()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.LoadLevel(_mainSceneName);
    }
    
    private void OnCancelPlayButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
        _waitingText.gameObject.SetActive(false);
        _forceStartButton.gameObject.SetActive(false);
        _playButton.gameObject.SetActive(true);
        _nameField.gameObject.SetActive(true);
        _cancelPlayButton.gameObject.SetActive(false);
    }

    private void OnNameEntered(string nameString)
    {
        PhotonNetwork.NickName = nameString;
        PlayerPrefs.SetString(_PLAYER_NAME, nameString);
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
        _cancelPlayButton.gameObject.SetActive(true);
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        if (PhotonNetwork.CurrentRoom.PlayerCount != PhotonNetwork.CurrentRoom.MaxPlayers) return;

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.LoadLevel(_mainSceneName);
        }
    }

    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);
        _forceStartButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
    }
}
