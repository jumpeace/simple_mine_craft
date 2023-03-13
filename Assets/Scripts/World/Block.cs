using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ブロック
public class Block {
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
