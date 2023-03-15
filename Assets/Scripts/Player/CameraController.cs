using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    // 垂直方向の回転速度
    [SerializeField]
    private float verticalRotateSpeed = 120f;
    // 手の届く距離
    [SerializeField]
    private float reachDist = 10f;
    
    // 左マウスをクリックしたままか
    private bool isNowLeftMouseDown = false;
    // 以前破壊していたブロックのBlockコンポーネント
    private Block nowDestroyingBlock = null;

    // ブロックを破壊する
    private void DestroyBlock() {
        if (Input.GetMouseButtonDown(0)) 
            this.isNowLeftMouseDown = true;
        else if (Input.GetMouseButtonUp(0)) 
            this.isNowLeftMouseDown = false;
        
        if (this.isNowLeftMouseDown) {
            // 左マウスをクリックしたままの場合
            if (this.nowDestroyingBlock == null) {
                // 現在破壊しているブロックがない場合
                var ray = new Ray(this.transform.position, this.transform.forward);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, this.reachDist)) {
                    GameObject hitGameObject = hit.collider.gameObject;
                    if (hitGameObject.tag == "Block") {
                        // 視線の先にブロックが見つかった場合は破壊を開始する
                        this.nowDestroyingBlock = hitGameObject.GetComponent<Block>();
                    }
                }
            }
            else {
                // 現在破壊しているブロックがある場合は, そのブロックを破壊する
                bool doneDestroyed = this.nowDestroyingBlock.ReduceEndurance();
                if (doneDestroyed) {
                    // 破壊し終えた場合は現在破壊しているブロックをなしにする
                    this.nowDestroyingBlock = null;
                }
            }
        }
        else {
            // 左マウスをクリックしたままでない場合
            if (this.nowDestroyingBlock != null) {
                // 現在破壊しているブロックがある場合は, 破壊をやめる
                this.nowDestroyingBlock.ResetEndurance();
                this.nowDestroyingBlock = null;
            }
        }
    }

    void Update()
    {
        // --- 垂直方向の回転 ---
        // 回転角度の絶対値
        float rotateAngleSize = this.verticalRotateSpeed * Time.deltaTime;

        // 上回転
        if (Input.GetKey(KeyCode.UpArrow)) {
            this.transform.Rotate(-rotateAngleSize, 0f, 0f);
            // 後ろまで回転しないようにする
            if (this.transform.localEulerAngles.z != 0) {
                this.transform.Rotate(rotateAngleSize, 0f, 0f);
            }
        }
        // 下回転
        if (Input.GetKey(KeyCode.DownArrow)) {
            this.transform.Rotate(rotateAngleSize, 0f, 0f);
            // 後ろまで回転しないようにする
            if (this.transform.localEulerAngles.z != 0) {
                this.transform.Rotate(-rotateAngleSize, 0f, 0f);
            }
        }

        // --- ブロックの操作 ---
        this.DestroyBlock();
    }
}
