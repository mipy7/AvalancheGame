using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
	public FolderScript Save;

	private void Start()
	{
		if(FolderScript.instance == null)
			FolderScript.instance = Save;

		FolderScript.Reset();
	}

	public void Play()
    {
        SceneManager.LoadScene("GameSelection");
    }

    public void Quite()
    {
        Application.Quit();
    }
}
