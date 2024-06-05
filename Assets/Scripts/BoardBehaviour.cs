using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
//using UnityEngine.Networking;

public class BoardBehaviour : MonoBehaviourPunCallbacks
{
    public GameObject crossPrefub;
    public GameObject zeroPrefub;

    public GameObject cellPrefub;
    private GameObject[,] cells = new GameObject[6, 6]; 

    private int[,] crosses = new int[36, 2];
    private int[,] zeros = new int[36, 2];

    private int nCrosses = 0;
    private int nZeros = 0;

    private int[,] board = new int[6, 6];

    private int x, y;
    public int turn = 0;
    private int stepNum = 0;

    private const int X = 1;
    private const int O = 2;

    private Queue<int> visit = new Queue<int>();

    public bool isClicked = false;
    public bool isGame = true; 
    public int clickedX;
    public int clickedY;

    [Header("Offsets")]
    public float borderOffset = 100f;
	public float cellOffset = 50f;

	private void fillDirect(ref int[,] shapes, int elem, ref int nShape, int shape, int xDirect, int yDirect)
    {
        var x = shapes[elem, 0];
        var y = shapes[elem, 1];

        do
        {
            x = x + xDirect;
            y = y + yDirect;
        } while (x < 6 && x >= 0 && y < 6 && y >= 0 && board[x, y] == 0);

        if (x < 6 && x >= 0 && y < 6 && y >= 0)
            if (board[x, y] == shape)
            {
                while (board[(x - xDirect), (y - yDirect)] != shape)
                {
                    x = x - xDirect;
                    y = y - yDirect;
                    board[x, y] = shape;
                    if (shape == X)
                        Instantiate(crossPrefub, cells[x, y].transform);
                    if (shape == O)
						Instantiate(zeroPrefub, cells[x, y].transform);
					shapes[nShape, 0] = x;
                    shapes[nShape, 1] = y;
                    visit.Enqueue(nShape);
                    ++nShape;
                }
            }
    }

    private int fillBoard(int shape)
    {
        var i = 0;
        if (shape == X)
        {
            while (crosses[i, 0] != -1 && i < 36)
            {
                visit.Enqueue(i);
                ++i;
            }
            
            while (visit.Count() > 0)
            {
                var elem = visit.Dequeue();

                // ray tracing
                fillDirect(ref crosses, elem, ref nCrosses, shape, -1, -1); // ????? ?????
                fillDirect(ref crosses, elem, ref nCrosses, shape, 0, -1); // ?????
                fillDirect(ref crosses, elem, ref nCrosses, shape, 1, -1); // ?????? ?????
                fillDirect(ref crosses, elem, ref nCrosses, shape, 1, 0); // ??????
                fillDirect(ref crosses, elem, ref nCrosses, shape, 1, 1); // ?????? ????
                fillDirect(ref crosses, elem, ref nCrosses, shape, 0, 1); // ????
                fillDirect(ref crosses, elem, ref nCrosses, shape, -1, 1); // ????? ????
                fillDirect(ref crosses, elem, ref nCrosses, shape, -1, 0); // ?????
            }
        }

        else if (shape == O)
        {
            while (zeros[i, 0] != -1 && i < 36)
            {
                visit.Enqueue(i);
                ++i;
            }

            while (visit.Count() > 0)
            {
                var elem = visit.Dequeue();

                // ray tracing
                fillDirect(ref zeros, elem, ref nZeros, shape, -1, -1); // ????? ?????
                fillDirect(ref zeros, elem, ref nZeros, shape, 0, -1); // ?????
                fillDirect(ref zeros, elem, ref nZeros, shape, 1, -1); // ?????? ?????
                fillDirect(ref zeros, elem, ref nZeros, shape, 1, 0); // ??????
                fillDirect(ref zeros, elem, ref nZeros, shape, 1, 1); // ?????? ????
                fillDirect(ref zeros, elem, ref nZeros, shape, 0, 1); // ????
                fillDirect(ref zeros, elem, ref nZeros, shape, -1, 1); // ????? ????
                fillDirect(ref zeros, elem, ref nZeros, shape, -1, 0); // ?????
            }
        }
        return 0;
    }

