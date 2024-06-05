using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerSync : MonoBehaviourPun, IPunObservable
{

	//List of the scripts that should only be active for the local player (ex. PlayerController, MouseLook etc.)
	public MonoBehaviour[] localScripts;
	//List of the GameObjects that should only be active for the local player (ex. Camera, AudioListener etc.)
	public GameObject[] localObjects;

	private BoardBehaviour Board;
	//Values that will be synced over network
	public int remotePlayerX = -1, remotePlayerY = -1;
	public bool remotePlayerIsClicked;

	public Button crossBtn;
	public Button zeroBtn;

	private MultiplayerBehaviour multyBehaviour;
	private Text testText;

	//[HideInInspector]
	public int selection = 0;
	//[HideInInspector]
	public bool isAcceptStep = true;

	private RoomController roomController;

	private float lastTime;
	private float lastRPCSendTime;
	private string stepData;

	// Start is called before the first frame update
	void Start()
    {
		lastTime = Time.time;

		remotePlayerX = -1; 
		remotePlayerY = -1;
		isAcceptStep = true;
		roomController = GameObject.FindObjectOfType<RoomController>();

		multyBehaviour = GameObject.Find("Square").GetComponent<MultiplayerBehaviour>();

		if (SceneManager.GetActiveScene().name == "FigureSelection")
		{
			//testText = GameObject.Find("Test").GetComponent<Text>();

			crossBtn = GameObject.Find("Button (Cross)").GetComponent<Button>();
			zeroBtn = GameObject.Find("Button (Zero)").GetComponent<Button>();

			if(photonView.IsMine)
				GameObject.Find("Main Camera").GetComponent<FigureSelection>().player = this;
		}

		if (SceneManager.GetActiveScene().name == "GameLevel")
		{
			Board = GameObject.Find("mainBoard").GetComponent<BoardBehaviour>();
		}

		if (photonView.IsMine)
		{
			//Player is local
		}
		else
		{
			//Player is Remote, deactivate the scripts and object that should only be enabled for the local player
			for (int i = 0; i < localScripts.Length; i++)
			{
				localScripts[i].enabled = false;
			}
			for (int i = 0; i < localObjects.Length; i++)
			{
				localObjects[i].SetActive(false);
			}
		}

		if(photonView.IsMine) {
			var cellArr = GameObject.FindObjectsOfType<CellBehaviour>();
			foreach (CellBehaviour cell in cellArr)
			{
				cell.playerSync = this;
			}
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (SceneManager.GetActiveScene().name == "GameLevel")
		{
			if (stream.IsWriting)
			{
				Debug.Log(isAcceptStep);
				if (!isAcceptStep)
				{
					//We own this player: send the others our data
					stream.SendNext(Board.clickedX);
					stream.SendNext(Board.clickedY);
				}
			}
			else
			{
				//Network player, receive data
				int currentRemotePlayerX = (int)stream.ReceiveNext();
				int currentRemotePlayerY = (int)stream.ReceiveNext();
				Debug.Log("currentRemotePlayerX " + currentRemotePlayerX + " currentRemotePlayerY " + currentRemotePlayerY + " " + (currentRemotePlayerX != remotePlayerX || currentRemotePlayerY != remotePlayerY));
				if (currentRemotePlayerX != remotePlayerX || currentRemotePlayerY != remotePlayerY)
				{
					Board.clickedX = currentRemotePlayerX;
					Board.clickedY = currentRemotePlayerY;
					Board.isClicked = true;

					remotePlayerX = currentRemotePlayerX;
					remotePlayerY = currentRemotePlayerY;

					photonView.RPC("RPCAcceptStep", PhotonNetwork.PlayerListOthers[0], (Board.clickedX.ToString() + Board.clickedY.ToString()));

				}

				//if (Time.time - lastRPCSendTime < 2) {
				//	photonView.RPC("RPCAcceptStep", PhotonNetwork.PlayerListOthers[0], (Board.clickedX.ToString() + Board.clickedY.ToString()));
				//	lastRPCSendTime = Time.time;
				//}
			}
		}
	}

	// Update is called once per frame
	void Update()
    {
		// Figure Selection scene
		if (SceneManager.GetActiveScene().name == "FigureSelection")
		{
			if (PhotonNetwork.PlayerList.Length < 2)
			{
				crossBtn.transform.gameObject.SetActive(false);
				zeroBtn.transform.gameObject.SetActive(false);
			}
			else
			{
				crossBtn.transform.gameObject.SetActive(true);
				zeroBtn.transform.gameObject.SetActive(true);
			}

			if (!photonView.IsMine)
			{
				//Update remote player (smooth this, this looks good, at the cost of some accuracy)
				switch (selection)
				{
					case 1:
						crossBtn.interactable = false; break;
					case 2:
						zeroBtn.interactable = false; break;
				}
			}

			if (PhotonNetwork.LocalPlayer.IsMasterClient && photonView.IsMine)
			{
				if (multyBehaviour.isPlayer1Ready && multyBehaviour.isPlayer2Ready)
				{
					Debug.Log("Оба игрока готовы к игре");
					SceneManager.LoadScene("GameLevel");
				}
			}

			// make instatiate player prefub request
			if (GameObject.FindObjectsOfType<PlayerSync>().Length < PhotonNetwork.PlayerList.Length && ((Time.time - lastTime) > 10f))
			{
				if (photonView.IsMine)
				{
					photonView.RPC("RPCReconnectRequest", PhotonNetwork.PlayerListOthers[0]);
					lastTime = Time.time;
				}
			}
		}

		// Game Level scene
		if (SceneManager.GetActiveScene().name == "GameLevel")
		{
			if (!Board.isGame && photonView.IsMine && !multyBehaviour.isPlayer1FinishGame)
			{
				photonView.RPC("RPCSetFinishGame", RpcTarget.All);
			}

			if (photonView.IsMine)
			{
				if (multyBehaviour.isPlayer1FinishGame && multyBehaviour.isPlayer2FinishGame)
				{
					PhotonNetwork.LeaveRoom();
				}

				if (!isAcceptStep)
				{
					isAcceptStep = (Board.clickedX.ToString() + Board.clickedY.ToString()) == stepData;
				}
			}
		}


	}

	public void Cross()
	{
		photonView.RPC("RPCBlockButton", RpcTarget.All, 1);
	}

	public void Zero()
	{
		photonView.RPC("RPCBlockButton", RpcTarget.All, 2);
	}

	// TODO: Доделать кик игроков при выходе мастер-клиента
	public void Leave()
	{
		Debug.Log("LEAVE FROM ROOM !!!!!!!!!!!!!!!!!");
		if (PhotonNetwork.LocalPlayer.IsMasterClient && photonView.IsMine)
		{
			PhotonNetwork.CurrentRoom.IsOpen = false;
		}
		
		foreach(var player in PhotonNetwork.PlayerListOthers)
		{
			PhotonNetwork.CloseConnection(player);
		}
	}

	public void DebugTest()
	{
		testText.text = photonView.IsMine.ToString() + " " + PhotonNetwork.LocalPlayer.IsMasterClient + " " + GameObject.FindObjectsOfType<PlayerSync>().Length.ToString();
	}

	[PunRPC]
	public void RPCBlockButton(int select)
	{
		selection = select;
		if(PhotonNetwork.LocalPlayer.IsMasterClient && photonView.IsMine)
		{
			multyBehaviour.isPlayer1Ready = true;
		}
		else
		{
			multyBehaviour.isPlayer2Ready = true;
		}
	}

	[PunRPC]
	public void RPCSetFinishGame()
	{
		if (PhotonNetwork.LocalPlayer.IsMasterClient && photonView.IsMine)
		{
			multyBehaviour.isPlayer1FinishGame = true;
		}
		else
		{
			multyBehaviour.isPlayer2FinishGame = true;
		}
	}

	[PunRPC]
	public void RPCReconnectRequest(PhotonMessageInfo info)
	{
		// Figure Selection scene
		if (SceneManager.GetActiveScene().name == "FigureSelection")
		{
			if (!photonView.IsMine)
			{
				var PVArr = FindObjectsOfType<PlayerSync>();
				var otherPV = (PVArr[0] == photonView) ? PVArr[0] : PVArr[1];
				Debug.Log("RPC Reconnect Request");
				roomController.InstatiatePlayerPref();
				PhotonNetwork.Destroy(otherPV.gameObject);
			}
		}
	}

	[PunRPC]
	public void RPCAcceptStep(string data, PhotonMessageInfo info)
	{
		// Figure Selection scene
		if (SceneManager.GetActiveScene().name == "GameLevel")
		{
			Debug.Log(photonView.IsMine + " RPCAcceptStep " + data);
			if (photonView.IsMine)
			{
				stepData = data;
				//Debug.Log("RPCAcceptStep " + data);
			}
		}
	}
}
