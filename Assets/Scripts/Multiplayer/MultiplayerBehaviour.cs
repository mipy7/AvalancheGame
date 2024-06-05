using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerBehaviour : MonoBehaviour
{
    public GameObject multiplayer;

    //[HideInInspector]
    public bool isPlayer1Ready = false;
	//[HideInInspector]
	public bool isPlayer2Ready = false;

	//[HideInInspector]
	public bool isPlayer1FinishGame = false;
	//[HideInInspector]
	public bool isPlayer2FinishGame = false;

	// Start is called before the first frame update
	void Start()
    {
		if (FolderScript.GameType == 2)
        {
			multiplayer.SetActive(true);
		}
    }

    // Update is called once per frame
    void Update()
    {
                
    }
}
