using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameSelectionScript : MonoBehaviour
{
	private void Start()
	{
		FolderScript.Reset();
	}

	public void Singleplayer()
    {
		FolderScript.GameType = 1;
        SceneManager.LoadScene("FigureSelection");
    }
    
    public void OfflineMultiplayer()
    {
		FolderScript.GameType = 0;
        SceneManager.LoadScene("GameLevel");
    }
    
    public void OnlineMultiplayer()
    {
        FolderScript.GameType = 2;
		SceneManager.LoadScene("GameLobby");
	}

	public void Menu()
    {
		FolderScript.GameType = 0;
		SceneManager.LoadScene("Menu");
    }
}
