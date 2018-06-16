using UnityEngine;

// https://postd.cc/understanding-perlin-noise/
// のサイト様を参考にほぼそのまま実装
public static class PerlinNoise 
{
    // 乱数テーブル。
    // 0～255の256個のセットをランダムな順序で用意。
    // 全く同じ順序でそれをもう一周用意（そのほうが計算が簡単になるため）。計512個の配列。
    static readonly int[] p = {
        36, 102, 45, 194, 188, 241, 32, 141, 115, 97, 117, 82, 143, 209, 1, 112, 158, 169,
        213, 77, 223, 253, 43, 133, 238, 76, 40, 90, 222, 177, 139, 95, 83, 219, 55, 191, 144, 26,
        203, 37, 232, 221, 0, 17, 100, 59, 138, 11, 204, 134, 38, 71, 207, 84, 114, 235, 210, 23,
        248, 251, 130, 81, 183, 201, 145, 93, 31, 151, 9, 6, 152, 94, 127, 99, 176, 61, 54, 212,
        51, 22, 142, 192, 33, 19, 208, 189, 74, 157, 88, 24, 60, 147, 64, 50, 202, 181, 53, 250,
        215, 186, 228, 150, 105, 30, 69, 140, 35, 200, 224, 107, 27, 57, 185, 225, 92, 155, 226,
        220, 78, 164, 87, 66, 172, 132, 116, 67, 126, 42, 246, 217, 146, 70, 108, 171, 2, 242,
        166, 96, 52, 62, 44, 121, 240, 167, 89, 214, 16, 124, 129, 197, 41, 216, 49, 8, 211, 72,
        120, 46, 170, 48, 122, 174, 153, 104, 68, 5, 125, 101, 230, 205, 187, 179, 58, 182, 21,
        65, 249, 137, 12, 243, 252, 165, 85, 245, 86, 254, 123, 7, 154, 47, 4, 28, 136, 34, 14,
        15, 161, 135, 79, 218, 29, 25, 131, 10, 56, 156, 234, 119, 63, 229, 233, 91, 103, 39, 190,
        118, 3, 198, 113, 75, 244, 163, 80, 178, 160, 173, 227, 106, 196, 149, 148, 175, 255, 236,
        18, 206, 168, 128, 231, 247, 111, 13, 110, 180, 73, 109, 162, 193, 199, 98, 184, 195, 237,
        20, 239, 159,
        // もう一度繰り返す
        36, 102, 45, 194, 188, 241, 32, 141, 115, 97, 117, 82, 143, 209, 1, 112, 158, 169,
        213, 77, 223, 253, 43, 133, 238, 76, 40, 90, 222, 177, 139, 95, 83, 219, 55, 191, 144, 26,
        203, 37, 232, 221, 0, 17, 100, 59, 138, 11, 204, 134, 38, 71, 207, 84, 114, 235, 210, 23,
        248, 251, 130, 81, 183, 201, 145, 93, 31, 151, 9, 6, 152, 94, 127, 99, 176, 61, 54, 212,
        51, 22, 142, 192, 33, 19, 208, 189, 74, 157, 88, 24, 60, 147, 64, 50, 202, 181, 53, 250,
        215, 186, 228, 150, 105, 30, 69, 140, 35, 200, 224, 107, 27, 57, 185, 225, 92, 155, 226,
        220, 78, 164, 87, 66, 172, 132, 116, 67, 126, 42, 246, 217, 146, 70, 108, 171, 2, 242,
        166, 96, 52, 62, 44, 121, 240, 167, 89, 214, 16, 124, 129, 197, 41, 216, 49, 8, 211, 72,
        120, 46, 170, 48, 122, 174, 153, 104, 68, 5, 125, 101, 230, 205, 187, 179, 58, 182, 21,
        65, 249, 137, 12, 243, 252, 165, 85, 245, 86, 254, 123, 7, 154, 47, 4, 28, 136, 34, 14,
        15, 161, 135, 79, 218, 29, 25, 131, 10, 56, 156, 234, 119, 63, 229, 233, 91, 103, 39, 190,
        118, 3, 198, 113, 75, 244, 163, 80, 178, 160, 173, 227, 106, 196, 149, 148, 175, 255, 236,
        18, 206, 168, 128, 231, 247, 111, 13, 110, 180, 73, 109, 162, 193, 199, 98, 184, 195, 237,
        20, 239, 159};


