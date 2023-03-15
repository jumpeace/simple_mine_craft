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
    // ブロックの位置
    public Xyz pos;
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
    public void UpdateGameObject() {
        this.gameObject = this.InstanceBlock(this, this.pos);

        this.block = gameObject.GetComponent<Block>();
        this.block.manager = this;
        this.block.pos = this.pos;
    }

    // 空ブロックかどうかにより表示するかどうかを判定
    public bool CheckDisplay() {
        return this.kindName != BlockManager.AIR_KIND_NAME;
    }

    public BlockManager(string kindName, Xyz pos, InstanceBlockType InstanceBlock, DestroyBlockType DestroyBlock) {
        this.kindName = kindName;
        this.pos = pos;
        this.InstanceBlock = InstanceBlock;
        this.DestroyBlock = DestroyBlock;

        if (this.CheckDisplay()) this.UpdateGameObject();
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

    // ブロックを設置する
    // 返り値: ブロックを設置した場合はそのブロックの管理コンポーネント
    public BlockManager Install(Stock stock) {
        if (this.CheckDisplay()) {
            return null;
        }

        // ブロックの種類を設定
        this.kindName = stock.kindName;
        // ブロックの持ち物を減らす
        bool canDecrement = stock.Decrement();
        if (!canDecrement) {
            this.kindName = BlockManager.AIR_KIND_NAME;
            return null;
        }

        // ブロックを設置
        this.UpdateGameObject();

        return this;
    }
}
