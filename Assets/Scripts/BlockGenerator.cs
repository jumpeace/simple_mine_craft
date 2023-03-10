using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGenerator : MonoBehaviour
{
    void Start()
    {
        GameObject block = (GameObject)Resources.Load ("Block");
        for (int x = 0; x < 16; x++) {
            for (int z = 0; z < 16; z++) {
                Instantiate (block, new Vector3((float)x, 0.0f, (float)z), Quaternion.identity);
            }
        }
    }

    void Update()
    {
        
    }
}
