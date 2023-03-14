using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree {
    // 木の葉の相対的な位置
    public static List<Xyz> reefRelativePoses = new List<Xyz>() {
        // y=3
        new Xyz(1, 3, 0), new Xyz(-1, 3, 0),
        new Xyz(0, 3, 1), new Xyz(0, 3, -1),
        new Xyz(2, 3, 0), new Xyz(-2, 3, 0),
        new Xyz(0, 3, 2), new Xyz(0, 3, -2),
        new Xyz(1, 3, 1), new Xyz(1, 3, -1),
        new Xyz(-1, 3, 1), new Xyz(-1, 3, -1),
        new Xyz(2, 3, 1), new Xyz(2, 3, -1),
        new Xyz(-2, 3, 1), new Xyz(-2, 3, -1),
        new Xyz(1, 3, 2), new Xyz(-1, 3, 2),
        new Xyz(1, 3, -2), new Xyz(-1, 3, -2),
        // y=4
        new Xyz(1, 4, 0), new Xyz(-1, 4, 0),
        new Xyz(0, 4, 1), new Xyz(0, 4, -1),
        new Xyz(2, 4, 0), new Xyz(-2, 4, 0),
        new Xyz(0, 4, 2), new Xyz(0, 4, -2),
        new Xyz(1, 4, 1), new Xyz(1, 4, -1),
        new Xyz(-1, 4, 1), new Xyz(-1, 4, -1),
        new Xyz(2, 4, 1), new Xyz(2, 4, -1),
        new Xyz(-2, 4, 1), new Xyz(-2, 4, -1),
        new Xyz(1, 4, 2), new Xyz(-1, 4, 2),
        new Xyz(1, 4, -2), new Xyz(-1, 4, -2),
        // y=5
        new Xyz(1, 5, 0), new Xyz(-1, 5, 0),
        new Xyz(0, 5, 1), new Xyz(0, 5, -1),
        new Xyz(1, 5, 1), new Xyz(1, 5, -1),
        new Xyz(-1, 5, 1), new Xyz(-1, 5, -1),
        // y=6
        new Xyz(0, 6, 0),
        new Xyz(1, 6, 0), new Xyz(-1, 6, 0),
        new Xyz(0, 6, 1), new Xyz(0, 6, -1),
    };

    // 木の幹の高さ
    public static int stemHeight = 6;
    // ワールド上の位置
    public Xyz pos;
    // チャンクの二次元位置
    public Xz chunkPos2;

    public Tree (Xyz pos, Xz chunkPos2) {
        this.pos = pos;
        this.chunkPos2 = chunkPos2;
    }
}
