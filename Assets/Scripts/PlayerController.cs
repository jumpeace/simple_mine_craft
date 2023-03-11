using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // 移動スピード
    [SerializeField]
    private float transitionSpeed = 6f;
    // 水平方向の回転速度
    [SerializeField]
    private float horizonRotateSpeed = 180f;
    // ジャンプ力
    [SerializeField]
    private float jumpPower = 4f;

    // 物理演算用のコンポーネント
    private Rigidbody rb;
    // ジャンプしているかどうか
    private bool isJumping = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // --- 移動 ---
        // 前方移動
        if (Input.GetKey(KeyCode.W)) {
            transform.position += transitionSpeed * transform.forward * Time.deltaTime;
        }
        // 後方移動
        if (Input.GetKey(KeyCode.S)) {
            transform.position -= transitionSpeed * transform.forward * Time.deltaTime;
        }
        // 右移動
        if (Input.GetKey(KeyCode.D)) {
            transform.position += transitionSpeed * transform.right * Time.deltaTime;
        }
        // 左移動
        if (Input.GetKey(KeyCode.A)) {
            transform.position -= transitionSpeed * transform.right * Time.deltaTime;
        }

        // --- 水平方向の回転 ---
        // 左回転
        if (Input.GetKey(KeyCode.LeftArrow)) {
            transform.Rotate(0f, -horizonRotateSpeed * Time.deltaTime, 0f);
        }
        // 右回転
        if (Input.GetKey(KeyCode.RightArrow)) {
            transform.Rotate(0f, horizonRotateSpeed * Time.deltaTime, 0f);
        }

        // ジャンプによるグラつきをなくす
        if (transform.eulerAngles.x != 0 || transform.eulerAngles.z != 0) {
            transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
        }
    }

    void FixedUpdate() {
        // --- ジャンプ ---
        if (Input.GetKey(KeyCode.Space) && !isJumping) {
            rb.velocity = transform.up * jumpPower;
            isJumping = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 地面に着地したら, ジャンプしていないことにする
        if(collision.gameObject.CompareTag("Block"))
        {
            isJumping = false;
        }
    }
}
