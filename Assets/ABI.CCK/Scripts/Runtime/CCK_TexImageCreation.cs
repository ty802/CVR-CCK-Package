﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("")]
public class CCK_TexImageCreation : MonoBehaviour
{
    public void SaveTexture(RenderTexture renderTexture, int resWidth = 512, int resHeight = 512)
    {
        Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
        RenderTexture.active = renderTexture;
        screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
        screenShot.Apply();
        RenderTexture.active = null;
        byte[] bytes = screenShot.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.persistentDataPath + "/bundle.png", bytes);
    }
}
