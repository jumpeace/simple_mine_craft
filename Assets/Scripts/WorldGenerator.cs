using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    // チャンクの大きさ
    [SerializeField]
    private int chunkSize;
    // x, z方向のチャンク数
    [SerializeField]
    private int chunkNum;
    // フィールドの垂直方向の大きさ
    [SerializeField]
    private int height;
    [SerializeField]
    // 起伏の緩さ
    private float relief;
    [SerializeField]
    // シード
    private int seed;

    // ブロック
    private class Block {
        // 空ブロックの種類名
        public static string AIR_KIND = "Air";
        // ブロックの一覧
        public static Dictionary<string, GameObject> CANDIDATES = new Dictionary<string, GameObject>() {
            {"Grass", (GameObject)Resources.Load("Grass")},
        };

        // ブロックの種類名
        public string kind;
        // 表示するかどうか
        public bool isDisplay;
        public Block(string kind, bool isDisplay) {
            this.kind = kind;
            this.isDisplay = isDisplay;
        }

        // 空ブロックかどうかも含めて表示するかどうかを判定
        public bool checkDisplay() {
            return this.isDisplay && this.kind != Block.AIR_KIND;
        }
    }

    // ワールドデータ
    private List<List<List<List<List<Block>>>>> world = new List<List<List<List<List<Block>>>>>();

    void Start()
    {
        // --- ワールドを生成 ---
        // ワールドデータを生成
        for (int chunkX = 0; chunkX < chunkNum; chunkX++) {
            world.Add(new List<List<List<List<Block>>>>());
            for (int chunkZ = 0; chunkZ < chunkNum; chunkZ++) {
                world[chunkX].Add(new List<List<List<Block>>>());
                for (int x = 0; x < chunkSize; x++) {
                    world[chunkX][chunkZ].Add(new List<List<Block>>());
                    for (int z = 0; z < chunkSize; z++) {
                        world[chunkX][chunkZ][x].Add(new List<Block>());

                        float rawNoise = Mathf.PerlinNoise(
                            ((chunkSize * chunkX + x) + seed) / relief,
                            ((chunkSize * chunkZ + z) + seed) / relief
                        );
                        float noise = ((float)Math.Pow((rawNoise * 2 - 1), 3) + 1) / 2;
                        int groundY = (int)Math.Floor(height * noise);

                        for (int y = 0; y < height; y++) {
                            if (y <= groundY) {
                                world[chunkX][chunkZ][x][z].Add(new Block("Grass", true));
                            }
                            else {
                                world[chunkX][chunkZ][x][z].Add(new Block(Block.AIR_KIND, true));
                            }
                        }
                    }
                }
            }
        }

        // ワールドデータをワールドに反映
        for (int chunkX = 0; chunkX < chunkNum; chunkX++) {
            for (int chunkZ = 0; chunkZ < chunkNum; chunkZ++) {
                for (int x = 0; x < chunkSize; x++) {
                    for (int z = 0; z < chunkSize; z++) {
                        for (int y = 0; y < height; y++) {
                            var block = world[chunkX][chunkZ][x][z][y];
                            if (block.checkDisplay()) {
                                Instantiate(
                                    Block.CANDIDATES[block.kind],
                                    new Vector3(
                                        (float)chunkSize * chunkX + x,
                                        (float)y, 
                                        (float)chunkSize * chunkZ + z
                                    ),
                                    Quaternion.identity
                                );
                            }
                        }
                    }
                }
            }
        }
    }
}
