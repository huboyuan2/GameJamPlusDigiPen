using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxSpawner : MonoBehaviour
{
    [SerializeField] private GameObject backTree;
    [SerializeField] private float backYOffset;
    [SerializeField] private float backYBASEDOffset;
    [SerializeField] private float backCooldownBeforeSpawn;
    private float backTimer = 0.0f;
    private float backTimeThreshHold;
    [SerializeField] private GameObject midTree;
    [SerializeField] private float midYOffset;
    [SerializeField] private float midYBASEDOffset;
    [SerializeField] private float midCooldownBeforeSpawn;
    private float midTimer = 0.0f;
    private float midTimeThreshHold;
    [SerializeField] private GameObject frontTree;
    [SerializeField] private float frontYOffset;
    [SerializeField] private float frontYBASEDOffset;
    [SerializeField] private float frontCooldownBeforeSpawn;
    private float frontTimer = 0.0f;
    private float frontTimeThreshHold;
    
    [SerializeField] private GameObject roadLights;
    [SerializeField] private float roadLightsYOffset;
    [SerializeField] private float roadLightsYBASEDOffset;
    [SerializeField] private float roadLightsCooldownBeforeSpawn;
    private float roadLightsTimer = 0.0f;
    private float roadLightsTimeThreshHold;
    
    [SerializeField] private GameObject spawnPoint;
    
    public bool startSpawn = false;
    // Start is called before the first frame update
    void Start()
    {
        backTimeThreshHold = Random.Range(0.2f, backCooldownBeforeSpawn);
        midTimeThreshHold = Random.Range(0.5f, midCooldownBeforeSpawn);
        frontTimeThreshHold = Random.Range(0.3f, midCooldownBeforeSpawn);
    }

    // Update is called once per frame
    void Update()
    {
        if (!startSpawn)
        {
            return;
        }
        backTimer += Time.deltaTime;
        if (backTimer >= backTimeThreshHold)
        {
            spawnBackTree();
            backTimer = 0.0f;
            backTimeThreshHold = Random.Range(0.2f, backCooldownBeforeSpawn);
        }
        
        midTimer += Time.deltaTime;
        if (midTimer >= midTimeThreshHold)
        {
            spawnMidTree();
            midTimer = 0.0f;
            midTimeThreshHold = Random.Range(0.3f, midCooldownBeforeSpawn);
        }
        
        frontTimer += Time.deltaTime;
        if (frontTimer >= frontTimeThreshHold)
        {
            spawnFrontTree();
            frontTimer = 0.0f;
            frontTimeThreshHold = Random.Range(0.3f, frontCooldownBeforeSpawn);
        }
        
        roadLightsTimer += Time.deltaTime;
        if (roadLightsTimer >= roadLightsTimeThreshHold)
        {
            spawnRoadLights();
            roadLightsTimer = 0.0f;
            roadLightsTimeThreshHold = roadLightsCooldownBeforeSpawn;
        }
    }

    void spawnBackTree()
    {
        float rand = Random.Range(0, backYOffset);
        Vector3 spawnPos = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y + rand - backYBASEDOffset, spawnPoint.transform.position.z);
        GameObject spawnedTree = Instantiate(backTree, spawnPos, Quaternion.identity);
    }
    
    void spawnMidTree()
    {
        float rand2 = Random.Range(0, midYOffset);
        Vector3 spawnPos = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y - midYBASEDOffset + rand2, spawnPoint.transform.position.z);
        GameObject spawnedTree = Instantiate(midTree, spawnPos, Quaternion.identity);
    }
    
    void spawnFrontTree()
    {
        float rand2 = Random.Range(0, frontYOffset);
        Vector3 spawnPos = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y - frontYBASEDOffset + rand2 - 0.25f, spawnPoint.transform.position.z);
        GameObject spawnedTree = Instantiate(frontTree, spawnPos, Quaternion.identity);
    }
    
    void spawnRoadLights()
    {
        float rand2 = Random.Range(0, roadLightsYOffset);
        Vector3 spawnPos = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y - roadLightsYBASEDOffset + rand2 - 0.25f, spawnPoint.transform.position.z);
        GameObject spawnedTree = Instantiate(roadLights, spawnPos, Quaternion.identity);
    }
}
