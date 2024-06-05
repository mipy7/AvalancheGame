using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FigureSelection : MonoBehaviour
{
    public Button crossBtn;
    public Button zeroBtn;

	[HideInInspector]
	public PlayerSync player;

	public void Cross()
    {
		FolderScript.Figure = 1;
		if (FolderScript.GameType == 2)
        {
			player.Cross();
			crossBtn.interactable = false;
			zeroBtn.interactable = false;
		}
        else
        {
			SceneManager.LoadScene("GameLevel");
		}
    }

    public void Zero()
    {
		FolderScript.Figure = 2;
		if (FolderScript.GameType == 2)
		{
			player.Zero();
			crossBtn.interactable = false;
			zeroBtn.interactable = false;
		}
		else
		{
			SceneManager.LoadScene("GameLevel");
		}
	}

    public void Back()
    {
		if (FolderScript.GameType == 2)
		{
			player.Leave();
			PhotonNetwork.LeaveRoom();
		}
		else
		{
			SceneManager.LoadScene("GameSelection");
		}
    }
    
    public void Menu()
    {
		if (FolderScript.GameType == 2)
		{
			PhotonNetwork.Disconnect();
		}
		else 
		{ 
			SceneManager.LoadScene("Menu"); 
		}
    }

	public void Test()
	{
		player.DebugTest();
	}
}
