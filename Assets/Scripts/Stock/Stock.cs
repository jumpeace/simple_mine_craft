using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Stock : MonoBehaviour{
    // ブロックの種類名
    [SerializeField]
    private string kindName;

    // ストック数のテキストコンポーネント
    [SerializeField]
    private TextMeshProUGUI stockCountText;
    
    // ストック数
    private int count = 0;

    // 任意のブロックに対応するストックかどうかを判定
    public bool CheckApplyBlock(Block block) {
        return block.manager.kindName == this.kindName;
    }

    // ストック数を設定
    // - 返り値: 変更したかどうか
    private bool SetCount(int nextCount) {
        if (nextCount < 0) return false;

        this.count = nextCount;
        this.stockCountText.text = this.count.ToString();
        return true;
    }

    // ストック数を1増やす
    // - 返り値: 変更したかどうか
    public bool Increment() {
        return this.SetCount(this.count + 1);
    }
    
    // ストック数を1減らす
    // - 返り値: 変更したかどうか
    public bool Decrement() {
        return this.SetCount(this.count - 1);
    }

    void Start() {
        bool result = this.SetCount(0);
    }
}
