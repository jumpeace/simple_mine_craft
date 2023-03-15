using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {
    // 破壊の耐久秒数
    [SerializeField]
    private float enduranceSecond;

    // ブロック管理インスタンス
    public BlockManager manager;
    // 現在のゲーム上でのブロックの位置
    public Xyz pos;
    // 残りの破壊の耐久秒数
    private float remainingEnduranceSecond;

    // 破壊の耐久秒数を減らす
    // - 返り値: 破壊し終えたかどうか
    public bool ReduceEndurance() {
        this.remainingEnduranceSecond -= Time.deltaTime;
        if (this.remainingEnduranceSecond <= 0) {
            manager.Destroy();
            return true;
        }

        return false;
    }

    // 破壊の耐久秒数をリセットする
    public void ResetEndurance() {
        this.remainingEnduranceSecond = enduranceSecond;
    }

    void Start() {
        this.ResetEndurance();
    }
}
