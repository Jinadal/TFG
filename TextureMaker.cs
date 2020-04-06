using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TextureMaker : MonoBehaviour
{
    SpriteRenderer rend;
    public Texture2D[] textureArray;
    public Color[] colorArray;
    Texture2D tex;
    // Start is called before the first frame update
    void Start()
    {
        GameObject go = new GameObject("Test");
        
        rend = go.AddComponent<SpriteRenderer>();
        tex = MakeTexture(textureArray,colorArray);
        rend.sprite = MakeSpite(tex);
    }

    public Texture2D MakeTexture(Texture2D[] layer, Color[] layerColor)
    {
        if(layer.Length == 0)
        {
            return Texture2D.whiteTexture;
        }
        else if(layer.Length == 1)
        {
            return layer[0];
        }
        Texture2D newTexture = new Texture2D(layer[0].width, layer[0].height);
        Color[] colorArray = new Color[newTexture.width * newTexture.height];
        Color[][] adjustedLayers = new Color[layer.Length][];

        for (int i = 0; i < layer.Length; i++)
        {
            if (i == 0 || newTexture.width == layer[i].width && newTexture.height == layer[i].height)
                adjustedLayers[i] = layer[i].GetPixels();
            else
            {
                int getX, getWidth, setX, setWidth;

                getX = (layer[i].width > newTexture.width) ? (layer[i].width - newTexture.width) / 2 : 0;
                getWidth = (layer[i].width > newTexture.width) ? newTexture.width : layer[i].width;
                setX = (layer[i].width < newTexture.width) ? (newTexture.width - layer[i].width) / 2 : 0;
                setWidth = (layer[i].width < newTexture.width) ? layer[i].width : newTexture.width;

                int getY, getHeight, setY, setHeight;

                getY = (layer[i].height > newTexture.height) ? (layer[i].height - newTexture.height) / 2 : 0;
                getHeight = (layer[i].height > newTexture.height) ? newTexture.height : layer[i].height;
                setY = (layer[i].height < newTexture.height) ? (newTexture.height - layer[i].height) / 2 : 0;
                setHeight = (layer[i].height < newTexture.height) ? layer[i].height : newTexture.height;

                Color[] getPixels = layer[i].GetPixels(getX, getY, getWidth, getHeight);
                if (layer[i].width >= newTexture.width && layer[i].height >= newTexture.height)
                {
                    adjustedLayers[i] = getPixels;
                }
                else
                {
                    Texture2D sizedLayer = ClearTexture(newTexture.width, newTexture.height);
                    sizedLayer.SetPixels(setX, setY, setWidth, setHeight, getPixels);
                    adjustedLayers[i] = sizedLayer.GetPixels();
                }
            }
        }

        for (int i = 0; i < layerColor.Length; i++)
        {
            if(layerColor[i].a < 1f)
            {
                layerColor[i] = new Color(layerColor[i].r, layerColor[i].g, layerColor[i].b, 1f);
            }
        }

        for (int i = 0; i < layer.Length; i++)
        {
            for (int x = 0; x < newTexture.width; x++)
            {
                for (int y = 0; y < newTexture.width; y++)
                {
                    int pixelIndex = x + (y * newTexture.width);
                    Color srcPixel = adjustedLayers[i][pixelIndex];
                    if (srcPixel.a != 0 && srcPixel.r != 0 && i < layerColor.Length)
                    {
                        srcPixel = ApplyColorToPixel(srcPixel,layerColor[i]);
                    }

                    if (srcPixel.a == 1)
                        colorArray[pixelIndex] = srcPixel;
                    else if (srcPixel.a > 0)
                        colorArray[pixelIndex] = Blend(colorArray[pixelIndex], srcPixel);
                }
            }
        }
        newTexture.SetPixels(colorArray);
        newTexture.Apply();

        newTexture.wrapMode = TextureWrapMode.Clamp;
        newTexture.filterMode = FilterMode.Point;

        return newTexture;
    }

    public Texture2D ClearTexture(int width, int heigh)
    {
        Texture2D clearTexture = new Texture2D(width, heigh);
        Color[] clearColor = new Color[width * heigh];
        clearTexture.SetPixels(clearColor);
        return clearTexture;
    }

    Color ApplyColorToPixel(Color pixel, Color apply)
    {
        if(pixel.r == 1f)
        {
            return apply;
        }
        return pixel * apply;
    }

    public Sprite MakeSpite(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
    }

    Color Blend(Color dest, Color src)
    {
        float srcA = src.a;
        float destA = (1 - srcA) * dest.a;
        Color blendDest = dest * destA;
        Color blendSrc = src * srcA;

        return blendDest + blendSrc;
    }

}
