using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    // チャンクの大きさ
    [SerializeField]
    private int chunkSize = 12;
    // x, z方向のチャンク数
    [SerializeField]
    private int chunkNum = 2;
    // フィールドの垂直方向の大きさ
    [SerializeField]
    private int height = 128;
    [SerializeField]
    // 起伏の緩さ
    private float relief = 15f;
    [SerializeField]
    // シード
    private int seed = 2000;
    
    // ワールドデータ
    private List<Chunk> data;

    // ブロック生成のコールバック関数
    // - blockKind: ブロックの種類
    // - posInWorld: ワールド上の位置
    void InstanceBlock(Block block, Vector3 posInWorld) {
        Instantiate(
            Block.CANDIDATES[block.kind],
            posInWorld,
            Quaternion.identity
        );
    }

    void Start(){
        // --- ワールドを生成 ---
        // ワールドデータを生成
        this.data = new List<Chunk>();
        for (int chunkX = 0; chunkX < this.chunkNum; chunkX++) {
            for (int chunkZ = 0; chunkZ < this.chunkNum; chunkZ++) {
                this.data.Add(new Chunk(
                    new Vector3(chunkX, 0f, chunkZ),
                    this.chunkSize,
                    this.height,
                    this.relief,
                    this.seed
                ));
            }
        }
        
        // ワールドデータをワールドに反映
        for (int i = 0; i < this.data.Count; i++) {
            this.data[i].Reflect(this.InstanceBlock);
        }
    }

    Vector3 GetChunkPosWithPlayer() {
        var player = GameObject.FindGameObjectWithTag("Player");
        return new Vector3(
            (float)Math.Floor(player.transform.position.x / this.chunkSize),
            0f,
            (float)Math.Floor(player.transform.position.z / this.chunkSize)
        );
    }

    void Update() {
        var chunkPos = GetChunkPosWithPlayer();
    }
}
