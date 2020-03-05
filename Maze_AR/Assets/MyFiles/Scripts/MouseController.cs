using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ImageTarget;

public class MouseController : MonoBehaviour
{
    #region variables
    MazeManager manager;

    int layerMask = 1 << 0;

    public float moveSpeed = 5f;
    RaycastHit hit;

    public Vector3 setScaleTo = new Vector3(0.0438034f, 0.0438034f, 0.0438034f);
    #endregion

    #region main Unity callbacks
    private void Awake()
    {
        manager = FindObjectOfType<MazeManager>();
    }

    void Start()
    {
        transform.localScale = setScaleTo;
    }

    private void Update()
    {
        Move();
        CheckPointTowards();
    }

    private void OnDisable()
    {
        Destroy(gameObject);
    }

    #endregion

    bool IsObjectFree(GameObject obj)
    {
        if(obj.GetComponentInChildren<MeshRenderer>().enabled)
            return false;
        else return true;
    }

    void Move()
    {
        if (movingTo != null) transform.position = Vector3.MoveTowards(transform.position, movingTo.transform.position, moveSpeed * Time.deltaTime);
    }

    GameObject movingTo;
    void CheckPointTowards()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);

            if (IsObjectFree(hit.collider.gameObject))
            {
                if(movingTo != hit.collider.gameObject)
                {
                    if(TryChance(7))
                        RotateToRandomSideDirection();
                    else movingTo = hit.collider.gameObject;
                }
            }
            else
            {
                movingTo = null;
                RotateToRandomDirection();
            }
            
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
        }
    }

    void RotateToRandomDirection()
    {
        int rand = Random.Range(0, 4);
        transform.Rotate(0, 90 * rand, 0);
    }

    void RotateToRandomSideDirection()
    {
        int rand = Random.Range(0,2);
        if(rand == 0)
        {
            transform.Rotate(0, -90, 0);
        }
        else if(rand == 1)
        {
            transform.Rotate(0, 90, 0);
        }

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
        {
            if (IsObjectFree(hit.collider.gameObject))
            {
                if (movingTo != hit.collider.gameObject)
                {
                    movingTo = hit.collider.gameObject;
                }
            }
            else
            {
                movingTo = null;
                RotateToRandomDirection();
            }
        }
    }

    bool TryChance(int luckPercent)
    {
        int rand = Random.Range(1, 101);
        if (luckPercent > rand) return true;
        else return false;
    }
}
