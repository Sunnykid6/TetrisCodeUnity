using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveColliders : MonoBehaviour
{
    public BoxCollider2D[] myColliders;
    public List<GameObject> childObjects;
    public List<Vector2> childPositions;

    public bool DeleteColliders = false;
    public bool firstTime = true;
    public int storeRotation = 0;
    public int numberOfChilds = 4;
    public int getChildEveryFrame = 0;

    // Start is called before the first frame update
    void Start()
    {
        myColliders = gameObject.GetComponents<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (firstTime)
        {
            for (int i = 0; i < transform.childCount; i++)
            {

                childObjects.Add(transform.GetChild(i).gameObject);
            }
            firstTime = false;
        }
        checkIfParentIsEmpty();
        deleteObjectsThenBoxCollider();
    }

    public void checkIfParentIsEmpty()
    {
        if (transform.childCount < 1)
        {
            Destroy(gameObject);
        }
    }

    public void deleteObjectsThenBoxCollider()
    {
        int i = 0;
        for (int j = 0; j < 4; j++)
        {
            if (childObjects[i] == null)
            {
                myColliders[i].enabled = false;
            }
            i++;
        }
    }

}
