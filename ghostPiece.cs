using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ghostPiece : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        tag = "ghost";
        foreach(Transform child in transform)
        {
            child.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.2f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        FollowActiveTetrimino();
        moveGhostDown();
    }

    void FollowActiveTetrimino()
    {
        transform.position = GameObject.FindGameObjectWithTag("active").transform.position;
        transform.rotation = GameObject.FindGameObjectWithTag("active").transform.rotation;


    }

    bool checkIsValidPosition()
    {
        foreach (Transform piece in transform)
        {
            Vector2 pos = FindObjectOfType<gameScript>().Round(piece.position);
            if (FindObjectOfType<gameScript>().checkInsideGrid(pos) == false)
            {
                return false;
            }
            if (FindObjectOfType<gameScript>().getTransformAtGridPosition(pos) != null && FindObjectOfType<gameScript>().getTransformAtGridPosition(pos).parent.tag == "active")
            {
                return true;
            }
            if (FindObjectOfType<gameScript>().getTransformAtGridPosition(pos) != null && FindObjectOfType<gameScript>().getTransformAtGridPosition(pos).parent != transform)
            {
                return false;
            }
        }
        return true;
    }

    void moveGhostDown()
    {
        while (checkIsValidPosition())
        {
            transform.position += new Vector3(0, -1, 0);
        }
        if (!checkIsValidPosition())
        {
            transform.position += new Vector3(0, 1, 0);
        }
    }
}
