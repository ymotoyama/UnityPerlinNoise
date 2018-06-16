using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PerlinNoiseTest : MonoBehaviour
{
    RawImage m_RawImage;
    Texture2D m_Texture;

    [SerializeField] float m_Frequency = 0.01f;
    [SerializeField] int m_Ocraves = 1;
    [SerializeField] float m_Persistence = 0.5f;
    [SerializeField] float m_Speed = 0.1f;

    void Start()
    {
        m_Texture = new Texture2D(256, 256, TextureFormat.ARGB32, false, false);
        m_Texture.filterMode = FilterMode.Point;

        m_RawImage = GetComponent<RawImage>();
        m_RawImage.texture = m_Texture;
    }

    private void Update()
    {
        Color[] pixels = new Color[256 * 256];

        for (int y = 0; y < 256; y++)
        {
            for (int x = 0; x < 256; x++)
            {
                int index = y * 256 + x;

                float p = PerlinNoise.GetNoise(x, y, Time.time * m_Speed, m_Frequency, m_Ocraves, m_Persistence);
                pixels[index] = new Color(p, p, p, 1f);
            }
        }

        m_Texture.SetPixels(pixels);
        m_Texture.Apply();
    }
}
