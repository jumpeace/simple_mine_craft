using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // 垂直方向の回転速度
    [SerializeField]
    private float verticalRotateSpeed = 120f;

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
    }
}
