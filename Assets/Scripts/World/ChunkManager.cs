using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// チャンク
public class Chunk {
    // チャンク内のブロックデータ
    public List<List<List<BlockManager>>> data;
    // チャンクの二次元位置
    public Xz pos2;
    // チャンクの大きさ
    private int size;
    // ワールドの最大高さ
    private int height;

    // ブロック生成のコールバック関数
    private BlockManager.InstanceBlockType InstanceBlock;
    // ブロック破壊のコールバック関数
    private BlockManager.DestroyBlockType DestroyBlock;

    // ワールド上の二次元位置を取得
    // - pos2InChunk: チャンク上の二次元位置
    private Xz GetPos2InWorld(Xz pos2InChunk) {
        return new Xz(
            this.size * this.pos2.x + pos2InChunk.x,
            this.size * this.pos2.z + pos2InChunk.z
        );
    }

    // ワールド上の位置を取得
    // - posInChunk: チャンク上の位置
    private Xyz GetPosInWorld(Xyz posInChunk) {
        var pos2InWorld = this.GetPos2InWorld(new Xz(posInChunk.x, posInChunk.z));
        return new Xyz(
            pos2InWorld.x,
            posInChunk.y, 
            pos2InWorld.z
        );
    }
    
    // チャンク上の位置を取得
    // - posInWorld: 世界上の位置
    private Xyz GetPosInChunk(Xyz posInWorld) {
        // 範囲内であるかを調べる
        if (posInWorld.x / this.size != this.pos2.x) return null;
        if (posInWorld.z / this.size != this.pos2.z) return null;

        // 変換処理
        var result = new Xyz(
            posInWorld.x % this.size,
            posInWorld.y,
            posInWorld.z % this.size
        );
        if (result.x < 0) result.x += this.size;
        if (result.z < 0) result.z += this.size;

        return result;
    }

    // パーリンノイズにより地面のy座標を取得
    // - pos2InChunk: チャンク上の二次元位置
    // - relief: 起伏の緩さ
    // - seed: シード値
    private int GetGroundY(Xz pos2InChunk, float relief, int seed) {
        Xz pos2InWorld = this.GetPos2InWorld(pos2InChunk);
        float rawNoise = Mathf.PerlinNoise(
            (pos2InWorld.x + seed) / relief,
            (pos2InWorld.z + seed) / relief
        );
        float noise = ((float)Math.Pow((rawNoise * 2 - 1), 3) + 1) / 2;
        return (int)Math.Floor(height * noise);
    }

    // チャンクデータを初期化
    // - relief: 起伏の緩さ
    // - seed: シード値
    // - InstanceBlock: ブロックを世界に生成する関数
    private void InitData(float relief, int seed) {
        // チャンクデータを生成
        this.data = new List<List<List<BlockManager>>>();
        for (int x = 0; x < this.size; x++) {
            this.data.Add(new List<List<BlockManager>>());
            for (int z = 0; z < this.size; z++) {
                this.data[x].Add(new List<BlockManager>());

                // 地面の高さ
                int groundY = this.GetGroundY(new Xz(x, z), relief, seed);
                for (int y = 0; y < this.height; y++) {
                    // 空ブロック以外のブロック
                    if (y <= groundY) {
                        // 生成するブロックの種類を決定
                        string blockKindName = "";
                        // 地面より3マス以上低い場合
                        if (y <= groundY - 3) {
                            blockKindName = "Stone";
                        }
                        // 地面と3マス以内の場合
                        else {
                            blockKindName = "Grass";
                        }

                        var blockManager = new BlockManager(
                            blockKindName, this.InstanceBlock, this.DestroyBlock
                        );
                        blockManager.SetGameObject(this.GetPosInWorld(new Xyz(x, y, z)));
                        this.data[x][z].Add(blockManager);
                    }
                    // 空ブロック
                    else {
                        this.data[x][z].Add(new BlockManager(
                            BlockManager.AIR_KIND_NAME, this.InstanceBlock, this.DestroyBlock
                        ));
                    }
                }
            }
        }
    }


