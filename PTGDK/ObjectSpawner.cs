using System.Collections;
using System.Collections.Generic;
using PTGDK.Utility;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField] private GameObject spawnObject;
    [SerializeField] private int numberOfItems = 10;
    
    [SerializeField] private float spawnGridX = 10;
    [SerializeField] private float spawnGridY = 10;
    [SerializeField] private float spawnGridZ = 10;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < numberOfItems; i++)
        {
            GameObject obj = Instantiate(spawnObject, 
                                         RandomUtils.GetRandomVector3(spawnGridX, spawnGridY, spawnGridZ), 
                                         RandomUtils.GetRandomQuaternion(1f, 1f, 1f, 1f), 
                                         transform);
            obj.GetComponent<Rigidbody>().AddForce(RandomUtils.GetRandomVector3(1, 1, 1) * 1000);
        }
    }
}

