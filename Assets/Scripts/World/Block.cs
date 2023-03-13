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

    public Block(string kind) {
        this.kind = kind;
    }

    // 空ブロックかどうかにより表示するかどうかを判定
    public bool checkDisplay() {
        return this.kind != Block.AIR_KIND;
    }
}
