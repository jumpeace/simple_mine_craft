using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ブロック
public class BlockManager {
    // 空ブロックの種類名
    public static string AIR_KIND_NAME = "Air";
    // ブロックの一覧
    public static List<BlockKind> KINDS = new List<BlockKind>() {
        new BlockKind("Grass"),
        new BlockKind("Stone"),
        new BlockKind("Tree"),
        new BlockKind("Reef"),
    };

    // ブロックの種類名
    public string kindName;
    // ブロックのゲームオブジェクト
    public GameObject gameObject;
    // ブロックコンポーネント
    private Block block;
    
    // ブロック生成のコールバック関数
    public delegate GameObject InstanceBlockType(BlockManager blockManager, Xyz posInWorld);
    private InstanceBlockType InstanceBlock;
    // ブロック破壊のコールバック関数
    public delegate void DestroyBlockType(BlockManager blockManager);
    private DestroyBlockType DestroyBlock;
    
    // ブロックのゲームオブジェクトを設定する
    public void SetGameObject(Xyz pos) {
        this.gameObject = this.InstanceBlock(this, pos);

        this.block = gameObject.GetComponent<Block>();
        this.block.manager = this;
        this.block.pos = pos;
    }

    public BlockManager(string kindName, InstanceBlockType InstanceBlock, DestroyBlockType DestroyBlock) {
        this.kindName = kindName;
        this.InstanceBlock = InstanceBlock;
        this.DestroyBlock = DestroyBlock;
    }

    // 空ブロックかどうかにより表示するかどうかを判定
    public bool CheckDisplay() {
        return this.kindName != BlockManager.AIR_KIND_NAME;
    }

    private Stock GetStock() {
        foreach (GameObject stockObject in GameObject.FindGameObjectsWithTag("Stock")) {
            Stock stock = stockObject.GetComponent<Stock>();
            if (stock.CheckApplyBlock(this.block)) {
                return stock;
            }
        }
        
        return null;
    }
    
    // ブロックを破壊する
    public void Destroy() {
        // ブロックを破壊
        this.DestroyBlock(this);
        // ブロックの持ち物を増やす
        this.GetStock().Increment();
        // ブロックの種類を空ブロックに変更する
        this.kindName = BlockManager.AIR_KIND_NAME;
    }
}
