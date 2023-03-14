using UnityEngine;

// 平面座標系
public class Xz {
    public int x;
    public int z;

    public Xz(int x, int z) {
        this.x = x;
        this.z = z;
    }

    // 等価関数
    public bool Equals(Xz other) {
        return this.x == other.x && this.z == other.z;
    }

    // 加算結果を返す
    public Xz Added(Xz other) {
        return new Xz(
            this.x + other.x,
            this.z + other.z
        );
    }

    // Xyzに変換する
    public Xyz ConvertToXyz(int y) {
        return new Xyz(this.x, y, this.z);
    }
}

// 立方座標系
public class Xyz {
    public int x;
    public int y;
    public int z;

    public Xyz(int x, int y, int z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    // 等価関数
    public bool Equals(Xyz other) {
        return (this.x == other.x && this.y == other.y) && this.z == other.z;
    }
    
    // 加算結果を返す
    public Xyz Added(Xyz other) {
        return new Xyz(
            this.x + other.x,
            this.y + other.y,
            this.z + other.z
        );
    }

    // Vector3に変換する
    public Vector3 ConvertToVector3() {
        return new Vector3((float)this.x, (float)this.y, (float)this.z);
    }
}