using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour
{
    public int numLanes = 3;
    public int mapLength = 10;
    public int maxPitfallDistance = 1;
    public float scrollPosition = 0;
    public float lagBackDestroy = -10f;
    public float leadSpawnDistance = 45f;
    public float scrollSpeed = 100f;
    public GameObject roadPrefab;
    public GameObject pitfallPrefab;
    public GameObject obstaclePrefab;
    public LinkedList<float>[] mapData;
    public int[] lanePitfallDistances;
    // Start is called before the first frame update
    void Start()
    {
        lanePitfallDistances = new int[numLanes];
        mapData = new LinkedList<float>[numLanes];
        for (int i = 0; i < numLanes; i++)
        {
            float[] b = new float[mapLength];
            
            for (int j = 0; j < mapLength; j++)
            {
                b[j] = (Random.Range(0, 10));
                GameObject a;//= Instantiate(roadPrefab);
                if (b[j] < 1.45)
                {
                    if (lanePitfallDistances[i] >= maxPitfallDistance)
                    {
                        a = Instantiate(roadPrefab);
                        lanePitfallDistances[i] = 0;
                    }
                    else
                    {
                        a = Instantiate(pitfallPrefab);
                        lanePitfallDistances[i]++;
                    }
                }
                else if (b[j] < 2.0)
                {
                    a = Instantiate(obstaclePrefab);
                    lanePitfallDistances[i]++;
                }
                else
                {
                    a = Instantiate(roadPrefab);
                }
                Vector3 sc = transform.localScale;
                a.transform.position = new Vector3(j * sc.x, i * sc.y, 0 * sc.z - 0.7071f * transform.localScale.z * (numLanes - i)) + transform.position;
                a.transform.localScale = transform.localScale;
                a.transform.localScale = new Vector3(a.transform.localScale.x, a.transform.localScale.y, 0);
                a.transform.SetParent(transform);

            }
            mapData[i] = new LinkedList<float>(b);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x - scrollSpeed * Time.deltaTime, transform.position.y, transform.position.z);// = scrollSpeed * Time.deltaTime;
    }
}
