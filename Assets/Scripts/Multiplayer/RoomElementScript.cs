using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomElementScript : MonoBehaviour
{
    public Text roomName;
    public Text playersInRoom;
	public RoomInfo roomInfo;

    private GameLobby _gameLobby;

    // Start is called before the first frame update
    void Start()
    {
		_gameLobby = GameObject.FindObjectOfType<GameLobby>();
		roomName.text = roomInfo.Name;
	}

	// Update is called once per frame
	void Update()
    {
		playersInRoom.text = roomInfo.PlayerCount + "/" + roomInfo.MaxPlayers;
	}

	public void Join()
	{
		_gameLobby.joiningRoom = true;

		//Set our Player name
		PhotonNetwork.NickName = _gameLobby.playerName;

		//Join the Room
		PhotonNetwork.JoinRoom(roomName.text);
	}
}
