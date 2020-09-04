using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetrimino : MonoBehaviour
{
    public float fallSpeed = 1;
    public bool isPreviewPiece = false;
    public bool allowedRotation = true;
    public bool isIpiece = false;
    public int rotationCounter = 0;
    public GameObject child1;
    public GameObject child2;
    public GameObject child3;
    public bool SetchildObjects = true;
    public bool wasSavedPiece = false;
    public bool firstTime = true;

    private float continuousVerticalSpeed = 0.05f;
    private float continuousHorizontalSpeed = 0.1f;
    private float buttonDownWaitMax = 0.2f;

    private float verticalTimer = 0;
    private float horizontalTimer = 0;
    private float buttonDownWaitTimer = 0;

    private bool moveImmediateVertical = false;
    private bool moveImmediateHorizontal = false;

    int counterforCalling = 0;
    int checkRotation = 0;
    int IpieceRotation = 0;
    float fall = 0;
    bool tryAllRotationCheck = false;
    BoxCollider2D[] myColliders;

    //public BoxCollider2D[] myColliders;
    public List<GameObject> childObjects;
    public List<Vector2> childPositions;

    // Start is called before the first frame update
    void Start()
    {
        myColliders = gameObject.GetComponents<BoxCollider2D>();
        foreach (BoxCollider2D bc in myColliders) bc.enabled = false;
       
    }

    // Update is called once per frame
    void Update()
    {
        if (firstTime)
        {
            for (int i = 0; i < transform.childCount; i++)
            {

                childObjects.Add(transform.GetChild(i).gameObject);
                Debug.Log(childObjects[i]);
            }
            Debug.Log(childObjects.Count);
            firstTime = false;
        }
        checkUserInput();
    }

    void checkUserInput()
    {

        if(Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.DownArrow))
        {
            moveImmediateHorizontal = false;
            moveImmediateVertical = false;

            horizontalTimer = 0;
            verticalTimer = 0;
            buttonDownWaitTimer = 0;
        }


        if (Input.GetKey(KeyCode.RightArrow))
        {
            if (moveImmediateHorizontal)
            {
                if (buttonDownWaitTimer < buttonDownWaitMax)
                {
                    buttonDownWaitTimer += Time.deltaTime;
                    return;
                }
                if (horizontalTimer < continuousHorizontalSpeed)
                {
                    horizontalTimer += Time.deltaTime;
                    return;
                }
            }
            if (!moveImmediateHorizontal)
            {
                moveImmediateHorizontal = true;
            }
            horizontalTimer = 0;
            transform.position += new Vector3(1, 0, 0);
            if (checkIsValidPosition())
            {
                FindObjectOfType<gameScript>().UpdateGrid(this);
            }
            else
            {
                transform.position += new Vector3(-1, 0, 0);
            }
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (moveImmediateHorizontal)
            {
                if (buttonDownWaitTimer < buttonDownWaitMax)
                {
                    buttonDownWaitTimer += Time.deltaTime;
                    return;
                }
                if (horizontalTimer < continuousHorizontalSpeed)
                {
                    horizontalTimer += Time.deltaTime;
                    return;
                }
            }
            if (!moveImmediateHorizontal)
            {
                moveImmediateHorizontal = true;
            }
            horizontalTimer = 0;
            transform.position += new Vector3(-1, 0, 0);
            if (checkIsValidPosition())
            {
                FindObjectOfType<gameScript>().UpdateGrid(this);
            }
            else
            {
                transform.position += new Vector3(1, 0, 0);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            while (checkIsValidPosition())
            {
                transform.position += new Vector3(0, -1, 0);
            }
            if (!checkIsValidPosition())
            {
                transform.position += new Vector3(0, 1, 0);
                FindObjectOfType<gameScript>().UpdateGrid(this);
                enabled = false;
                if (isIpiece)
                {
                    turnOffChildColliders();
                }
                setChildPositions();
                //FindObjectOfType<moveColliders>().storeRotation = rotationCounter;
                foreach (BoxCollider2D bc in myColliders) bc.enabled = true;
                gameObject.tag = "tetriminos";
                FindObjectOfType<gameScript>().DeleteRow();

                FindObjectOfType<gameScript>().spawnNextTetrimino();
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            GameObject temp = GameObject.FindGameObjectWithTag("active");
            FindObjectOfType<gameScript>().saveTetrimino(temp.transform);
        }
        else if (Input.GetKey(KeyCode.DownArrow) || Time.time - fall >= fallSpeed)
        {
            if (moveImmediateVertical)
            {
                if (buttonDownWaitTimer < buttonDownWaitMax)
                {
                    buttonDownWaitTimer += Time.deltaTime;
                    return;
                }
                if (verticalTimer < continuousVerticalSpeed)
                {
                    verticalTimer += Time.deltaTime;
                    return;
                }
            }
            if (!moveImmediateVertical)
            {
                moveImmediateVertical = true;
            }
            verticalTimer = 0;
            transform.position += new Vector3(0, -1, 0);
            if (checkIsValidPosition())
            {
                FindObjectOfType<gameScript>().UpdateGrid(this);
            }
            else
            {
                transform.position += new Vector3(0, 1, 0);
                enabled = false;
                if (isIpiece)
                {
                    turnOffChildColliders();
                }
                setChildPositions();
                //FindObjectOfType<moveColliders>().storeRotation = rotationCounter;
                foreach (BoxCollider2D bc in myColliders) bc.enabled = true;
                gameObject.tag = "tetriminos";
                FindObjectOfType<gameScript>().DeleteRow();
                
                FindObjectOfType<gameScript>().spawnNextTetrimino();
            }
            fall = Time.time;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (allowedRotation)
            {
                if (isIpiece)
                {
                    if (tryAllRotationCheck)
                    {
                        checkITetriminoCollisionWithTetrimino(-90);
                    }
                    else
                    {
                        transform.Rotate(0, 0, -90);
                        checkWallAfterRotateIpiece();
                        checkTetriminoIsInValidAfterRotationAndMove(-90);
                    }
                }
                else
                {
                    if (tryAllRotationCheck)
                    {
                        checkTetriminoCollisionWithTetrimino(-90);
                    }
                    else
                    {
                        transform.Rotate(0, 0, -90);
                        checkWallAfterRotate();
                        checkTetriminoIsInValidAfterRotationAndMove(-90);
                    }
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            if (allowedRotation)
            {
                if (isIpiece)
                {
                    if (tryAllRotationCheck)
                    {
                        checkITetriminoCollisionWithTetrimino(90);
                    }
                    else
                    {
                        transform.Rotate(0, 0, 90);
                        checkWallAfterRotateIpiece();
                        checkTetriminoIsInValidAfterRotationAndMove(90);
                    }
                }
                else
                {
                    if (tryAllRotationCheck)
                    {
                        checkTetriminoCollisionWithTetrimino(90);
                    }
                    else
                    {
                        transform.Rotate(0, 0, 90);
                        checkWallAfterRotate();
                        checkTetriminoIsInValidAfterRotationAndMove(90);
                    }
                }
            }
        }
    }

    public void callMoveColliders()
    {
        for(int i = 0; i < 3; i++)
        {
            if (FindObjectOfType<gameScript>().isDelete)
            {
                if (i >= 2)
                {
                    FindObjectOfType<gameScript>().isDelete = false;
                    FindObjectOfType<gameScript>().moveCollidersdown();
                    FindObjectOfType<gameScript>().LinesDeleted = 0;
                    i = 0;
                }
            }
        }
    }

    void checkTetriminoIsInValidAfterRotationAndMove(int x)
    {
        if (checkIsValidPosition())
        {
            IpieceRotation = 0;
            checkRotation = 0;
            FindObjectOfType<gameScript>().UpdateGrid(this);
            rotationCounter = rotationCounter + 1;
            if (rotationCounter > 3)
            {
                rotationCounter = 0;
            }
        }
        else
        {
            transform.Rotate(0, 0, -x);
            if (isIpiece)
            {
                moveIPieceBackBeforeRotate();
            }
            movePieceBackBeforeRotate();
        }
    }

    void checkITetriminoCollisionWithTetrimino(int x)
    {
        switch (rotationCounter)
        {
            case 0:
                setIpieceint();
                setCheckRotationForCases();
                transform.Rotate(0, 0, x);
                checkWallAfterRotateIpiece();
                checkTetriminoIsInValidAfterRotationAndMove(x);
                tryAllRotationCheck = false;
                return;
            case 1:
                setIpieceint();
                setCheckRotationForCases();
                transform.Rotate(0, 0, x);
                checkWallAfterRotateIpiece();
                checkTetriminoIsInValidAfterRotationAndMove(x);
                tryAllRotationCheck = false;
                return;
            case 3:
                setIpieceint();
                setCheckRotationForCases();
                transform.Rotate(0, 0, x);
                checkWallAfterRotateIpiece();
                checkTetriminoIsInValidAfterRotationAndMove(x);
                tryAllRotationCheck = false;
                return;
        }
    }

    void checkTetriminoCollisionWithTetrimino(int x)
    {
        switch (rotationCounter)
        {
            case 0:
                checkRotation = 3;
                transform.Rotate(0, 0, x);
                checkWallAfterRotate();
                checkTetriminoIsInValidAfterRotationAndMove(x);
                tryAllRotationCheck = false;
                return;
            case 1:
                checkRotation = 2;
                transform.Rotate(0, 0, x);
                checkWallAfterRotate();
                checkTetriminoIsInValidAfterRotationAndMove(x);
                tryAllRotationCheck = false;
                return;
            case 3:
                checkRotation = 1;
                transform.Rotate(0, 0, x);
                checkWallAfterRotate();
                checkTetriminoIsInValidAfterRotationAndMove(x);
                tryAllRotationCheck = false;
                return;
        }
    }
    /*
     * sets the checkRotation int when any piece touches the left wall
     * right wall or bottom floor so that the piece and properly rotate
     */
    private void OnTriggerEnter2D(Collider2D collisionInfo)
    {
        if (collisionInfo.tag == "bottomWall")
        {
            checkRotation = 3;
            if (isIpiece)
            {
                setIpieceint();
            }
        }
        else if (collisionInfo.tag == "leftWall")
        {
            checkRotation = 2;
            if (isIpiece)
            {
                setIpieceint();
            }
        }
        else if (collisionInfo.tag == "rightWall")
        {
            checkRotation = 1;
            if (isIpiece)
            {
                setIpieceint();
            }
        }
        else if (collisionInfo.tag == "tetriminos")
        {
            tryAllRotationCheck = true;
        }
        else
        {
            checkRotation = 0;
        }
    }
    /*
     * checks to see whether the piece is in the board and if a spot
     * is a valid location and isn't taken up by another tetrimino
     */
    bool checkIsValidPosition()
    {
        foreach (Transform piece in transform)
        {
            Vector2 pos = FindObjectOfType<gameScript>().Round(piece.position);
            if (FindObjectOfType<gameScript>().checkInsideGrid(pos) == false)
            {
                return false;
            }
            if(FindObjectOfType<gameScript>().getTransformAtGridPosition(pos) != null && FindObjectOfType<gameScript>().getTransformAtGridPosition(pos).parent != transform)
            {
                return false;
            }
        }
        return true;
    }

    /*
     * turns off the colliders for the Ipiece that have to do 
     * with checking for rotation
     */
    void turnOffChildColliders()
    {
        child1.GetComponent<BoxCollider2D>().enabled = false;
        child2.GetComponent<BoxCollider2D>().enabled = false;
        child3.GetComponent<CapsuleCollider2D>().enabled = false;
    }

    /*
     * helper method to set the checkRotation to indicate what kind of wall
     * had been hit when it touches another tetrimino block
     * ie if it hits a block on the left, act like it hit the left wall
     */
    void setCheckRotationForCases()
    {
        switch (rotationCounter)
        {
            case 0:
                if(IpieceRotation == 3)
                {
                    checkRotation = 3;
                }
                return;
            case 1:
                if(IpieceRotation == 1 || IpieceRotation == 2)
                {
                    checkRotation = 1;
                }
                else if(IpieceRotation == 3)
                {
                    checkRotation = 2;
                }
                return;
            case 2:
                if (IpieceRotation == 1 || IpieceRotation == 2)
                {
                    checkRotation = 3;
                }
                return;
            case 3:
                if (IpieceRotation == 1 || IpieceRotation == 2)
                {
                    checkRotation = 2;
                }
                else if(IpieceRotation == 3)
                {
                    checkRotation = 1;
                }
                return;
        }
    }


    /*
     * Check to see if any of the childs on the Ipiece got deleted
     * this is to avoid any errors when we set the Ipiece rotatoin
    */
    bool setTrueorFalse()
    {
        if(child1 == null)
        {
            return true;
        }
        else if(child2 == null)
        {
            return true;
        }
        else if(child3 == null)
        {
            return true;
        }
        return false;
    }
    /*
     * Set the I piece int to indicate which of the child 
     * box colliders collided with another object
     */
    void setIpieceint()
    {
        if (!setTrueorFalse() && child1.GetComponent<blockCollision>().collided && !child2.GetComponent<blockCollision>().collided)
        {
            IpieceRotation = 1;
            child1.GetComponent<blockCollision>().collided = false;
            child2.GetComponent<blockCollision>().collided = false;
            child3.GetComponent<blockCollision>().collided = false;
        }
        else if (!setTrueorFalse() && child2.GetComponent<blockCollision>().collided)
        {
            IpieceRotation = 2;
            child1.GetComponent<blockCollision>().collided = false;
            child2.GetComponent<blockCollision>().collided = false;
            child3.GetComponent<blockCollision>().collided = false;
        }
        else if (!setTrueorFalse() && child3.GetComponent<blockCollision>().collided)
        {
            IpieceRotation = 3;
            child1.GetComponent<blockCollision>().collided = false;
            child2.GetComponent<blockCollision>().collided = false;
            child3.GetComponent<blockCollision>().collided = false;
        }
    }
    /*
     * Helper method to move the piece into a more desired location
     * after the rotation to help avoid any wall collsions
     */
    void checkWallAfterRotate()
    {
        if (checkRotation == 3)
        {
            transform.position += new Vector3(0, 1, 0);
        }
        else if (checkRotation == 2)
        {
            transform.position += new Vector3(1, 0, 0);
        }
        else if (checkRotation == 1)
        {
            transform.position += new Vector3(-1, 0, 0);
        }
    }
    /*
     * move the piece back before it rotated if the piece 
     * didn't have enough room for the next rotation 
     */
    void movePieceBackBeforeRotate()
    {
        if (checkRotation == 3)
        {
            transform.position += new Vector3(0, -1, 0);
            checkRotation = 0;
        }
        else if (checkRotation == 2)
        {
            transform.position += new Vector3(-1, 0, 0);
            checkRotation = 0;
        }
        else if (checkRotation == 1)
        {
            transform.position += new Vector3(1, 0, 0);
            checkRotation = 0;
        }
    }
    /*
     * helper method to move the piece into the right spot
     * to avoid any wall collisions
     */
    void checkWallAfterRotateIpiece()
    {
        if (checkRotation == 3)
        {
            if (IpieceRotation == 1 || IpieceRotation == 3)
            {
                transform.position += new Vector3(0, 1, 0);
            }
            else if(IpieceRotation == 2)
            {
                transform.position += new Vector3(0, 2, 0);
            }
        }
        else if (checkRotation == 2)
        {
            if (IpieceRotation == 1 || IpieceRotation == 3)
            {
                transform.position += new Vector3(1, 0, 0);
            }
            else if (IpieceRotation == 2)
            {
                transform.position += new Vector3(2, 0, 0);
            }
        }
        else if (checkRotation == 1)    
        {
            if (IpieceRotation == 1 || IpieceRotation == 3)
            {
                transform.position += new Vector3(-1, 0, 0);
            }
            else if (IpieceRotation == 2)
            {
                transform.position += new Vector3(-2, 0, 0);
            }
        }
    }


    /*
     * If the Piece didn't have room to rotate after moving
     * put it back in its original location based on which 
     * way it was facing
     */
    void moveIPieceBackBeforeRotate()
    {
        if (checkRotation == 3)
        {
            if (IpieceRotation == 1 || IpieceRotation == 3)
            {
                transform.position += new Vector3(0, -1, 0);
                IpieceRotation = 0;
                checkRotation = 0;
            }
            else if (IpieceRotation == 2)
            {
                transform.position += new Vector3(0, -2, 0);
                IpieceRotation = 0;
                checkRotation = 0;
            }
        }
        else if (checkRotation == 2)
        {
            if (IpieceRotation == 1 || IpieceRotation == 3)
            {
                transform.position += new Vector3(-1, 0, 0);
                IpieceRotation = 0;
                checkRotation = 0;
            }
            else if (IpieceRotation == 2)
            {
                transform.position += new Vector3(-2, 0, 0);
                IpieceRotation = 0;
                checkRotation = 0;
            }
        }
        else if (checkRotation == 1)
        {
            if (IpieceRotation == 1 || IpieceRotation == 3)
            {
                transform.position += new Vector3(1, 0, 0);
                IpieceRotation = 0;
                checkRotation = 0;
            }
            else if (IpieceRotation == 2)
            {
                transform.position += new Vector3(2, 0, 0);
                IpieceRotation = 0;
                checkRotation = 0;
            }
        }
    }



    public void setChildPositions()
    {
        foreach (GameObject obj in childObjects)
        {
            Vector2 pos = FindObjectOfType<gameScript>().Round(obj.transform.position);
            childPositions.Add(pos);
        }
    }

    List<GameObject> updateChildList(GameObject tetriminoToTest)
    {
        List<GameObject> newChildObject = new List<GameObject>();
        for (int i = 0; i < tetriminoToTest.transform.childCount; i++)
        {
            newChildObject.Add(tetriminoToTest.transform.GetChild(i).gameObject);
        }
        return newChildObject;
    }

    List<int> checkifChildPositionChanged(List<GameObject> newChildObjectList)
    {
        List<int> indexes = new List<int>();
        int i = 0;
        Debug.Log(childObjects.Count);
        for (int j = 0; j < 4; j++)
        {
            if (childObjects[j] == null)
            {
                continue;
            }
            if (childObjects[j].name != newChildObjectList[i].name)
            {
                continue;
            }
            else
            {
                Vector2 pos2 = FindObjectOfType<gameScript>().Round(newChildObjectList[i].transform.position);
                if (childPositions[j] != pos2)
                {
                    childPositions[j] = pos2;
                    indexes.Add(j);
                }
                i++;
            }
        }
        return indexes;
    }

    public void moveCollidersdown(GameObject objectColliderToMove)
    {
        Debug.Log("InsideMoveColliderTetrimino");
        if (FindObjectOfType<gameScript>().LinesDeleted > 0)
        {
            List<int> indexes = checkifChildPositionChanged(updateChildList(objectColliderToMove));
            if (indexes.Count == 0)
            {
                return;
            }
            switch (rotationCounter)
            {
                case 0:
                    for (int i = 0; i < indexes.Count; i++)
                    {
                        myColliders[indexes[i]].offset += new Vector2(0, -FindObjectOfType<gameScript>().LinesDeleted);
                    }
                    FindObjectOfType<gameScript>().isDelete = false;
                    return;
                case 1:
                    for (int i = 0; i < indexes.Count; i++)
                    {
                        myColliders[indexes[i]].offset += new Vector2(FindObjectOfType<gameScript>().LinesDeleted, 0);
                    }
                    FindObjectOfType<gameScript>().isDelete = false;
                    return;
                case 2:
                    for (int i = 0; i < indexes.Count; i++)
                    {
                        myColliders[indexes[i]].offset += new Vector2(0, FindObjectOfType<gameScript>().LinesDeleted);
                    }
                    FindObjectOfType<gameScript>().isDelete = false;
                    return;
                case 3:
                    for (int i = 0; i < indexes.Count; i++)
                    {
                        myColliders[indexes[i]].offset += new Vector2(-FindObjectOfType<gameScript>().LinesDeleted, 0);
                    }
                    FindObjectOfType<gameScript>().isDelete = false;

                    return;
            }
        }
    }
}