    private bool makeStep(int stepX, int stepY, int shape)
    {
        if (board[stepX, stepY] != 0) return false;
        board[stepX, stepY] = shape;
        if(shape == 1)
			Instantiate(crossPrefub, cells[stepX, stepY].transform);
		if (shape == 2)
			Instantiate(zeroPrefub, cells[stepX, stepY].transform);
        return true;
    }

    private void Step(int stepX, int stepY)
    {
        isGame = false;
        bool goodStep = false;
        if (turn == 0)
        {
            Debug.Log("Ход первого игрока.");
            if (stepNum == 0)
            {
                //Debug.Log("Выберите первую клетку.");
            }
            else
            {
                //Debug.Log("Выберите вторую клетку.");
            }
            goodStep = makeStep(stepX, stepY, X);
            if (goodStep)
            {
                crosses[nCrosses, 0] = stepX;
                crosses[nCrosses, 1] = stepY;
                nCrosses++;

                fillBoard(X);
                stepNum++;
            }
            else
            {
                //Debug.Log("Клетка занята.");
            }
            
        }
        else
        {
            Debug.Log("Ход второго игрока.");
            if (stepNum == 0)
            {
                //Debug.Log("Выберите первую клетку.");
            }
            else
            {
                //Debug.Log("Выберите вторую клетку.");
            }
            goodStep = makeStep(stepX, stepY, O);
            if (goodStep)
            {
                zeros[nZeros, 0] = stepX;
                zeros[nZeros, 1] = stepY;
                nZeros++;

                fillBoard(O);
                stepNum++;
            }
            else
            {
                //Debug.Log("Клетка занята.");
            }
        }

        if (nCrosses + nZeros < 36)
        {
            if (stepNum >= 2)
            {
                stepNum = 0;
                turn += 1;
                if (turn == 2)
                    turn = 0;
				FolderScript.Turn = turn;
            }
        }

        isGame = true;
    }

    // Start is called before the first frame update
    void Start()
    {
		// Gameboard initialization
		for (var j = 0; j < 6; ++j)
            for (var i = 0; i < 6; ++i)
            {              
                GameObject cell = Instantiate(cellPrefub, transform);

				cell.name = "cell" + i + "," + j;

				cells[i, j] = cell;

				CellBehaviour cellScript = cells[i, j].GetComponent<CellBehaviour>();

				cellScript.x = i;
				cellScript.y = j;
            }

        // Crosses and zeroes initialization
        for (var i = 0; i < 36; ++i)
        {
            crosses[i, 0] = -1;
            crosses[i, 1] = -1;
            zeros[i, 0] = -1;
            zeros[i, 1] = -1;
        }

        // Inner board initialization
        for (var i = 0; i < 6; ++i)
        {
            for (var j = 0; j < 6; ++j)
                board[i, j] = 0;
        }

		FolderScript.Turn = 0;
    }
    float delay = 0.5f;
    float timer;
    // Update is called once per frame
    void Update()
    {
		if (isGame)
        {
            
            // Bot Step
            if (FolderScript.GameType == 1 && turn != (FolderScript.Figure - 1))
            {
                if (nCrosses + nZeros < 36)
                {

                    timer += Time.deltaTime;
                    if (timer > delay)
                    {
                        var botX = 0;
                        var botY = 0;
                        do
                        {
                            botX = Random.Range(0, 6);
                            botY = Random.Range(0, 6);
                        } while (board[botX, botY] != 0);

                        Debug.Log("botx = " + botX + " ,boty = " + botY);
                        Step(botX, botY);
                        timer = 0;
                    }
                    
                }
            }

            // Player Step
            if (isClicked)
            {
                if (nCrosses + nZeros < 36)
                {
                    Step(clickedX, clickedY);
                }
                isClicked = false;
            }

			// Check end of game
			if (nCrosses + nZeros == 36)
            {
				FolderScript.Turn = turn;
                if(FolderScript.GameType != 2)
                    SceneManager.LoadScene("GameOver");
				isGame = false;
            }
        }
    }
}
