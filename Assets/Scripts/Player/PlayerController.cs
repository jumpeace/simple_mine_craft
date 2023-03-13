using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // 移動スピード
    [SerializeField]
    private float transitionSpeed = 5f;
    // 水平方向の回転速度
    [SerializeField]
    private float horizonRotateSpeed = 120f;
    // ジャンプ力
    [SerializeField]
    private float jumpPower = 4f;

    // 物理演算用のコンポーネント
    private Rigidbody rb;
    // ジャンプしているかどうか
    private bool isJumping = false;

    void Start()
    {
        this.rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // --- 移動 ---
        // 前方移動
        if (Input.GetKey(KeyCode.W)) {
            this.transform.position += this.transitionSpeed * this.transform.forward * Time.deltaTime;
        }
        // 後方移動
        if (Input.GetKey(KeyCode.S)) {
            this.transform.position -= this.transitionSpeed * this.transform.forward * Time.deltaTime;
        }
        // 右移動
        if (Input.GetKey(KeyCode.D)) {
            this.transform.position += this.transitionSpeed * this.transform.right * Time.deltaTime;
        }
        // 左移動
        if (Input.GetKey(KeyCode.A)) {
            this.transform.position -= this.transitionSpeed * this.transform.right * Time.deltaTime;
        }

        // --- 水平方向の回転 ---
        // 左回転
        if (Input.GetKey(KeyCode.LeftArrow)) {
            this.transform.Rotate(0f, -this.horizonRotateSpeed * Time.deltaTime, 0f);
        }
        // 右回転
        if (Input.GetKey(KeyCode.RightArrow)) {
            this.transform.Rotate(0f, this.horizonRotateSpeed * Time.deltaTime, 0f);
        }

        // ジャンプによるグラつきをなくす
        if (this.transform.eulerAngles.x != 0f || this.transform.eulerAngles.z != 0f) {
            this.transform.rotation = Quaternion.Euler(0f, this.transform.eulerAngles.y, 0f);
        }
        // 物理演算の水平方向の速度を無視
        if (this.rb.velocity.x != 0f || this.rb.velocity.z != 0f) {
            this.rb.velocity = new Vector3(0f, this.rb.velocity.y, 0f);
        }
        // 物理演算の角速度を無視
        if ((this.rb.angularVelocity.x != 0f || this.rb.angularVelocity.y != 0f) || this.rb.angularVelocity.z != 0f) {
            this.rb.angularVelocity = new Vector3(0f, 0f, 0f);
        }
    }

    void FixedUpdate() {
        // --- ジャンプ ---
        if (Input.GetKey(KeyCode.Space) && !isJumping) {
            this.rb.velocity = this.transform.up * this.jumpPower;
            this.isJumping = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 地面に着地したら, ジャンプしていないことにする
        if(collision.gameObject.CompareTag("Block")) {
            this.isJumping = false;
        }
    }
}
