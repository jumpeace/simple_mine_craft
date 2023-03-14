using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Stock : MonoBehaviour{
    // ストックを表示するゲームオブジェクト
    [SerializeField]
    private GameObject stockCountObject;

    // ストック数
    private uint count;
    // ストックを表示するゲームオブジェクトのテキスト
    private TextMeshProUGUI stockCountText;

    // ストック数を設定
    private void SetCount(uint count) {
        this.count = count;
        this.stockCountText.text = count.ToString();
    }

    // ストック数を1増やす
    // - 返り値: 変更したかどうか
    public bool Increment() {
        this.count++;

        return true;
    }
    
    // ストック数を1減らす
    // - 返り値: 変更したかどうか
    public bool Decrement() {
        if (this.count == 0) return false;

        this.count--;
        return true;
    }

    void Start() {
        this.stockCountText = this.stockCountObject.GetComponent<TextMeshProUGUI>();
        this.SetCount(0);
    }
}
