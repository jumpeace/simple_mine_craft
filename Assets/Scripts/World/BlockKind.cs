using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockKind {
    // 種類名
    public string name;
    // プリハブ化したゲームオブジェクト
    public GameObject gameObject;

    public BlockKind(string name) {
        this.name = name;
        this.gameObject = (GameObject)Resources.Load($"Block/{name}");
    }
}
