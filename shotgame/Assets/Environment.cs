using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour
{
    public int numLanes = 3;
    public int mapLength = 10;
    public GameObject roadPrefab;
    public GameObject pitfallPrefab;
    public GameObject obstaclePrefab;
    public float[][] mapData;
    // Start is called before the first frame update
    void Start()
    {
        mapData = new float[numLanes][];
        for (int i = 0; i < numLanes; i++)
        {
            mapData[i] = new float[mapLength];
            for (int j = 0; j < mapLength; j++)
            {
                mapData[i][j] = Random.Range(0, 10);
                GameObject a;//= Instantiate(roadPrefab);
                if (mapData[i][j] < 1.5)
                {
                    a = Instantiate(pitfallPrefab);
                }
                else if (mapData[i][j] < 2.5)
                {
                    a = Instantiate(obstaclePrefab);
                }
                else
                {
                    a = Instantiate(roadPrefab);
                }
                a.transform.position = new Vector3(j, i, 0);

            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
