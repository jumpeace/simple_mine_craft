using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ブロック
public class Block {
    // 空ブロックの種類名
    public static string AIR_KIND_NAME = "Air";
    // ブロックの一覧
    public static List<BlockKind> KINDS = new List<BlockKind>() {
        new BlockKind("Grass", (GameObject)Resources.Load("Grass"))
    };

    // ブロックの種類名
    public string kindName;
    // ブロックのゲームオブジェクト
    public GameObject gameObject;

    public Block(string kindName) {
        this.kindName = kindName;
    }

    // ブロックのゲームオブジェクトを設定する
    public void SetGameObject(GameObject gameObject) {
        this.gameObject = gameObject;
    }

    // 空ブロックかどうかにより表示するかどうかを判定
    public bool CheckDisplay() {
        return this.kindName != Block.AIR_KIND_NAME;
    }
}
