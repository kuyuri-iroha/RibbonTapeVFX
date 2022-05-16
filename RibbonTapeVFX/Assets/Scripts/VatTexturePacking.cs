using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Collections;
using UnityEngine;

public class VatTexturePacking : MonoBehaviour
{
    [SerializeField] private List<Texture2D> sources;

    [ContextMenu("Packing sources")]
    private void PackingSources()
    {
        if (sources.Count <= 0)
        {
            Debug.LogError("テクスチャを sources に指定してください。");
            return;
        }

        if (sources.Any(tex => !tex.isReadable))
        {
            Debug.LogError("sources に指定されている全てのTexture2DをReadableにしてください。");
            return;
        }

        var width = sources.Min(tex => tex.width);
        var height = sources.Sum(tex => tex.height);

        var combined = new Texture2D(width, height, sources.FirstOrDefault()!.format, false);

        var pixelData = new NativeArray<Vector4>(width * height, Allocator.Temp);
        var currentIndex = 0;
        for (var i = 0; i < sources.Count; i++)
        {
            var pixels = sources[i].GetPixelData<Vector4>(0);
            for (var j = 0; j < pixels.Length; j++)
            {
                pixelData[j + currentIndex] = pixels[j];
            }

            currentIndex += pixels.Length;
        }
        
        combined.SetPixelData(pixelData, 0);
        combined.Apply();
        
        var bytes = combined.EncodeToEXR(Texture2D.EXRFlags.None);
        File.WriteAllBytes(Application.dataPath + $"/../{DateTime.Now.ToString("yyyyMMddHHmmss")}_{gameObject.name}.exr", bytes);

        pixelData.Dispose();
        
        DestroyImmediate(combined);
    }
}