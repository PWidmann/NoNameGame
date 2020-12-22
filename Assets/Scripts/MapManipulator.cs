using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Attach this script to the player object in the scene to get a basic first person terrain manipulator
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class MapManipulator : MonoBehaviour
{
    Camera cam;
    Ray ray;
    RaycastHit hit;
    LineRenderer lr;
    Vector3[] lrPos = new Vector3[2];

    public float interactionDistance = 15f;
    public float terrainHeightChangeAmount = 0.1f;
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        cam = Camera.main;
    }

    
    void Update()
    {
        ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.collider.tag == "Terrain")
            {
                int xPos = (int)hit.point.x;
                int zPos = (int)hit.point.z;
                float y = hit.point.y;

                lrPos[0] = new Vector3(xPos, y, zPos);
                lrPos[1] = new Vector3(xPos, y + 3, zPos);
                lr.SetPositions(lrPos);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            int xPos = (int)hit.point.x;
            int zPos = (int)hit.point.z;
            MapGenerator.Instance.noiseValues[xPos, zPos] -= terrainHeightChangeAmount;
            MapGenerator.Instance.UpdateMap();
        }

        if (Input.GetMouseButtonDown(1))
        {
            int xPos = (int)hit.point.x;
            int zPos = (int)hit.point.z;
            MapGenerator.Instance.noiseValues[xPos, zPos] += terrainHeightChangeAmount;
            MapGenerator.Instance.UpdateMap();
        }
    }
}
