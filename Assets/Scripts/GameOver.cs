using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{    
    private GameObject textObject;
    Text GameOverText;
    
    // Start is called before the first frame update
    void Start()
    {        
        // Text initialization
        textObject = GameObject.Find("Text (GameOver)");
        GameOverText = textObject.GetComponent<Text>();
        GameOverText.text = "Game Over";
        
        // board
        if(FolderScript.GameType == 0)
        {
            if (FolderScript.Turn == 1)
                GameOverText.text = "Победил первый игрок";
            else
                GameOverText.text = "Победил второй игрок";
        }

        if (FolderScript.GameType == 1 || FolderScript.GameType == 2)
        {
            if (FolderScript.Turn != (FolderScript.Figure - 1))
                GameOverText.text = "Вы победили";
            else
                GameOverText.text = "Вы проиграли";
        }
    }

    public void Menu()
    {
        SceneManager.LoadScene("Menu");
    }
}
