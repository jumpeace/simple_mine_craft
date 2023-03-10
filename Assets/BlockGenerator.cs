using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGenerator : MonoBehaviour
{
    void Start()
    {
        GameObject obj = (GameObject)Resources.Load ("Block");
        Instantiate (obj, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
        Instantiate (obj, new Vector3(0.0f, 1.0f, 0.0f), Quaternion.identity);
    }

    void Update()
    {
        
    }
}
