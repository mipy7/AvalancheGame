using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System;

public class GameLobby : MonoBehaviourPunCallbacks
{
	//Our player name
	public string playerName = "Player 1";
	//Users are separated from each other by gameversion (which allows you to make breaking changes).
	string gameVersion = "0.1";
	//The list of created rooms
	public List<RoomInfo> createdRooms = new List<RoomInfo>();
	//Use this name when creating a Room
	string roomName = "Room 1";
	Vector2 roomListScroll = Vector2.zero;
	public bool joiningRoom = false;

	public Text statusTxt;

	public InputField roomNameIF;
	public InputField nickNameIF;

	public GameObject roomContent;

	public GameObject roomElemPrefub;

	private List<GameObject> roomElements = new List<GameObject>();
	private int prevRoomCount = 0;

	private float firstPos = 132f;
	private float height = 36f;

	// Start is called before the first frame update
	void Start()
    {
		firstPos = 132f;
		height = 36f;
		//This makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
		PhotonNetwork.AutomaticallySyncScene = true;

		if (!PhotonNetwork.IsConnected)
		{
			Debug.Log("Photon Connection");
			//Set the App version before connecting
			PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = gameVersion;
			// Connect to the photon master-server. We use the settings saved in PhotonServerSettings (a .asset file in this project)
			PhotonNetwork.ConnectUsingSettings();
		}
		else
		{
			Debug.Log("Photon already connected");
		}
	}

	public override void OnDisconnected(DisconnectCause cause)
	{
		Debug.Log("OnFailedToConnectToPhoton. StatusCode: " + cause.ToString() + " ServerAddress: " + PhotonNetwork.ServerAddress);
	}

	public override void OnConnectedToMaster()
	{
		Debug.Log("OnConnectedToMaster");
		//After we connected to Master server, join the Lobby
		PhotonNetwork.JoinLobby(TypedLobby.Default);
	}

	public override void OnRoomListUpdate(List<RoomInfo> roomList)
	{
		Debug.Log("We have received the Room list");
		//After this callback, update the room list
		createdRooms = roomList;
	}

	#region GUI

	void OnGUI()
	{
		LobbyWindow(0);
	}

	void LobbyWindow(int index)
	{
		playerName = nickNameIF.text.ToString();

		if (joiningRoom || !PhotonNetwork.IsConnected || PhotonNetwork.NetworkClientState != ClientState.JoinedLobby)
		{
			GUI.enabled = false;
		}

		if (prevRoomCount != createdRooms.Count)
		{
			foreach (var elem in roomElements)
			{
				Destroy(elem);
			}

			if (createdRooms.Count == 0)
			{
				GUILayout.Label("No Rooms were created yet...");
			}
			else
			{
				for (int i = 0; i < createdRooms.Count; i++)
				{
					var room = Instantiate(roomElemPrefub, roomContent.transform);
					room.transform.localScale = Vector3.one;
					room.GetComponent<RoomElementScript>().roomInfo = createdRooms[i];
					roomElements.Add(room);
				}
			}

			prevRoomCount = createdRooms.Count;
		}

		GUI.enabled = (PhotonNetwork.NetworkClientState == ClientState.JoinedLobby || PhotonNetwork.NetworkClientState == ClientState.Disconnected || PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer) && !joiningRoom;

		if (joiningRoom)
		{
			GUI.enabled = true;
			statusTxt.text = "Connecting...";
		}
		else
		{
			statusTxt.text = "Status: " + PhotonNetwork.NetworkClientState.ToString();
		}
	}
	#endregion

	public override void OnCreateRoomFailed(short returnCode, string message)
	{
		Debug.Log("OnCreateRoomFailed got called. This can happen if the room exists (even if not visible). Try another room name.");
		joiningRoom = false;
	}

	public override void OnJoinRoomFailed(short returnCode, string message)
	{
		Debug.Log("OnJoinRoomFailed got called. This can happen if the room is not existing or full or closed.");
		joiningRoom = false;
	}

	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		Debug.Log("OnJoinRandomFailed got called. This can happen if the room is not existing or full or closed.");
		joiningRoom = false;
	}

	public override void OnCreatedRoom()
	{
		Debug.Log("OnCreatedRoom");
		//Set our player name
		PhotonNetwork.NickName = playerName;
		//Load the Scene called GameLevel (Make sure it's added to build settings)
		PhotonNetwork.LoadLevel("FigureSelection");
	}

	public override void OnJoinedRoom()
	{
		Debug.Log("OnJoinedRoom");
	}

	public override void OnLeftLobby()
	{
		PhotonNetwork.Disconnect();
		SceneManager.LoadScene("GameSelection");
	}

	// Update is called once per frame
	void Update()
    {
		if (Input.GetKey(KeyCode.Escape))
		{
			BackBtn();
		}
	}

	public void BackBtn()
	{
		if (PhotonNetwork.InLobby)
		{
			PhotonNetwork.LeaveLobby();
		}
		else
		{
			PhotonNetwork.Disconnect();
			SceneManager.LoadScene("GameSelection");
		}
	}

	public void CreateRoomBtn()
	{
		//Room name text field
		roomName = roomNameIF.text;

		if (roomName != "")
		{
			joiningRoom = true;

			RoomOptions roomOptions = new RoomOptions();
			roomOptions.IsOpen = true;
			roomOptions.IsVisible = true;
			roomOptions.MaxPlayers = (byte)2; //Set any number
			roomOptions.PublishUserId = true;

			PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
		}
	}

	public void refreshBtn() 
	{
		if (PhotonNetwork.IsConnected)
		{
			//Re-join Lobby to get the latest Room list
			PhotonNetwork.JoinLobby(TypedLobby.Default);
		}
		else
		{
			//We are not connected, estabilish a new connection
			PhotonNetwork.ConnectUsingSettings();
		}
	}
}
