using System.Collections;
using System.Collections.Generic;
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
    
    float GetRandomSymmetricalRange(float num)
    {
        return Random.Range(-num, num);
    }
    
    Vector3 GetRandomVector3(float x, float y, float z)
    {
        return new Vector3(GetRandomSymmetricalRange(x), 
                           GetRandomSymmetricalRange(y),
                           GetRandomSymmetricalRange(y));
    }

    Quaternion GetRandomQuaternion(float x, float y, float z, float w)
    {
        return new Quaternion(GetRandomSymmetricalRange(x), 
                              GetRandomSymmetricalRange(y) , 
                              GetRandomSymmetricalRange(z), 
                              GetRandomSymmetricalRange(w));
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < numberOfItems; i++)
        {
            GameObject obj = Instantiate(spawnObject, 
                                         GetRandomVector3(spawnGridX, spawnGridY, spawnGridZ), 
                                         GetRandomQuaternion(1f, 1f, 1f, 1f), 
                                         transform);
            obj.GetComponent<Rigidbody>().AddForce(GetRandomVector3(1, 1, 1) * 1000);
        }
    }
}

