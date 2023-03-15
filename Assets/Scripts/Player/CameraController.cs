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
    
    // ワールドの管理コンポーネント
    private WorldManager worldManager;
    // マウスを左クリックしたままか
    private bool isNowMouseLeftDown = false;
    // なぜか最初にマウスの左クリックが検知されるので, 最初の左クリックは除外する
    private bool isAlreadyFirstMouseLeftDown = false;
    // 以前破壊していたブロックのBlockコンポーネント
    private Block nowDestroyingBlock = null;

    void Start() {
        // ワールドの管理コンポーネントを読み込む
        GameObject world = GameObject.FindGameObjectWithTag("World");
        this.worldManager = world.GetComponent<WorldManager>();
    }

    // ブロックを破壊する
    private void DestroyBlock() {
        // マウスを左クリックしたままかを判定する
        if (Input.GetMouseButtonDown(0)) {
            // 最初のマウスを左クリックを無視
            if (!this.isAlreadyFirstMouseLeftDown) {
                this.isAlreadyFirstMouseLeftDown = true;
            }
            else {
                this.isNowMouseLeftDown = true;
            }
        }
        else if (Input.GetMouseButtonUp(0)) {
            this.isNowMouseLeftDown = false;
        }
        
        if (this.isNowMouseLeftDown) {
            // マウスを左クリックしたままの場合
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
            // マウスを左クリックしたままでない場合
            if (this.nowDestroyingBlock != null) {
                // 現在破壊しているブロックがある場合は, 破壊をやめる
                this.nowDestroyingBlock.ResetEndurance();
                this.nowDestroyingBlock = null;
            }
        }
    }

    // ブロックを設置する
    private void InstallBlock() {
        if (Input.GetMouseButtonDown(1)) {
            // 左マウスを右クリックした場合
            // どのブロックを置くかを決定する
            var stockObjects = GameObject.FindGameObjectsWithTag("Stock");
            Stock selectedStock = null;
            foreach (var stockObject in stockObjects) {
                Stock stock = stockObject.GetComponent<Stock>();
                if (stock.isSelected) {
                    // 選択されているストックがあった場合
                    selectedStock = stock;
                    break;
                }
            }
            
            // ストック数が0個でない場合
            if (!selectedStock.CheckCountZero()) {
                // 選択されているストックがある場合
                if (selectedStock != null) {
                    var ray1 = new Ray(this.transform.position, this.transform.forward);
                    RaycastHit hit1;
                    if (Physics.Raycast(ray1, out hit1, this.reachDist)) {
                        GameObject hitGameObject1 = hit1.collider.gameObject;
                        if (hitGameObject1.tag == "Block") {
                            Block nextToInstallingBlock = hitGameObject1.GetComponent<Block>();

                            var blockManagers = new List<BlockManager>();
                            // x, y, z方向それぞれの対応する方向に置いてみる
                            // 3個多い状態で始める(あとで減らす)
                            for (int i = 0; i < 3; i++) selectedStock.Increment();

                            // x方向
                            if (this.transform.forward.x != 0f) {
                                int way = (this.transform.forward.x > 0f) ? -1 : 1;
                                var bm = this.worldManager.InstallBlock(
                                    selectedStock,
                                    new Xyz(
                                        nextToInstallingBlock.pos.x + way,
                                        nextToInstallingBlock.pos.y,
                                        nextToInstallingBlock.pos.z
                                    )
                                );
                                if (bm != null) blockManagers.Add(bm);
                            }
                            // y方向
                            if (this.transform.forward.y != 0f) {
                                int way = (this.transform.forward.y > 0f) ? -1 : 1;
                                var bm = this.worldManager.InstallBlock(
                                    selectedStock,
                                    new Xyz(
                                        nextToInstallingBlock.pos.x,
                                        nextToInstallingBlock.pos.y + way,
                                        nextToInstallingBlock.pos.z
                                    )
                                );
                                if (bm != null) blockManagers.Add(bm);
                            }
                            // z方向
                            if (this.transform.forward.z != 0f) {
                                int way = (this.transform.forward.z > 0f) ? -1 : 1;
                                var bm = this.worldManager.InstallBlock(
                                    selectedStock,
                                    new Xyz(
                                        nextToInstallingBlock.pos.x,
                                        nextToInstallingBlock.pos.y,
                                        nextToInstallingBlock.pos.z + way
                                    )
                                );
                                if (bm != null) blockManagers.Add(bm);
                            }

                            // 設置後に視点の先にあるブロック以外は破壊する
                            var ray2 = new Ray(this.transform.position, this.transform.forward);
                            RaycastHit hit2;
                            if (Physics.Raycast(ray2, out hit2, this.reachDist)) {
                                GameObject hitGameObject2 = hit2.collider.gameObject;
                                if (hitGameObject2.tag == "Block") {
                                    Block installingBlock = hitGameObject2.GetComponent<Block>();

                                    // どのブロックを設置したままにするか決定する
                                    int installingIndex = -1;
                                    for (int i = 0; i < blockManagers.Count; i++) {
                                        if (blockManagers[i].pos.Equals(installingBlock.pos)) {
                                            installingIndex = i;
                                            break;
                                        }
                                    }

                                    // 設置したままにするブロック以外破壊する
                                    for (int i = 0; i < blockManagers.Count; i++) {
                                        if (i == installingIndex) continue;
                                        blockManagers[i].Destroy();
                                    }
                                }
                            }

                            
                            // 3個多い状態で始めたので減らす
                            for (int i = 0; i < 3; i++) selectedStock.Decrement();
                        }
                    }
                }
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
        this.InstallBlock();
    }
}
