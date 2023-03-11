using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGenerator : MonoBehaviour
{
    void Start()
    {
        // 16×16 草ブロックを配置
        GameObject block = (GameObject)Resources.Load("Grass");
        for (int x = 0; x < 16; x++) {
            for (int z = 0; z < 16; z++) {
                Instantiate(block, new Vector3((float)x, 0.0f, (float)z), Quaternion.identity);
            }
        }
    }
}
