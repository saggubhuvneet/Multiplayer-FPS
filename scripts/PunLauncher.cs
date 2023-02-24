using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;

public class PunLauncher : MonoBehaviourPunCallbacks  //----------------------------  call back for room creation, errors and lobbies; 
{
    public static PunLauncher Instance;

    [SerializeField] TMP_InputField roomNameInputFeild;
    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] Transform roomListContent;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject roomListPrefab;
    [SerializeField] GameObject playerListPrefab;
    [SerializeField] GameObject startGameButton;

    //public GameObject roomMenu;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Debug.Log("conneting to master");
        PhotonNetwork.ConnectUsingSettings(); //------------------------------------  connect with server;
    }

    //------------------------------------------------------------------------------  when succesfully connected to server
    public override void OnConnectedToMaster()
    {
        Debug.Log("connected to master");
        PhotonNetwork.JoinLobby();

        PhotonNetwork.AutomaticallySyncScene = true;
    }


    public override void OnJoinedLobby()
    {
        MenuManager.Instance.OpenMenu("tittle");
        Debug.Log("Joined Lobby");
       
    }


    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInputFeild.text))
            return;
        PhotonNetwork.CreateRoom(roomNameInputFeild.text);
        MenuManager.Instance.OpenMenu("loading");
    }


    public override void OnJoinedRoom()
    {
        MenuManager.Instance.OpenMenu("RoomMenu");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
 
        foreach(Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        Player[] players = PhotonNetwork.PlayerList;

        for (int i = 0; i < players.Count(); i++)
        {
            Instantiate(playerListPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        }

        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room Creation Failed: " + message;
        MenuManager.Instance.OpenMenu("error");
    }

    public void StartGameMenu()
    {
        PhotonNetwork.LoadLevel(1);  
    }


    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("loading");
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("loading");

        

    }

    public override void OnLeftRoom()
    {
        MenuManager.Instance.OpenMenu("tittle");
    }

    

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }

        for(int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
                continue;
            Instantiate(roomListPrefab, roomListContent).GetComponent<RoomListItem>().Setup(roomList[i]);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }
}
