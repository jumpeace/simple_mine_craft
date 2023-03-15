using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Stock : MonoBehaviour{
    // ストック番号とキーコードの対応表
    static private Dictionary<int, KeyCode> indexToKeyCode = new Dictionary<int, KeyCode>() {
        {1, KeyCode.Alpha1},
        {2, KeyCode.Alpha2},
        {3, KeyCode.Alpha3},
        {4, KeyCode.Alpha4}
    };
    // 選択されていない場合の色
    static private string notSelectedColor = "white";
    // 選択されている場合の色
    static private string selectedColor = "red";

    // ブロックの種類名
    [SerializeField]
    public string kindName;
    // ストック番号
    [SerializeField]
    private int index;
    // ストック数のテキストコンポーネント
    [SerializeField]
    private TextMeshProUGUI stockCountText;
    
    // 選択されているかどうか
    public bool isSelected = false;
    // ストック数
    private int count = 0;
    // テキストの色
    private string color = Stock.notSelectedColor;

    // 任意のブロックに対応するストックかどうかを判定
    public bool CheckApplyBlock(Block block) {
        return block.manager.kindName == this.kindName;
    }

    // テキストを更新
    private void UpdateText() {
        this.stockCountText.text = $"<color=\"{this.color}\">{this.count}</color>";
    }

    // 現在のストック数が0個であるかを判定する
    public bool CheckCountZero() {
        return this.count == 0;
    }

    // ストック数を設定
    // - 返り値: 変更したかどうか
    private bool SetCount(int nextCount) {
        if (nextCount < 0) return false;

        this.count = nextCount;
        this.UpdateText();
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

    void Update() {
        // 選択されているかを判定
        foreach (KeyValuePair<int, KeyCode> item in Stock.indexToKeyCode) {
            if (Input.GetKey(item.Value)) {
                isSelected = item.Key == this.index;
                break;
            }
        }

        // 選択されているかによって, 色を変更
        string prevColor = this.color;
        this.color = isSelected ? Stock.selectedColor : Stock.notSelectedColor;
        // 色が変わった場合のみ, テキストを更新する
        if (this.color != prevColor) this.UpdateText();
    }
}
