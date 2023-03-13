using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    // チャンクの大きさ
    [SerializeField]
    private int chunkSize = 12;
    // 任意の方向の見えるチャンク数
    [SerializeField]
    private int visibleChunkNum = 1;
    // フィールドの垂直方向の大きさ
    [SerializeField]
    private int height = 64;
    [SerializeField]
    // 起伏の緩さ
    private float relief = 31f;
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

    // ワールドを生成
    void GenerateWorld() {
        // ワールドデータを生成
        this.data = new List<Chunk>();
        for (int chunkX = -this.visibleChunkNum; chunkX <= this.visibleChunkNum; chunkX++) {
            for (int chunkZ = -this.visibleChunkNum; chunkZ <= this.visibleChunkNum; chunkZ++) {
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

    // プレイヤーを地面の上に配置する
    void ArrangePlayerOnTheGround() {
        var playerPos = new Vector3();

        // x, z座標を設定
        float playerPosXZ = (float)(Math.Ceiling((double)(chunkSize + 1) / 2) - 1);
        playerPos.x = playerPosXZ;
        playerPos.z = playerPosXZ;

        // プレイヤーが立つ柱を取得
        var targetPole = this.data.Find(
            chunk => chunk.pos.x == 0 && chunk.pos.z == 0
        ).data[(int)playerPos.x][(int)playerPos.z];

        // y座標を設定
        playerPos.y = 0f;
        for (int y = this.height - 1; y >= 0; y--) {
            if (targetPole[y].checkDisplay()) {
                playerPos.y = (float)y + 1.5f;
                break;
            }
        }

        var player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = playerPos;
    }

    void Start(){
        GenerateWorld();
        ArrangePlayerOnTheGround();
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
        // TODO プレイヤーがチャンクを移動したら, 描画するチャンクを更新
    }
}
