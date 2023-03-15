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
    // シード
    [SerializeField]
    private int seed = 2000;
    // 木の最小間隔
    [SerializeField]
    private int minTreeInterval = 4;
    
    // ワールドデータ
    private List<Chunk> data;
    // 木の二次元位置
    private List<Tree> trees;
    // 現在のチャンクの二次元位置
    private Xz nowChunkPos2;

    // ブロック生成のコールバック関数
    // - blockKind: ブロックの種類
    // - posInWorld: ワールド上の位置
    GameObject InstanceBlock(BlockManager blockManager, Xyz posInWorld) {
        return Instantiate(
            BlockManager.KINDS.Find(kind => blockManager.kindName == kind.name).gameObject,
            posInWorld.ConvertToVector3(),
            Quaternion.identity
        );
    }
    
    // ブロック破壊のコールバック関数
    // - blockKind: ブロックの種類
    // - posInWorld: ワールド上の位置
    void DestroyBlock(BlockManager blockManager) {
        Destroy(blockManager.gameObject);
    }

    // ワールドを生成
    void GenerateWorld() {
        // 初期チャンクを設定
        this.nowChunkPos2 = new Xz(0, 0);

        // ワールドデータを生成し, ゲームに反映
        this.data = new List<Chunk>();
        this.trees = new List<Tree>();
        for (int chunkWx = -this.visibleChunkNum; chunkWx <= this.visibleChunkNum; chunkWx++) {
            for (int chunkWz = -this.visibleChunkNum; chunkWz <= this.visibleChunkNum; chunkWz++) {
                this.data.Add(new Chunk(
                    new Xz(nowChunkPos2.x + chunkWx, nowChunkPos2.z + chunkWz),
                    this.chunkSize,
                    this.height,
                    this.relief,
                    this.seed,
                    this.minTreeInterval,
                    this.trees,
                    InstanceBlock,
                    DestroyBlock
                ));
            }
        }
        
    }

    // プレイヤーを地面の上に配置する
    void ArrangePlayerOnTheGround() {
        // x, z座標を設定
        int playerPosXZ = (int)(Math.Ceiling((double)(chunkSize + 1) / 2) - 1);
        var playerPosTmp = new Xz(playerPosXZ, playerPosXZ);

        // プレイヤーが立つ柱を取得
        var targetPole = this.data.Find(
            chunk => chunk.pos2.Equals(new Xz(0, 0))
        ).data[playerPosTmp.x][playerPosTmp.z];

        // Vector3型に変換
        var playerPos = new Vector3();
        playerPos.x = playerPosTmp.x;
        playerPos.z = playerPosTmp.z;

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

    Xz GetChunkPosWithPlayer() {
        var player = GameObject.FindGameObjectWithTag("Player");
        return new Xz(
            (int)Math.Round((double)player.transform.position.x) / this.chunkSize,
            (int)Math.Round((double)player.transform.position.z) / this.chunkSize
        );
    }

    // チャンクのリロード
    void ReloadChunk() {
        var nextChunkPos2 = GetChunkPosWithPlayer();
        if (nextChunkPos2.Equals(this.nowChunkPos2)) return;

        // 現在の描画チャンク位置の一覧
        var nowVisibleChunkPos2s = new List<Xz>();
        // 次の描画チャンク位置の一覧
        var nextVisibleChunkPos2s = new List<Xz>();
        for (int wx = -1; wx <= 1; wx++) {
            for (int wz = -1; wz <= 1; wz++) {
                nowVisibleChunkPos2s.Add(
                    new Xz(this.nowChunkPos2.x + wx, this.nowChunkPos2.z + wz)
                );
                nextVisibleChunkPos2s.Add(
                    new Xz(nextChunkPos2.x + wx, nextChunkPos2.z + wz)
                );
            }
        }

        // 非表示にするチャンクを非表示にする
        foreach (var nowVisibleChunkPos2 in nowVisibleChunkPos2s) {
            if (!nextVisibleChunkPos2s.Contains(nowVisibleChunkPos2)) {
                var toInActiveChunk = this.data.Find(
                    chunk => chunk.pos2.Equals(nowVisibleChunkPos2)
                );
                toInActiveChunk.SetActive(false);
            }
        }
        
        // 表示にするチャンクを表示にする
        foreach (var nextVisibleChunkPos2 in nextVisibleChunkPos2s) {
            if (!nowVisibleChunkPos2s.Contains(nextVisibleChunkPos2)) {
                var toActiveChunk = this.data.Find(
                    chunk => chunk.pos2.Equals(nextVisibleChunkPos2)
                );
                // 新しくチャンクを生成する場合
                if (toActiveChunk == null) {
                    this.data.Add(new Chunk(
                        nextVisibleChunkPos2,
                        this.chunkSize,
                        this.height,
                        this.relief,
                        this.seed,
                        this.minTreeInterval,
                        this.trees,
                        InstanceBlock,
                        DestroyBlock
                    ));
                }
                // 既存のチャンクを表示させる場合
                else {
                    toActiveChunk.SetActive(true);
                }
            }
        }

        this.nowChunkPos2 = nextChunkPos2;
    }

    void Update() {
        this.ReloadChunk();
    }

    // ワールド内にブロックを設置する
    // 返り値: ブロックを設置した場合はそのブロックの管理コンポーネント
    public BlockManager InstallBlock(Stock stock, Xyz pos) {
        Xz chunkPos2 = new Xz(
            pos.x / this.chunkSize,
            pos.z / this.chunkSize
        );
        foreach (var chunk in this.data) {
            if (chunk.pos2.Equals(chunkPos2)) {
                return chunk.InstallBlock(stock, pos);
            }
        }
        return null;
    }
}
