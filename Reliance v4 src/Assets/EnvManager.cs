using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvManager : MonoBehaviour
{
    public GameObject LeftWall;
    public GameObject RightWall;

    public GameObject BgNear;
    public GameObject BgFar;

    public List<GameObject> ExistingWalls;
    public List<GameObject> ExistingClouds;

    private void Awake()
    {
        ExistingWalls = new List<GameObject>();
        ExistingClouds = new List<GameObject>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float camY = Camera.main.transform.position.y;
        BgNear.transform.position = Vector3.up * (camY * 0.2f);
        BgFar.transform.position = Vector3.up * (camY * 0.5f);
    }

    void OldWallManagement()
    {
        float topOfScreen = GlobalManager.Instance.GetTopOfScreen();
        float bottomOfScreen = GlobalManager.Instance.GetBottomOfScreen();
        float screenHeight = topOfScreen - bottomOfScreen;
        bool needsToAddWall = true;
        float topmostWallY = float.NegativeInfinity;
        for (int i = 0; i < ExistingWalls.Count; i++)
        {
            var wall = ExistingWalls[i];
            var wallBounds = wall.GetComponent<BoxCollider2D>().bounds;
            if (wallBounds.max.y > topOfScreen)
            {
                needsToAddWall = false;
            }
            if (wallBounds.max.y < bottomOfScreen - screenHeight)
            {
                // Delete this wall
                Destroy(wall.gameObject);
                ExistingWalls.RemoveAt(i);
                i--;
            }
            else
            {
                if (topmostWallY < wall.transform.position.y)
                {
                    topmostWallY = wall.transform.position.y;
                }
            }
        }
        if (needsToAddWall)
        {
            GameObject[] newWalls = new GameObject[]
            {
                Instantiate(LeftWall),
                Instantiate(RightWall)
            };

            foreach (var wall in newWalls)
            {
                var pos = wall.transform.position;
                pos.y = topmostWallY + wall.GetComponent<BoxCollider2D>().size.y;
                wall.transform.position = pos;
                ExistingWalls.Add(wall);
            }
        }
    }
}
