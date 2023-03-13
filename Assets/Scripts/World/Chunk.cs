using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// チャンク
public class Chunk {
    // チャンク内のブロックデータ
    public List<List<List<Block>>> data;
    // チャンクの二次元位置（y座標は使わない）
    public Vector3 pos;
    // チャンクの大きさ
    private int size;
    // ワールドの最大高さ
    private int height;
    
    // ブロック生成のコールバック関数
    public delegate GameObject InstanceBlock(Block block, Vector3 posInWorld);

    // ワールド上の位置
    // posInChunk: チャンク上の位置
    // - blockKind: ブロックの種類
    // - posInWorld: ワールド上の位置
    private Vector3 GetPosInWorld(Vector3 posInChunk) {
        return new Vector3(
            (float)this.size * this.pos.x + posInChunk.x,
            (float)posInChunk.y, 
            (float)this.size * this.pos.z + posInChunk.z
        );
    }

    // パーリンノイズにより地面のy座標を取得
    // - posInChunk: チャンク上の二次元位置
    // - relief: 起伏の緩さ
    // - seed: シード値
    private int GetGroundY(Vector3 posInChunk, float relief, int seed) {
        Vector3 posInWorld = this.GetPosInWorld(posInChunk);
        float rawNoise = Mathf.PerlinNoise(
            (posInWorld.x + seed) / relief,
            (posInWorld.z + seed) / relief
        );
        float noise = ((float)Math.Pow((rawNoise * 2 - 1), 3) + 1) / 2;
        return (int)Math.Floor(height * noise);
    }

    // チャンクデータを初期化
    // - relief: 起伏の緩さ
    // - seed: シード値
    private void InitData(float relief, int seed, InstanceBlock InstanceBlockCallback) {
        // チャンクデータを生成
        this.data = new List<List<List<Block>>>();
        for (int x = 0; x < this.size; x++) {
            this.data.Add(new List<List<Block>>());
            for (int z = 0; z < this.size; z++) {
                this.data[x].Add(new List<Block>());
                // 地面の高さ
                int groundY = this.GetGroundY(new Vector3(x, 0f, z), relief, seed);
                for (int y = 0; y < this.height; y++) {
                    // 空ブロック以外のブロック
                    if (y <= groundY) {
                        var block = new Block("Grass");
                        block.SetGameObject(
                            InstanceBlockCallback(block, this.GetPosInWorld(new Vector3(x, y, z)))
                        );
                        this.data[x][z].Add(block);
                    }
                    // 空ブロック
                    else {
                        this.data[x][z].Add(new Block(Block.AIR_KIND_NAME));
                    }
                }
            }
        }
    }

    // - relief: 起伏の緩さ
    // - seed: シード値
    public Chunk(Vector3 pos, int size, int height, float relief, int seed, InstanceBlock InstanceBlockCallback) {
        this.pos = pos;
        this.size = size;
        this.height = height;

        this.InitData(relief, seed, InstanceBlockCallback);
    }

    // チャンク内のゲームオブジェクトを表示/非表示にする
    public void SetActive(bool isToActive) {
        for (int x = 0; x < this.size; x++) {
            for (int z = 0; z < this.size; z++) {
                for (int y = 0; y < this.height; y++) {
                    var block = this.data[x][z][y];
                    if (block.gameObject != null) {
                        block.gameObject.SetActive(isToActive);
                    }
                }
            }
        }
    }
}
