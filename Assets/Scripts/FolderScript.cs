using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FolderData", menuName = "Folder Data", order = 51)]
public class FolderScript : ScriptableObject
{
	[SerializeField] private int gameType;
	[SerializeField] private int figure;
	[SerializeField] private int turn;

	public static int GameType {
		get { return instance.gameType; }
		set { instance.gameType = value; } 
	}
	public static int Figure
	{
		get { return instance.figure; }
		set { instance.figure = value; }
	}
	public static int Turn
	{
		get { return instance.turn; }
		set { instance.turn = value; }
	}

	public static FolderScript instance;

	private void OnEnable()
	{
		instance = this;
	}

	public static void Reset() 
	{
		instance.gameType = 0;
		instance.figure = 0;
		instance.turn = 0;
	}
}
