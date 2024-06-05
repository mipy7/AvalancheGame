using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;

public class RoomController : MonoBehaviourPunCallbacks
{
	//Player instance prefab, must be located in the Resources folder
	public GameObject playerPrefab;
	//Player spawn point
	public Transform spawnPoint;

	private MultiplayerBehaviour multyBehaviour;

	// Start is called before the first frame update
	void Start()
    {
		multyBehaviour = GameObject.Find("Square").GetComponent<MultiplayerBehaviour>();

		//In case we started this demo with the wrong scene being active, simply load the menu scene
		if (PhotonNetwork.CurrentRoom == null)
		{
			Debug.Log("Is not in the room, returning back to Lobby");
			UnityEngine.SceneManagement.SceneManager.LoadScene("GameLobby");
			return;
		}

		//We're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
		PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, Quaternion.identity, 0);
	}

	void OnGUI()
	{
		if (PhotonNetwork.CurrentRoom == null)
			return;

		//Leave this Room
		if (GUI.Button(new Rect(5, 5, 125, 25), "Leave Room"))
		{
			PhotonNetwork.LeaveRoom();
		}

		//Show the Room name
		GUI.Label(new Rect(135, 5, 200, 25), PhotonNetwork.CurrentRoom.Name);

		//Show the list of the players connected to this Room
		for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
		{
			//Show if this player is a Master Client. There can only be one Master Client per Room so use this to define the authoritative logic etc.)
			string isMasterClient = (PhotonNetwork.PlayerList[i].IsMasterClient ? ": MasterClient" : "");
			GUI.Label(new Rect(5, 35 + 30 * i, 200, 25), PhotonNetwork.PlayerList[i].NickName + isMasterClient);
		}
	}

	public override void OnLeftRoom()
	{
		if (SceneManager.GetActiveScene().name == "FigureSelection")
			SceneManager.LoadScene("GameLobby");
		if (SceneManager.GetActiveScene().name == "GameLevel")
		{
			if (multyBehaviour.isPlayer1FinishGame && multyBehaviour.isPlayer2FinishGame)
			{
				SceneManager.LoadScene("GameOver");
			}
			else
			{
				//We have left the Room, return back to the GameLobby
				SceneManager.LoadScene("GameLobby");
			}
		}
	}

	public override void OnDisconnected(DisconnectCause cause)
	{
		SceneManager.LoadScene("Menu");
	}

	// Update is called once per frame
	void Update()
    {

	}

	public void InstatiatePlayerPref()
	{
		PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, Quaternion.identity, 0);
	}

}
