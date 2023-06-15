using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;


public class GameplayManager : MonoBehaviour
{
    public ScoreManager scoreManager;
    public GameObject tilePrefab;

    public enum BoardSize { Tiny, Classic, Big, Bigger, Huge };
    public BoardSize boardSize;

    private int rowCount;
    private int colCount;
    public Tile[,] tiles;

    private int[,] tilesPrevValues;
    private int prevScore;

    private string savedTileValuesString = "";


    public List<Tile> movingTiles = new List<Tile>();

    private bool gameOver;

    private Vector3 firstTouchPos;
    private Vector3 lastTouchPos;
    private float swipeDistRequirement; 


    // Start is called before the first frame update
    void Start()
    {
        //PlayerPrefs.DeleteAll();

        boardSize = (BoardSize) MenuManager.boardSizeSelection;

        switch (boardSize)
        {
            case BoardSize.Tiny:
                rowCount = colCount = 3;
                break;

            case BoardSize.Classic:
                rowCount = colCount = 4;
                break;

            case BoardSize.Big:
                rowCount = colCount = 5;
                break;

            case BoardSize.Bigger:
                rowCount = colCount = 6;
                break;

            case BoardSize.Huge:
                rowCount = colCount = 7;
                break;
        }

        GridSetup();

        string loadedGameState = PlayerPrefs.GetString("savedTileValuesStringBoard" + rowCount.ToString());

        if (!string.IsNullOrEmpty(loadedGameState))
        {
            LoadGameState(loadedGameState);
        }
        else
        {
            ResetBoard();
        }

        swipeDistRequirement = Screen.width * 0.1f;
    }

    // Update is called once per frame
    void Update()
    {


        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetBoard();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("2048 Menu", LoadSceneMode.Single);
        }

