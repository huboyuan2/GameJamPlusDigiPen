using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public bool gamePaused = true;
    public GameObject roadPrefab;
    public GameObject pitfallPrefab;
    public GameObject pitfall2Prefab;
    public GameObject pitfall3Prefab;
    public GameObject obstaclePrefab;
    public GameObject emptyPrefab;
    public GameObject grPrefab;
    public GameObject player;
    public ParallaxSpawner ps;
    public LinkedList<float>[] mapData;
    public Texture roadTex;
    public int[] lanePitfallDistances;
    // Start is called before the first frame update
    void Start()
    {
        lanePitfallDistances = new int[numLanes];
        for (int i = 0; i < numLanes; i++)
        {
            lanePitfallDistances[i] = 0;
        }
        mapData = new LinkedList<float>[numLanes];
        for (int i = 0; i < numLanes; i++)
        {
            float[] b = new float[mapLength];
            
            for (int j = 0; j < mapLength; j++)
            {
                if (j % 8 == 0 && i == 1)
                {
                    GameObject roadSpan = Instantiate(roadPrefab);
                    roadSpan.transform.position = new Vector3(j + transform.position.x , i + transform.position.y, transform.position.z + 0.1f);
                    // keep the scaling and slice date //roadSpan.transform.localScale = new Vector3(1, 1, 1);
                    //a.transform.localScale = new Vector3(a.transform.localScale.x, a.transform.localScale.y, 0);
                    roadSpan.transform.SetParent(transform, false);
                    GameObject guardRail = Instantiate(grPrefab);
                    guardRail.transform.position = new Vector3(j + transform.position.x, 0.37f, +transform.position.z);
                    guardRail.transform.SetParent(transform, false);
                }
                b[j] = (Random.Range(0, 10));
                GameObject a;//= Instantiate(roadPrefab);
                if (b[j] < 1.45) // pitfall
                {
                    if (lanePitfallDistances[i] >= maxPitfallDistance)
                    {
                        a = Instantiate(emptyPrefab);
                        lanePitfallDistances[i] = 0;
                    }
                    else
                    {
                        a = Instantiate(pitfallPrefab);
                        lanePitfallDistances[i]++;
                    }
                }
                else if (b[j] < 2.0)// dumpster fire
                {
                    a = Instantiate(obstaclePrefab);
                    lanePitfallDistances[i]++;
                }
                else // regular road tile
                {
                    a = Instantiate(emptyPrefab);
                    //a.GetComponent<Renderer>().material.mainTexture = roadTex;// SetTexture()
                    //a_Tex = roadTex;

                }
                Vector3 sc = transform.localScale;
                a.transform.position = new Vector3(j , i , .1f) + transform.position;
                a.transform.localScale = new Vector3(1,1,1);
                //a.transform.localScale = new Vector3(a.transform.localScale.x, a.transform.localScale.y, 0);
                a.transform.SetParent(transform, false);
                

            }
            mapData[i] = new LinkedList<float>(b);
        }



        for (int j = 0; j < mapLength; j++)
        {
            bool isLargePit = true;
            for (int i = 0; i < numLanes; i++)
            {
                if (mapData[i].ToArrayPooled()[j] > 1.45 )
                {
                    isLargePit = false;
                }
            }

            if (isLargePit)
            {
                GameObject lPit = Instantiate(pitfall3Prefab);
                lPit.transform.position = new Vector3(j, 0, .1f) + transform.position;
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (ps.startSpawn)
        {
            gamePaused = false;
        }
        if (!gamePaused)
        {
            transform.position = new Vector3(transform.position.x - scrollSpeed * Time.deltaTime, transform.position.y, transform.position.z);// = scrollSpeed * Time.deltaTime;
        }
    }
        
}
