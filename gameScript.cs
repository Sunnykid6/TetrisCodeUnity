using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class gameScript : MonoBehaviour
{
    public static int gridWidth = 10;
    public static int gridHeight = 20;
    public static Transform[,] grid = new Transform[gridWidth, gridHeight];
    public int LinesDeleted = 0;
    public bool isFirstPiece = false;
    public bool isDelete = false;

    private GameObject previewTetrimino;
    private GameObject nextTetrimino;
    private GameObject savedTetrimino;
    private GameObject ghostTetrimino;
    
    private bool gameStarted = false;
    private Vector2 previewTetriminoPosition = new Vector2(14f, 16);
    private Vector2 savedTetriminoPosition = new Vector2(-4.5f, 16.6f);

    public int maxSwaps = 1;
    private int countSwaps = 0;
    // Start is called before the first frame update
    void Start()
    {
        spawnNextTetrimino();
    }

    // Update is called once per frame
    void Update()
    {

    }

    bool checkIsValidPosition(GameObject objectTest)
    {
        foreach(Transform obj in objectTest.transform){
            Vector2 pos = Round(obj.position);

            if(!checkInsideGrid(pos))
            {
                return false;
            }
            if(getTransformAtGridPosition(pos) != null && getTransformAtGridPosition(pos).parent != objectTest.transform)
            {
                return false;
            }
        }

        return true;
    }

    public void UpdateGrid(Tetrimino tetrimino)
    {
        for(int y = 0; y < gridHeight; ++y)
        {
            for(int x = 0; x < gridWidth; ++x)
            {
                if(grid[x, y] != null)
                {
                    if(grid[x,y].parent == tetrimino.transform)
                    {
                        grid[x, y] = null;
                    }
                }
            }
        }

        foreach(Transform piece in tetrimino.transform)
        {
            Vector2 pos = Round(piece.position);
            if(pos.y < gridHeight)
            {
                grid[(int)pos.x, (int)pos.y] = piece;
            }
        }
    }

    public void DeleteRow()
    {
        int temp = 0;
        for (int y = 0; y < gridHeight; ++y)
        {
            if (isFull(y))
            {
                temp += 1;
                DeleteBlocks(y);
                moveRowDown(y + 1);
                --y;
                isDelete = true;
            }
        }
        LinesDeleted = temp;
        FindObjectOfType<Tetrimino>().callMoveColliders();
    }

    public void moveCollidersdown()
    {
        GameObject[] objectlist = GameObject.FindGameObjectsWithTag("tetriminos");
        foreach(GameObject obj in objectlist)
        {
            obj.GetComponent<Tetrimino>().moveCollidersdown(obj);
        }
    }

    public bool isFull(int y)
    {
        for(int x = 0; x < gridWidth; ++x)
        {
            if(grid[x,y] == null)
            {
                return false;
            }
        }
        return true;
    }

    public void DeleteBlocks(int y)
    {
        for (int x = 0; x < gridWidth; ++x)
        {
            Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
        }
    }

    public void blockDown(int y)
    {
        for(int x =  0; x < gridWidth; ++x)
        {
            if(grid[x,y] != null)
            {
                grid[x, y - 1] = grid[x, y];
                grid[x, y] = null;
                grid[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }
    }

    public void moveRowDown(int y)
    {
        for (int i = y; i < gridHeight; ++i)
        {
            blockDown(i);
        }
    }

    public Transform getTransformAtGridPosition(Vector2 pos)
    {
        if(pos.y > gridHeight - 1)
        {
            return null;
        }
        else
        {
            return grid[(int)pos.x, (int)pos.y];
        }
    }

    public void spawnNextTetrimino()
    {
        if (!gameStarted)
        {
            gameStarted = true;
            nextTetrimino = (GameObject)Instantiate(Resources.Load(GetRandomTetrimino(), typeof(GameObject)), new Vector2(5.0f, 19.0f), Quaternion.identity);
            if (nextTetrimino.name == "I piece(Clone)")
            {
                nextTetrimino.transform.position += new Vector3(0.5f, 0.5f, 0f);
            }
            nextTetrimino.tag = "active";
            isFirstPiece = true;
            previewTetrimino = (GameObject)Instantiate(Resources.Load(GetRandomTetrimino(), typeof(GameObject)), previewTetriminoPosition, Quaternion.identity);
            previewTetrimino.GetComponent<Tetrimino>().enabled = false;
            previewTetrimino.GetComponent<moveColliders>().enabled = false;
            previewTetrimino.tag = "preview";
            spawnGhost();
        }
        else
        {
            previewTetrimino.transform.position = new Vector2(5.0f, 19.0f);
            if (previewTetrimino.name == "I piece(Clone)")
            {
                previewTetrimino.transform.position += new Vector3(0.5f, 0.5f, 0f);
            }
            nextTetrimino = previewTetrimino;
            nextTetrimino.GetComponent<Tetrimino>().enabled = true;
            nextTetrimino.GetComponent<moveColliders>().enabled = true;
            nextTetrimino.tag = "active";
            previewTetrimino = (GameObject)Instantiate(Resources.Load(GetRandomTetrimino(), typeof(GameObject)), previewTetriminoPosition, Quaternion.identity);
            if (previewTetrimino.name == "I piece(Clone)")
            {
                previewTetrimino.transform.position += new Vector3(0.5f, 0.5f, 0f);
            }
            if(previewTetrimino.name == "Block piece(Clone)")
            {
                previewTetrimino.transform.position += new Vector3(-1f, 0f, 0f);
            }
            previewTetrimino.GetComponent<Tetrimino>().enabled = false;
            previewTetrimino.GetComponent<moveColliders>().enabled = false;
            previewTetrimino.tag = "preview";
            spawnGhost();
        }
        countSwaps = 0;
    }

    public void spawnGhost()
    {
        if (GameObject.FindGameObjectWithTag("ghost") != null)
        {
            Destroy(GameObject.FindGameObjectWithTag("ghost"));
        }
        ghostTetrimino = (GameObject)Instantiate(nextTetrimino, nextTetrimino.transform.position, Quaternion.identity);
        Destroy(ghostTetrimino.GetComponent<Tetrimino>());
        ghostTetrimino.AddComponent<ghostPiece>();
    }

    public void saveTetrimino(Transform t)
    {
        countSwaps++;
        if(countSwaps > maxSwaps)
        {
            return;
        }
        if(savedTetrimino != null)
        {
            GameObject temp = GameObject.FindGameObjectWithTag("saved");
            temp.transform.position = new Vector2(gridWidth / 2, gridHeight - 1);

            savedTetrimino = (GameObject)Instantiate(t.gameObject);
            savedTetrimino.GetComponent<Tetrimino>().enabled = false;
            savedTetrimino.transform.position = savedTetriminoPosition;
            rotatePiece();
            if (Regex.IsMatch(savedTetrimino.name, "I piece", RegexOptions.IgnoreCase))
            {
                savedTetrimino.transform.position += new Vector3(-0.1f, 0.9f, 0f);
            }
            else if (Regex.IsMatch(savedTetrimino.name, "Block piece", RegexOptions.IgnoreCase))
            {
                savedTetrimino.transform.position += new Vector3(-0.6f, 0f, 0f);
            }
            savedTetrimino.tag = "saved";
            savedTetrimino.GetComponent<Tetrimino>().wasSavedPiece = true;

            nextTetrimino = (GameObject)Instantiate(temp);
            nextTetrimino.GetComponent<Tetrimino>().enabled = true;
            nextTetrimino.transform.position = new Vector2(gridWidth / 2, gridHeight - 1);
            if(Regex.IsMatch(nextTetrimino.name, "I piece", RegexOptions.IgnoreCase))
            {
                nextTetrimino.transform.position += new Vector3(0.5f, 0.5f, 0f);
            }
            nextTetrimino.tag = "active";

            DestroyImmediate(t.gameObject);
            DestroyImmediate(temp);
            spawnGhost();

        }
        else
        {
            savedTetrimino = (GameObject)Instantiate(GameObject.FindGameObjectWithTag("active"));
            savedTetrimino.transform.position = savedTetriminoPosition;
            rotatePiece();
            if (Regex.IsMatch(savedTetrimino.name, "I piece", RegexOptions.IgnoreCase))
            {
                savedTetrimino.transform.position += new Vector3(-0.1f, 0.9f, 0f);
            }
            else if (Regex.IsMatch(savedTetrimino.name, "Block piece", RegexOptions.IgnoreCase))
            {
                savedTetrimino.transform.position += new Vector3(-0.6f, 0f, 0f);
            }
            savedTetrimino.GetComponent<Tetrimino>().enabled = false;
            savedTetrimino.tag = "saved";
            savedTetrimino.GetComponent<Tetrimino>().wasSavedPiece = true;

            DestroyImmediate(GameObject.FindGameObjectWithTag("active"));
            spawnNextTetrimino();
        }
    }

    void rotatePiece()
    {
        switch (savedTetrimino.GetComponent<Tetrimino>().rotationCounter)
        {
            case 0:
                return;
            case 1:
                savedTetrimino.transform.Rotate(0, 0, 90);
                return;
            case 2:
                savedTetrimino.transform.Rotate(0, 0, 180);
                return;
            case 3:
                savedTetrimino.transform.Rotate(0, 0, -90);
                return;
        }
    }

    public bool checkInsideGrid(Vector2 pos)
    {
        return ((int)pos.x >= 0 && (int)pos.x < gridWidth && (int)pos.y >= 0);
    }

    public Vector2 Round(Vector2 pos)
    {
        return new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
    }

    string GetRandomTetrimino()
    {
        int randomTetrimino = Random.Range(1, 8);
        
        string randomTetriminoName = "Prefabs/Block piece";

        switch (randomTetrimino)
        {
            case 1:
                randomTetriminoName = "Prefabs/I piece";
                break;
            case 2:
                randomTetriminoName = "Prefabs/J piece";
                break;
            case 3:
                randomTetriminoName = "Prefabs/L piece";
                break;
            case 4:
                randomTetriminoName = "Prefabs/S piece";
                break;
            case 5:
                randomTetriminoName = "Prefabs/T piece";
                break;
            case 6:
                randomTetriminoName = "Prefabs/Z piece";
                break;
            case 7:
                randomTetriminoName = "Prefabs/Block piece";
                break;
        }
        return randomTetriminoName;
    }
}