        if (movingTiles.Count != 0)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveMade(KeyCode.UpArrow);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveMade(KeyCode.DownArrow);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveMade(KeyCode.RightArrow);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveMade(KeyCode.LeftArrow);
        }



        if (Input.touchCount == 1) // user is touching the screen with a single touch
        {
            Touch touch = Input.GetTouch(0); // get the touch
            if (touch.phase == TouchPhase.Began) //check for the first touch
            {
                firstTouchPos = lastTouchPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved) // update the last position based on where they moved
            {
                lastTouchPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended) //check if the finger is removed from the screen
            {
                lastTouchPos = touch.position;  //last touch position. Ommitted if you use list

                //Check if drag distance is greater than 20% of the screen height
                if (Mathf.Abs(lastTouchPos.x - firstTouchPos.x) > swipeDistRequirement || Mathf.Abs(lastTouchPos.y - firstTouchPos.y) > swipeDistRequirement)
                {//It's a drag
                 //check if the drag is vertical or horizontal
                    if (Mathf.Abs(lastTouchPos.x - firstTouchPos.x) > Mathf.Abs(lastTouchPos.y - firstTouchPos.y))
                    {   //If the horizontal movement is greater than the vertical movement...
                        if ((lastTouchPos.x > firstTouchPos.x))  //If the movement was to the right)
                        {   //Right swipe
                            MoveMade(KeyCode.RightArrow);
                        }
                        else
                        {   //Left swipe
                            MoveMade(KeyCode.LeftArrow);
                        }
                    }
                    else
                    {   //the vertical movement is greater than the horizontal movement
                        if (lastTouchPos.y > firstTouchPos.y)  //If the movement was up
                        {   //Up swipe
                            MoveMade(KeyCode.UpArrow);
                        }
                        else
                        {   //Down swipe
                            MoveMade(KeyCode.DownArrow);
                        }
                    }
                }
            }
        }
    }

    public void LoadGameState(string loadedGameState)
    {
        string[] gameStateRows = loadedGameState.Split('n');

        for (int i = 0; i < gameStateRows.Length - 1; i++)
        {
            string[] gameStateCols = gameStateRows[i].Split('-');

            for (int j = 0; j < gameStateCols.Length - 1; j++)
            {
                tiles[i, j].AssignValue(int.Parse(gameStateCols[j]));
            }
        }
    }

    private void OnApplicationQuit()
    {
        SaveGameState();
    }

    private void OnDestroy()
    {
        SaveGameState();
    }

    public void SaveGameState()
    {
        savedTileValuesString = "";

        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < colCount; col++)
            {
                savedTileValuesString += tiles[row, col].value.ToString() + "-";
            }
            savedTileValuesString += "n";
        }

        PlayerPrefs.SetString("savedTileValuesStringBoard" + rowCount.ToString(), savedTileValuesString);
        PlayerPrefs.Save();
    }

    public void GridSetup()
    {
        tiles = new Tile[rowCount, colCount];
        tilesPrevValues = new int[rowCount, colCount];

        RectTransform parentRect = gameObject.GetComponent<RectTransform>();
        GridLayoutGroup gridLayout = gameObject.GetComponent<GridLayoutGroup>();

        gridLayout.cellSize = new Vector2(parentRect.rect.width / colCount, parentRect.rect.height / rowCount);

        for (int i = 0; i < rowCount; i++)
        {
            for (int j = 0; j < colCount; j++)
            {
                GameObject tileObj = Instantiate(tilePrefab);
                tileObj.transform.SetParent(gameObject.transform, false);

                Tile tile = tileObj.GetComponent<Tile>();

                tiles[i, j] = tile;
            }
        }
    }

    public void ResetBoard()
    {
        foreach (Tile tile in tiles)
        {
            tile.AssignValue(0);
        }

        gameOver = false;

        GenerateNewTile();
        GenerateNewTile();

        BroadcastMessage("ResetMerged");
        BroadcastMessage("UpdateVisual");

        scoreManager.ResetScore();


        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < colCount; col++)
            {
                tilesPrevValues[row, col] = tiles[row, col].value;
            }
        }

        prevScore = scoreManager.GetScore();
    }

    public void ShiftValueUp(int row, int col, int val)
    {
        if (val == 0)
        {
            return;
        }

        for (int entry = row - 1; entry >= 0; entry--)
        {
            if (tiles[entry, col].value == val)
            {
                if (tiles[entry, col].merged)
                {
                    tiles[row, col].MoveTile(tiles[entry + 1, col], val);
                }
                else
                {
                    tiles[row, col].MoveTile(tiles[entry, col], val);
                }
                break;
            }
            else if (tiles[entry, col].value == 0)
            {
                if (entry == 0)
                {
                    tiles[row, col].MoveTile(tiles[entry, col], val);
                    break;
                }
                else
                {
                    continue;
                }
            }
            else
            {
                tiles[row, col].MoveTile(tiles[entry + 1, col], val);
                break;
            }
        }

    }

    public void ShiftValueDown(int row, int col, int val)
    {
        if (val == 0)
        {
            return;
        }

        for (int entry = row + 1; entry <= rowCount - 1; entry++)
        {
            if (tiles[entry, col].value == val)
            {
                if (tiles[entry, col].merged)
                {
                    tiles[row, col].MoveTile(tiles[entry - 1, col], val);
                }
                else
                {
                    tiles[row, col].MoveTile(tiles[entry, col], val);
                }
                break;
            }
            else if (tiles[entry, col].value == 0)
            {
                if (entry == rowCount - 1)
                {
                    tiles[row, col].MoveTile(tiles[entry, col], val);
                    break;
                }
                else
                {
                    continue;
                }
            }
            else
            {
                tiles[row, col].MoveTile(tiles[entry - 1, col], val);
                break;
            }
        }

    }

    public void ShiftValueLeft(int row, int col, int val)
    {
        if (val == 0)
        {
            return;
        }

        for (int entry = col - 1; entry >= 0; entry--)
        {
            if (tiles[row, entry].value == val)
            {
                if (tiles[row, entry].merged)
                {
                    tiles[row, col].MoveTile(tiles[row, entry + 1], val);
                }
                else
                {
                    tiles[row, col].MoveTile(tiles[row, entry], val);
                }
                break;
            }
            else if (tiles[row, entry].value == 0)
            {
                if (entry == 0)
                {
                    tiles[row, col].MoveTile(tiles[row, entry], val);
                    break;
                }
                else
                {
                    continue;
                }
            }
            else
            {
                tiles[row, col].MoveTile(tiles[row, entry + 1], val);
                break;
            }
        }

    }

    public void ShiftValueRight(int row, int col, int val)
    {
        if (val == 0)
        {
            return;
        }

        for (int entry = col + 1; entry <= colCount - 1; entry++)
        {
            if (tiles[row, entry].value == val)
            {
                if (tiles[row, entry].merged)
                {
                    tiles[row, col].MoveTile(tiles[row, entry - 1], val);
                }
                else
                {
                    tiles[row, col].MoveTile(tiles[row, entry], val);
                }
                break;
            }
            else if (tiles[row, entry].value == 0)
            {
                if (entry == colCount - 1)
                {
                    tiles[row, col].MoveTile(tiles[row, entry], val);
                    break;
                }
                else
                {
                    continue;
                }
            }
            else
            {
                tiles[row, col].MoveTile(tiles[row, entry - 1], val);
                break;
            }
        }

    }

    public void MoveMade(KeyCode key)
    {
        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < colCount; col++)
            {
                tilesPrevValues[row, col] = tiles[row, col].value;
            }
        }
        prevScore = scoreManager.GetScore();


        switch (key)
        {
            case KeyCode.UpArrow:
                for (int row = 0; row < rowCount; row++)
                {
                    for (int col = 0; col < colCount; col++)
                    {
                        ShiftValueUp(row, col, tiles[row, col].value);
                    }
                }
                break;

            case KeyCode.DownArrow:
                for (int row = rowCount - 1; row >= 0; row--)
                {
                    for (int col = 0; col < colCount; col++)
                    {
                        ShiftValueDown(row, col, tiles[row, col].value);
                    }
                }
                break;

            case KeyCode.LeftArrow:
                for (int row = 0; row < rowCount; row++)
                {
                    for (int col = 0; col < colCount; col++)
                    {
                        ShiftValueLeft(row, col, tiles[row, col].value);
                    }
                }
                break;

            case KeyCode.RightArrow:
                for (int row = 0; row < rowCount; row++)
                {
                    for (int col = colCount - 1; col >= 0; col--)
                    {
                        ShiftValueRight(row, col, tiles[row, col].value);
                    }
                }
                break;
        }
    }

    public void AddMovingTile(Tile movingTile)
    {
        movingTiles.Add(movingTile);
    }
    public void RemoveMovingTile(Tile movingTile)
    {
        movingTiles.Remove(movingTile);
        if (movingTiles.Count == 0)
        {
            MoveFinished();
        }
    }

    public void MoveFinished()
    {
        GenerateNewTile();
        BroadcastMessage("ResetMerged");
        BroadcastMessage("UpdateVisual");
    }

    public void GenerateNewTile()
    {
        while (!gameOver)
        {
            int randomTileNum = Random.Range(0, rowCount * colCount);
            int randomTileRow = randomTileNum / colCount;
            int randomTileCol = randomTileNum % colCount;

            Tile randomTile = tiles[randomTileRow, randomTileCol];

            if (randomTile.value == 0)
            {
                int valToAssign = Choose2or4();
                randomTile.AssignValue(valToAssign);

                gameOver = CheckForGameOver(randomTileRow, randomTileCol);
                break;
            }

        }
    }

    public void UndoMove()
    {

        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < colCount; col++)
            {
                tiles[row, col].AssignValue(tilesPrevValues[row, col]);
            }
        }

        scoreManager.SetScore(prevScore);
    }

    public bool CheckForGameOver(int newTileRow, int newTileCol)
    {
        gameOver = true;

        Tile curTile = tiles[newTileRow, newTileCol];
        List<Tile> neighbourTiles = GetNeighbourTiles(newTileRow, newTileCol);

        foreach (Tile neighbourTile in neighbourTiles)
        {
            if (neighbourTile.value == 0 || neighbourTile.value == curTile.value)
            {
                gameOver = false;
                return false;
            }
        }

        for (int row = rowCount - 1; row >= 0; row--)
        {
            for (int col = 0; col < colCount; col++)
            {
                curTile = tiles[row, col];
                neighbourTiles = GetNeighbourTiles(row, col);

                foreach (Tile neighbourTile in neighbourTiles)
                {
                    if (neighbourTile.value == 0 || neighbourTile.value == curTile.value)
                    {
                        gameOver = false;
                        return false;
                    }
                }
            }
        }

        Debug.Log("Game Over!");

        return true;
    }

    public List<Tile> GetNeighbourTiles(int row, int col)
    {
        List<Tile> neighbours = new List<Tile>();

        if (row > 0)
        {
            neighbours.Add(tiles[row - 1, col]);
        }
        if (row < rowCount - 1)
        {
            neighbours.Add(tiles[row + 1, col]);
        }
        if (col > 0)
        {
            neighbours.Add(tiles[row, col - 1]);
        }
        if (col < colCount - 1)
        {
            neighbours.Add(tiles[row, col + 1]);
        }

        return neighbours;
    }

    public int Choose2or4()
    {
        return Random.value <= 0.1 ? 4 : 2; 
    }

    public bool CompareLists(List<int> list1, List<int> list2)
    {
        if (list1.Count != list2.Count)
        {
            return false;
        }

        for (int i = 0; i < list1.Count; i++)
        {
            if (list1[i] != list2[i])
                return false;
        }
        return true;
    }

}
