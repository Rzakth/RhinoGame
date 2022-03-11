using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Chat;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using AuthenticationValues = Photon.Chat.AuthenticationValues;

public class MultiplayerLobby : MonoBehaviourPunCallbacks
{
    [Header("Login Panel")]
    public Transform LoginPanel;
    public InputField PlayerName;


    [Header("Selection Panel")]
    public Transform SelectionPanel;

    [Header("Create Room Panel")]
    public Transform CreateRoomPanel;
    public InputField RoomName;

    [Header("Inside Room Panel")]
    public Transform InsideRoomPanel;
    public GameObject TextPrefab;
    public Transform InsideRoomPanelContent;
    public GameObject StartGameButton;

    [Header("List Rooms Panel")]
    public Transform ListRoomsPanel;
    public GameObject RoomEntryPrefab;
    public Transform ListRoomPanelContent;

    [Header("Chat Panel")]
    public Chat chat;
    public Transform ChatPanel;

    private Dictionary<string, RoomInfo> cachedRoomList;
    private Dictionary<string, GameObject> roomListEntries;
    private Dictionary<int, GameObject> playerListEntries;


    public void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        
        cachedRoomList = new Dictionary<string, RoomInfo>();
        roomListEntries = new Dictionary<string, GameObject>();

        PlayerName.text = "Player" + Random.Range(1, 10000);
    }

    #region Panels
    public void ActivatePanel(string panelName)
    {
        DisableAllPanels();

        if (panelName == LoginPanel.gameObject.name)
            LoginPanel.gameObject.SetActive(true);
        else if (panelName == SelectionPanel.gameObject.name)
            SelectionPanel.gameObject.SetActive(true);
        else if (panelName == CreateRoomPanel.gameObject.name)
            CreateRoomPanel.gameObject.SetActive(true);
        else if (panelName == InsideRoomPanel.gameObject.name)
            InsideRoomPanel.gameObject.SetActive(true);
        else if (panelName == ListRoomsPanel.gameObject.name)
            ListRoomsPanel.gameObject.SetActive(true);
        else if (panelName == ChatPanel.gameObject.name)
            ChatPanel.gameObject.SetActive(true);
    }

    public void DisableAllPanels()
    {
        LoginPanel.gameObject.SetActive(false);
        SelectionPanel.gameObject.SetActive(false);
        CreateRoomPanel.gameObject.SetActive(false);
        InsideRoomPanel.gameObject.SetActive(false);
        ListRoomsPanel.gameObject.SetActive(false);
        ChatPanel.gameObject.SetActive(false);
    }

    #endregion
    private void DeleteChildren(Transform parent)
    {
        foreach(Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }

    #region buttons
    public void LoginButtonClicked()
    {
        string playerName = PlayerName.text;

        if(!playerName.Equals(""))
        {
            PhotonNetwork.LocalPlayer.NickName = playerName;
            PhotonNetwork.ConnectUsingSettings();
            UpdateLeaderboardUsername();
        }
        else
        {
            Debug.LogError("Player Name is invalid.");
        }
    }

    private void UpdateLeaderboardUsername()
    {
        var request = new UpdateUserTitleDisplayNameRequest()
        {
            DisplayName = PlayerUsername.text
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request,
            (result) => { Debug.Log("PlayFab - Score submitted!"); },
            (error) => { Debug.Log("PlayFab - Error occured while submitting the score: " + error.ErrorMessage); });
    }
   
    public void DisconnectButtonClicked()
    {
        PhotonNetwork.Disconnect();
    }

    public void CreateRoomClicked()
    {
        var roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;

        PhotonNetwork.CreateRoom(RoomName.text, roomOptions);
    }

    public void LeaveRoomClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void ListRoomsClicked()
    {
        PhotonNetwork.JoinLobby();
    }

    public void LeaveLobbyClicked()
    {
        PhotonNetwork.LeaveLobby();
    }

    public void JoinRandomRoomClicked()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void StartGameClicked()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        PhotonNetwork.LoadLevel("Multiplayer");
    }

    #endregion

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master server!");
        ActivatePanel(SelectionPanel.gameObject.name);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected!");
        ActivatePanel(LoginPanel.name);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room created!");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Room joined!");

        chat.Username = PhotonNetwork.LocalPlayer.NickName;
        var authenticationValues = new AuthenticationValues(chat.Username);
        chat.ChatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, "1.0", authenticationValues);

        ActivatePanel(InsideRoomPanel.name);
        StartGameButton.SetActive(PhotonNetwork.IsMasterClient);

        foreach (var player in PhotonNetwork.PlayerList)
        {
           var newPlayerRoomEntry = Instantiate(TextPrefab, InsideRoomPanelContent);
            newPlayerRoomEntry.GetComponent<Text>().text = player.NickName;
            newPlayerRoomEntry.name = player.NickName;
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Room creation failed!");
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Room left!");
        chat.ChatClient.Disconnect();
        DeleteChildren(InsideRoomPanelContent);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby!");
        ActivatePanel(ListRoomsPanel.name);
    }

    public override void OnLeftLobby()
    {
        Debug.Log("Lobby left!");
        DeleteChildren(ListRoomPanelContent);
        cachedRoomList.Clear();
        ActivatePanel(SelectionPanel.name);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("Room update: " + roomList.Count);

        DeleteChildren(ListRoomPanelContent);
        UpdateCacheRoomList(roomList);

        foreach (var room in cachedRoomList)
        {
            var newRoomEntry = Instantiate(RoomEntryPrefab, ListRoomPanelContent);
            var newRoomEntryScript = newRoomEntry.GetComponent<RoomEntry>();
            newRoomEntryScript.RoomName = room.Key;
            newRoomEntryScript.RoomText.text = $"[{room.Key}] - ({room.Value.PlayerCount} / {room.Value.MaxPlayers})";
            //[Room1] - (1/4)
            //[Room2] - (2/4)
        }
    }

    private void UpdateCacheRoomList(List<RoomInfo> roomList)
    {
        foreach (var room in roomList)
        {
            //delete from cache
            if (!room.IsOpen || !room.IsVisible || room.RemovedFromList)
            {
                cachedRoomList.Remove(room.Name);
            }
            else
                cachedRoomList[room.Name] = room; //Add or Update cache
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log("Player entered the room!");
        var newPlayerRoomEntry = Instantiate(TextPrefab, InsideRoomPanelContent);
        newPlayerRoomEntry.GetComponent<Text>().text = newPlayer.NickName;
        newPlayerRoomEntry.name = newPlayer.NickName;
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log("Player left the room!");
        foreach (Transform child in InsideRoomPanelContent)
        {
            if(child.name == otherPlayer.NickName)
            {
                Destroy(child.gameObject);
                break;
            }
        }
        
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("On join random failed: " + message);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("On join room failed: " + message);
    }

    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        StartGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

}
