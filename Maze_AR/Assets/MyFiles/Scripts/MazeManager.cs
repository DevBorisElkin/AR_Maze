using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ImageTarget;

public class MazeManager : MonoBehaviour
{
    public ImageTarget mainTarget;
    public float waitToCheckPoint = 0.1f;

    [Space(6)]
    [Header("Multiple checks for better detection")]
    public float timeBetweenChecks = 0.1f;
    public int amountOfChecks = 5;

    [Header("Parent for points")]
    public GameObject parentForPoints;

    [Header("OuterRigParent")]
    public GameObject outerRig;

    [Header("MousePrefab")]
    public GameObject mouse;
    public Transform referenceTransform;

    [Space(6)]
    [SerializeField]
    public List<SceneObjectAR> objectsAR = new List<SceneObjectAR>();

    private void Start()
    {
        mainTarget.TargetFound += SetTargetFound;
        mainTarget.TargetLost += SetTargetLost;
        CreateListOfObjectsAR();
    }

    private void OnDestroy()
    {
        mainTarget.TargetFound -= SetTargetFound;
        mainTarget.TargetLost  -= SetTargetLost;
    }

    void CreateListOfObjectsAR()
    {
        objectsAR = new List<SceneObjectAR>();
        for (int i = 0; i < parentForPoints.transform.childCount; i++)
        {
            objectsAR.Add(new SceneObjectAR(parentForPoints.transform.GetChild(i)));
        }
    }

    void SetOuterRigActive(bool active)
    {
        outerRig.SetActive(active);
    }

    State currentState;
    void SetTargetFound()
    {
        ManageState(State.Found);

    }
    void SetTargetLost()
    {
        ManageState(State.Lost);
    }

    Texture2D screenTexture;
    void ManageState(State state)
    {
        currentState = state;
        StopAllCoroutines();
        StartCoroutine(ColorExtractingCoroutine(currentState));
    }

    List<Vector3[]> positionsForPoints;
    IEnumerator ColorExtractingCoroutine(State state)
    {
        
        if (state.Equals(State.Found))
        {
            positionsForPoints = new List<Vector3[]>(objectsAR.Count);
            print(positionsForPoints.Count + " positionsForPoints.Count");

            yield return new WaitForSeconds(waitToCheckPoint);

            screenTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            yield return new WaitForEndOfFrame();
            screenTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            screenTexture.Apply();

            print(objectsAR.Count);
            for (int i = 0; i < objectsAR.Count; i++)
            {
                print(i);
                positionsForPoints.Add(new Vector3[amountOfChecks]);
            }

            for (int i = 0; i < amountOfChecks; i++)
            {
                for (int j = 0; j < objectsAR.Count; j++)
                {
                    positionsForPoints[j][i] = objectsAR[j].rootTransform.position;
                }
                yield return new WaitForSeconds(timeBetweenChecks);
            }

            for (int i = 0; i < objectsAR.Count; i++)
            {
                objectsAR[i].CheckIfBusy(screenTexture, positionsForPoints[i]);
            }
            SetOuterRigActive(true);
            SpawnMouse();
        }
        else
        {
            foreach (SceneObjectAR objectAR in objectsAR)
            {
                objectAR.Disable();
            }
            SetOuterRigActive(false);
        }
    }

    #region Mouse related

    [EditorButton]
    public void SpawnMouse()
    {
        SceneObjectAR sceneObject = GetFreeSpot(objectsAR);
        if(sceneObject != null)
        {
            Transform transformToSpawn = sceneObject.rootTransform;

            GameObject mouseInstantiated = Instantiate(mouse, transformToSpawn.position, referenceTransform.rotation);
            mouseInstantiated.transform.SetParent(mainTarget.transform);
            mouseInstantiated.SetActive(true);
        }
    }

    SceneObjectAR GetFreeSpot(List<SceneObjectAR> list)
    {
        for (int i = 0; i < 5; i++)
        {
            int index = Random.Range(0, list.Count);
            print(index + " " + list.Count);
            if (IsObjectFree(list[index]))
            {
                return list[index];
            }
        }
        for (int i = 0; i < list.Count; i++)
        {
            if (IsObjectFree(list[i]))
            {
                return list[i];
            }
        }
        return null;
    }

    bool IsObjectFree(SceneObjectAR objectAR)
    {
        if (objectAR.mesh.enabled)
            return false;
        else return true;
    }
    #endregion
}