    // 複数の木を生成
    // - relief: 起伏の緩さを転用
    // - seed: シード値
    // - treeHeight: 木の幹の高さ
    // - trees: ワールドの木の一覧
    // - InstanceBlock: ブロックを世界に生成する関数
    private void GenerateTrees(float relief, int seed, int minTreeInterval, List<Tree> trees) {
        for (int x = 0; x < this.size; x++) {
            for (int z = 0; z < this.size; z++) {
                // 木を生成する二次元位置を取得
                var pos2InChunk = new Xz(x, z);
                var pos2InWorld = this.GetPos2InWorld(pos2InChunk);

                // 木を生成するかを判定
                float rawNoise = Mathf.PerlinNoise(
                    pos2InWorld.x / relief + seed,
                    pos2InWorld.z / relief + seed
                );
                float rawNoise2 = ((float)Math.Sin((rawNoise * 2 - 1) * 90) + 1) / 2;
                float noise = ((float)Math.Pow((double)(rawNoise2 * 2 - 1), 5) + 1 / 2);
                if (!(noise >= 0.8)) continue;

                // 木の位置が近すぎないようにする
                bool existsNear = false;
                foreach (var tree in trees) {
                    // x方向を調べる
                    int xDist = (pos2InWorld.x > tree.pos.x) ? pos2InWorld.x - tree.pos.x : tree.pos.x - pos2InWorld.x; 
                    if (xDist + 1 < minTreeInterval) {
                        existsNear = true;
                        break;
                    }
                    
                    // z方向を調べる
                    int zDist = (pos2InWorld.z > tree.pos.z) ? pos2InWorld.z - tree.pos.z : tree.pos.z - pos2InWorld.z; 
                    if (zDist + 1 < minTreeInterval) {
                        existsNear = true;
                        break;
                    }
                }
                if (existsNear) continue;

                // 木のある二次元位置の柱
                var pole = this.data[pos2InChunk.x][pos2InChunk.z];

                // y座標を設定
                int treeBottomY = 0;
                for (int y = this.height - 2; y >= 0; y--) {
                    if (pole[y].CheckDisplay()) {
                        treeBottomY = y + 1;
                        break;
                    }
                }

                // 木の幹を生成
                for (int treeY = 0; treeY < Tree.stemHeight; treeY++) {
                    int y = treeBottomY + treeY;
                    if (y > this.height) break;
                    
                    var blockManager = pole[y];
                    blockManager.kindName = "Tree";
                    blockManager.SetGameObject(this.GetPosInWorld(new Xyz(pos2InChunk.x, y, pos2InChunk.z)));
                }

                trees.Add(new Tree(
                    pos2InWorld.ConvertToXyz(treeBottomY), 
                    this.pos2
                ));
            }
        }
    }

    // 木の葉を生成
    private void GenerateReefs(List<Tree> trees) {
        foreach (var tree in trees) {
            // 自身もしくは隣のチャンクかどうか調べる
            // x方向を調べる
            int xDiff = (this.pos2.x > tree.chunkPos2.x) ? this.pos2.x - tree.chunkPos2.x : tree.chunkPos2.x - this.pos2.x; 
            if (xDiff > 1) continue;
            // z方向を調べる
            int zDiff = (this.pos2.z > tree.chunkPos2.z) ? this.pos2.z - tree.chunkPos2.z : tree.chunkPos2.z - this.pos2.z; 
            if (zDiff > 1) continue;

            foreach (var reefRelativePos in Tree.reefRelativePoses) {
                var reefPosInWorld = tree.pos.Added(reefRelativePos);
                var reefPosInChunk = this.GetPosInChunk(reefPosInWorld);
                if (reefPosInChunk == null) continue;
                
                var blockManager = this.data[reefPosInChunk.x][reefPosInChunk.z][reefPosInChunk.y];
                blockManager.kindName = "Reef";
                blockManager.SetGameObject(reefPosInWorld);
            }
        }
    }

    // - relief: 起伏の緩さ
    // - seed: シード値
    // - minTreeInterval: 木の最小間隔
    // - trees: ワールドの木の一覧
    // - InstanceBlock: ブロックを世界に生成する関数
    public Chunk(
            Xz pos2, int size, int height, float relief, int seed, 
            int minTreeInterval, List<Tree> trees, 
            BlockManager.InstanceBlockType InstanceBlock, BlockManager.DestroyBlockType DestroyBlock
        ) {
        this.pos2 = pos2;
        this.size = size;
        this.height = height;
        this.InstanceBlock = InstanceBlock;
        this.DestroyBlock = DestroyBlock;

        this.InitData(relief, seed);
        this.GenerateTrees(relief, seed, minTreeInterval, trees);
        this.GenerateReefs(trees);
    }

    // チャンク内のゲームオブジェクトを表示/非表示にする
    public void SetActive(bool isToActive) {
        for (int x = 0; x < this.size; x++) {
            for (int z = 0; z < this.size; z++) {
                for (int y = 0; y < this.height; y++) {
                    var blockManager = this.data[x][z][y];
                    if (blockManager.CheckDisplay()) {
                        blockManager.gameObject.SetActive(isToActive);
                    }
                }
            }
        }
    }
    
    // // チャンク内のブロックを壊す
    // public void DestroyBlock(Block block, BlockManager.DestroyBlockType DestroyBlock) {
    //     var posInChunk = this.GetPosInChunk(block.pos);
    //     this.data[posInChunk.x][posInChunk.z][posInChunk.z].DestroyGameObject(block, DestroyBlock);
    // }
}
