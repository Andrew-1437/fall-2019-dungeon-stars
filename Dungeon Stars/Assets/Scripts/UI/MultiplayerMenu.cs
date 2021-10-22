using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class MultiplayerMenu : MonoBehaviourPunCallbacks
{ 
    [Header("Screens")]
    public GameObject createLobbyMenu;
    public GameObject lobbyMenu;

    [Header("Multiplayer Screen")]
    public Button createRoomButton;
    public Button joinRoomButton;

    [Header("Lobby Screen")]
    public TextMeshProUGUI lobbyID;
    public TextMeshProUGUI playerList;
    public Button startGameButton;

    // Start is called before the first frame update
    void Start()
    {
        createRoomButton.interactable = false;
        joinRoomButton.interactable = false;
    }

    // Update is called once per frame
    void Update()
    {
        createRoomButton.interactable = NetworkManager.instance.connectedToMaster;
        joinRoomButton.interactable = NetworkManager.instance.connectedToMaster;
    }

    public void CreateRoom(TMP_InputField roomName)
    {
        NetworkManager.instance.CreateRoom(roomName.text);
    }

    public void JoinRoom(TMP_InputField roomName)
    {
        NetworkManager.instance.JoinRoom(roomName.text);
    }

    public void UpdatePlayerName(string playerName)
    {
        PhotonNetwork.NickName = playerName;
    }

    public override void OnJoinedRoom()
    {
        print("Joined Lobby: " + PhotonNetwork.CurrentRoom.Name);

        createLobbyMenu.SetActive(false);
        lobbyMenu.SetActive(true);
        lobbyID.text = "Lobby: " + PhotonNetwork.CurrentRoom.Name;

        photonView.RPC("UpdatePlayerList", RpcTarget.All);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        print("Player left: " + otherPlayer.NickName);

        UpdatePlayerList();
    }

    [PunRPC]
    public void UpdatePlayerList()
    {
        playerList.text = "";

        foreach(Player player in PhotonNetwork.PlayerList)
        {
            playerList.text += player.NickName + "\n";
        }

        if (PhotonNetwork.IsMasterClient)
            startGameButton.interactable = true;
        else
            startGameButton.interactable = false;
    }

    public void LeaveLobby()
    {
        PhotonNetwork.LeaveRoom();
        lobbyMenu.SetActive(false);
        createLobbyMenu.SetActive(true);
    }

    public void StartGame()
    {
        OmniController.omniController.twoPlayerMode = true;
        NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, "ShipSelect");
    }
}
