using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    public ParallaxSpawner treeSpawner;
    public List<ParallaxObject> allSetupTree;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void startGame()
    {
        foreach (ParallaxObject parallaxObject in allSetupTree)
        {
            parallaxObject.isMoving = true;
        }

        treeSpawner.startSpawn = true;
    }
}
