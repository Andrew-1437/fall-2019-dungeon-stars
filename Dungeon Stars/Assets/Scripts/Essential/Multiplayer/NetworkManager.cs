using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    // Singleton Instance
    public static NetworkManager instance;

    // Events
    public delegate void NetworkDelegate();
    public static event NetworkDelegate OnConnectToMaster;

    [HideInInspector]
    public bool connectedToMaster = false;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        print("Connected to Master");
        connectedToMaster = true;
        OnConnectToMaster?.Invoke();
    }

    public void CreateRoom(string name)
    {
        PhotonNetwork.CreateRoom(name);
    }

    public void JoinRoom(string name)
    {
        PhotonNetwork.JoinRoom(name);
    }

    public void ChangeScene(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }
}
