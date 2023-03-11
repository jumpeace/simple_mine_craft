using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    // フィールドの水平方向の大きさ
    [SerializeField]
    private int width;
    // フィールドの垂直方向の大きさ
    [SerializeField]
    private int height;
    [SerializeField]
    // 起伏の緩さ
    private float relief;
    [SerializeField]
    // シード
    private int seed;

    void Start()
    {
        // ワールド生成
        GameObject grassBlock = (GameObject)Resources.Load("Grass");
        for (int x = 0; x < width; x++) {
            for (int z = 0; z < width; z++) {
                float noise = Mathf.PerlinNoise((x + seed) / relief, (z + seed) / relief);
                float groundY = (float)Math.Floor((height - 1) * noise);
                Instantiate(grassBlock, new Vector3((float)x, (float)groundY, (float)z), Quaternion.identity);
            }
        }
    }
}