    /// <summary>
    /// フェード（イージング）関数。
    /// </summary>
    /// <param name="t">0～1を与えると、緩急を付けた0～1の値にして返却する</param>
    /// <returns></returns>
    static float Ease(float t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }

    /// <summary>
    /// オクターブ（粒度を変えての重ねがけ）を使ったパーリンノイズ。
    /// </summary>
    /// <param name="x">x座標</param>
    /// <param name="y">y座標</param>
    /// <param name="z">z座標や時間など</param>
    /// <param name="frequency">周波数。値が大きいほど細かい模様になる</param>
    /// <param name="octaves">オクターブの回数</param>
    /// <param name="persistence">細かいノイズの強さ。0～1で指定。1に近いほど細かいノイズが強く出る。</param>
    /// <returns></returns>
    public static float GetNoise(float x, float y, float z, float frequency, int octaves, float persistence = 0.5f)
    {
        float total = 0f;
        float amplitude = 1f;
        float maxValue = 0f;

        for (int i = 0; i < octaves; i++)
        {
            total += GetNoise(x * frequency, y * frequency, z * frequency) * amplitude;

            maxValue += amplitude;

            amplitude *= persistence;

            frequency *= 2;
        }

        return total / maxValue;
    }

    public static float GetNoise(float x, float y, float z)
    {
        // 小数点以下を切り捨てて、256で割った余りを求める
        int xi = (int)x & 0xff;
        int yi = (int)y & 0xff;
        int zi = (int)z & 0xff;

        // 小数点以下のみ取り出す
        float xf = x - (int)x;
        float yf = y - (int)y;
        float zf = z - (int)z;

        // イージング用係数
        float easeX = Ease(xf);
        float easeY = Ease(yf);
        float easeZ = Ease(zf);

        // 立方体の頂点部分の勾配ベクトルを決める元となる乱数
        int aaa = p[p[p[xi] + yi] + zi];
        int aba = p[p[p[xi] + yi + 1] + zi];
        int aab = p[p[p[xi] + yi] + zi + 1];
        int abb = p[p[p[xi] + yi + 1] + zi + 1];
        int baa = p[p[p[xi + 1] + yi] + zi];
        int bba = p[p[p[xi + 1] + yi + 1] + zi];
        int bab = p[p[p[xi + 1] + yi] + zi + 1];
        int bbb = p[p[p[xi + 1] + yi + 1] + zi + 1];

        // 入力座標の位置ベクトルと勾配ベクトルの内積を計算し、加重平均を取っていく

        float downBack = Mathf.Lerp(
            grad(aaa, xf, yf, zf),
            grad(baa, xf - 1, yf, zf),
            easeX);

        float upBack = Mathf.Lerp(
            grad(aba, xf, yf - 1, zf),
            grad(bba, xf - 1, yf - 1, zf),
            easeX);

        float back = Mathf.Lerp(downBack, upBack, easeY);

        float downForward = Mathf.Lerp(
            grad(aab, xf, yf, zf - 1),
            grad(bab, xf - 1, yf, zf - 1),
            easeX);

        float upForward = Mathf.Lerp(
            grad(abb, xf, yf - 1, zf - 1),
            grad(bbb, xf - 1, yf - 1, zf - 1),
            easeX);

        float forward = Mathf.Lerp(downForward, upForward, easeY);

        float result = Mathf.Lerp(back, forward, easeZ);

        // resultは-1～+1の範囲になっているので、0～1に正規化して返却する
        return (result + 1f) / 2f;
    }

    static float grad(int hash, float x, float y, float z)
    {
        switch (hash & 0xF)
        {
            case 0x0: return x + y;
            case 0x1: return -x + y;
            case 0x2: return x - y;
            case 0x3: return -x - y;
            case 0x4: return x + z;
            case 0x5: return -x + z;
            case 0x6: return x - z;
            case 0x7: return -x - z;
            case 0x8: return y + z;
            case 0x9: return -y + z;
            case 0xA: return y - z;
            case 0xB: return -y - z;
            case 0xC: return y + x;
            case 0xD: return -y + z;
            case 0xE: return y - x;
            case 0xF: return -y - z;
            default: return 0; // never happens
        }
    }
}
