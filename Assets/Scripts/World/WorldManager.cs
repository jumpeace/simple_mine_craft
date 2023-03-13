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
    // 現在のチャンクの位置
    private Vector3 nowChunkPos;

    // ブロック生成のコールバック関数
    // - blockKind: ブロックの種類
    // - posInWorld: ワールド上の位置
    GameObject InstanceBlock(Block block, Vector3 posInWorld) {
        return Instantiate(
            Block.KINDS.Find(kind => block.kindName == kind.name).gameObject,
            posInWorld,
            Quaternion.identity
        );
    }

    // ワールドを生成
    void GenerateWorld() {
        // ワールドデータを生成し, ゲームに反映
        this.data = new List<Chunk>();
        for (int chunkX = -this.visibleChunkNum; chunkX <= this.visibleChunkNum; chunkX++) {
            for (int chunkZ = -this.visibleChunkNum; chunkZ <= this.visibleChunkNum; chunkZ++) {
                this.data.Add(new Chunk(
                    new Vector3(chunkX, 0f, chunkZ),
                    this.chunkSize,
                    this.height,
                    this.relief,
                    this.seed,
                    this.InstanceBlock
                ));
            }
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
            if (targetPole[y].CheckDisplay()) {
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

    // チャンクのリロード
    void ReloadChunk() {
        var nextChunkPos = GetChunkPosWithPlayer();
        if (nextChunkPos.Equals(this.nowChunkPos)) return;

        // 現在の描画チャンク位置の一覧
        var nowVisibleChunkPoses = new List<Vector3>();
        // 次の描画チャンク位置の一覧
        var nextVisibleChunkPoses = new List<Vector3>();
        for (int wx = -1; wx <= 1; wx++) {
            for (int wz = -1; wz <= 1; wz++) {
                nowVisibleChunkPoses.Add(
                    new Vector3(this.nowChunkPos.x + wx, 0f, this.nowChunkPos.z + wz)
                );
                nextVisibleChunkPoses.Add(
                    new Vector3(nextChunkPos.x + wx, 0f, nextChunkPos.z + wz)
                );
            }
        }

        // 非表示にするチャンクを非表示にする
        foreach (Vector3 nowVisibleChunkPos in nowVisibleChunkPoses) {
            if (!nextVisibleChunkPoses.Contains(nowVisibleChunkPos)) {
                var toInActiveChunk = this.data.Find(
                    chunk => chunk.pos == nowVisibleChunkPos
                );
                toInActiveChunk.SetActive(false);
            }
        }
        
        // 表示にするチャンクを表示にする
        foreach (Vector3 nextVisibleChunkPos in nextVisibleChunkPoses) {
            if (!nowVisibleChunkPoses.Contains(nextVisibleChunkPos)) {
                var toActiveChunk = this.data.Find(
                    chunk => chunk.pos == nextVisibleChunkPos
                );
                // 新しくチャンクを生成する場合
                if (toActiveChunk == null) {
                    this.data.Add(new Chunk(
                        new Vector3(nextVisibleChunkPos.x, 0f, nextVisibleChunkPos.z),
                        this.chunkSize,
                        this.height,
                        this.relief,
                        this.seed,
                        InstanceBlock
                    ));
                }
                // 既存のチャンクを表示させる場合
                else {
                    toActiveChunk.SetActive(true);
                }
            }
        }

        this.nowChunkPos = nextChunkPos;
    }

    void Update() {
        this.ReloadChunk();
    }
}
