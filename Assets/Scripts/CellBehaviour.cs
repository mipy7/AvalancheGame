using UnityEngine.UI;
using UnityEngine;

public class CellBehaviour : MonoBehaviour
{
    public PlayerSync playerSync;
	private BoardBehaviour Board;

	private Image boardImg;

	public int x;
    public int y;

	[SerializeField]
	private bool isPosed = true;

	// Start is called before the first frame update
	void Start()
    {
		Board = GameObject.Find("mainBoard").GetComponent<BoardBehaviour>();
		boardImg = Board.gameObject.GetComponent<Image>();
	}

	private void OnMouseDown()
	{
		if ((FolderScript.GameType == 1 || FolderScript.GameType == 2) && FolderScript.Turn != (FolderScript.Figure - 1))
		{
			Debug.Log("Сейчас не твой ход");
		}
		else if (FolderScript.GameType == 2 && !Board.isClicked && FolderScript.Turn == (FolderScript.Figure - 1) && playerSync.isAcceptStep)
		{
			Board.isClicked = true;
			playerSync.isAcceptStep = false;
			Board.clickedX = x;
			Board.clickedY = y;
		}
		else if (FolderScript.GameType == 1 && !Board.isClicked && FolderScript.Turn == (FolderScript.Figure - 1))
		{
			Board.isClicked = true;
			Board.clickedX = x;
			Board.clickedY = y;
		}
		else if (FolderScript.GameType == 0 && !Board.isClicked)
		{
			Board.isClicked = true;
			Board.clickedX = x;
			Board.clickedY = y;
		}

	}

	// Update is called once per frame
	void Update()
    {
		if (!isPosed)
		{
			if (boardImg)
			{
				Debug.Log("Board Image");

				float summOffset = Board.borderOffset * 2 + Board.cellOffset * 5;
				// compute cell size
				float cellHeight = (boardImg.rectTransform.sizeDelta.y - summOffset) / 6;
				float cellWidth = cellHeight - boardImg.rectTransform.sizeDelta.y;
				GetComponent<Image>().rectTransform.sizeDelta = new Vector2(cellWidth, cellHeight);

				// compute cell position
				float posOffset = -boardImg.rectTransform.sizeDelta.y / 2 + GetComponent<Image>().rectTransform.sizeDelta.y / 2 + Board.borderOffset;
				Vector3 pos = new Vector3(posOffset + (GetComponent<Image>().rectTransform.sizeDelta.y + Board.cellOffset) * x, posOffset + (GetComponent<Image>().rectTransform.sizeDelta.y + Board.cellOffset) * y, transform.localPosition.z);
				transform.localPosition = pos;

				BoxCollider2D bc = GetComponent<BoxCollider2D>();
				bc.size = new Vector2(cellHeight, cellHeight);
			}

			if (!boardImg)
			{
				float borderOffset = Board.borderOffset / 1000f;
				float cellOffset = Board.cellOffset / 1000f;

				// compute cell size
				float summOffset = (borderOffset * 2 + cellOffset * 5);
				float cellWidth = (1f - summOffset) / 6f;
				float cellHeight = (1f - summOffset) / 6f;
				transform.localScale = new Vector2(cellHeight, cellWidth);

				// compute cell position
				float posOffset = -0.5f + transform.localScale.y / 2 + borderOffset;
				Vector3 pos = new Vector3(posOffset + (transform.localScale.y + cellOffset) * x, posOffset + (transform.localScale.y + cellOffset) * y, transform.localPosition.z);
				transform.localPosition = pos;
			}

			//isPosed = true;
		}
	}
}
