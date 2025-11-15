using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletUI : MonoBehaviour
{
    public GameObject bulletIcon;
    public GameObject bulletContainer;
    private int bulletCount = 0;
    private static BulletUI _instance;

    public static BulletUI Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<BulletUI>();

                if (_instance == null)
                {
                    Debug.LogWarning("[BulletUI] No BulletUI found in scene!");
                }
            }
            return _instance;
        }
    }

    public int BulletCount
    {
        get { return bulletCount; }
        set
        {
            bulletCount = value;
            UpdateBulletUI();
        }
    }
    void UpdateBulletUI()
    {
        // Clear existing bullet icons
        foreach (Transform child in bulletContainer.transform)
        {
            Destroy(child.gameObject);
        }
        // Instantiate new bullet icons based on bulletCount
        for (int i = 0; i < bulletCount; i++)
        {
            Instantiate(bulletIcon, bulletContainer.transform);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
