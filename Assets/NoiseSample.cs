using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseSample : MonoBehaviour
{
    [SerializeField] int m_From = 0;
    [SerializeField] int m_To = 4;
    [SerializeField] int m_SampleCount = 10;
    [SerializeField] float m_Seed = 875.97f;
    [SerializeField] bool m_5d;

    LineRenderer m_LineRenderer;

    void Start()
    {
        m_LineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        m_LineRenderer.positionCount = m_SampleCount;

        Vector3[] points = new Vector3[m_SampleCount];


        for (int i = 0; i < points.Length; i++)
        {
            float x = m_From + (float)(m_To - m_From) / (m_SampleCount - 1) * i;
            float y = Noise(x, 1f) + Noise(x, 0.5f) + Noise(x, 0.25f) + Noise(x, 0.125f);
            points[i] = new Vector3(x, y, 0);
        }

        m_LineRenderer.SetPositions(points);
    }

    float Noise(float x, float scale = 1f, float time = 0f)
    {
        x /= scale;

        float i = (int)x;
        float f = x - i;

        //float y1 = Mathf.Sin(i * m_Seed + time);
        //float y2 = Mathf.Sin((i + 1) * m_Seed + time);

        float y1 = Hash((int)(i * m_Seed), 0);
        float y2 = Hash((int)((i + 1) * m_Seed), 0);

        float u = f * f * (3.0f - 2.0f * f); // 補間係数
        // float u = f * f * f * (f * (f * 6f- 15f) + 10f); // 5次元補間。より滑らかな補間。

        float y = Mathf.Lerp(y1, y2, u);

        y *= scale;

        return y;
    }

    float Rand(float seed)
    {
        byte[] byteArray = System.BitConverter.GetBytes(seed);
        uint seedi = System.BitConverter.ToUInt32(byteArray, 0);

        const uint A = 1664525;
        const uint C = 1013904223;
        const uint M = 2147483647; // 2^31

        return ((float)((seedi * A + C) & M)) / M;
    }

    int[] P = {36, 102, 45, 194, 188, 241, 32, 141, 115, 97, 117, 82, 143, 209, 1, 112, 158, 169,
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

    float Hash(int x, int y)
    {
        x &= 0xff;
        y &= 0xff;
        return P[(x + P[y]) & 0xff] / 255.0f;
    }

}
