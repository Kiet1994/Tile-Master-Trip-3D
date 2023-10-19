using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public enum ColorType
	{
        YELLOW,
        ORANGE,
        RED,
        BLUE,
        GREEN,
        PINK,
        ANY,
        COUNT
	}

	[System.Serializable]
    public class ColorSprite
	{
        public ColorType color;
        public Sprite sprite;
	}
public class ColorPiece : MonoBehaviour
{

    public ColorSprite[] colorSprites;
    private ColorType color;
    public ColorType Color
	{
		get { return color; }
		set { SetColor(value); }
	}
    public int NumColors
	{
        get { return colorSprites.Length; }
	}

    private SpriteRenderer sprite;
    private Dictionary<ColorType, Sprite> colorSpriteDict;

	private void Awake()
	{
        sprite = GetComponent<SpriteRenderer>();

        colorSpriteDict = new Dictionary<ColorType, Sprite>();

        for (int i = 0; i < colorSprites.Length; i++)
		{
			if (!colorSpriteDict.ContainsKey(colorSprites[i].color))
			{
                colorSpriteDict.Add(colorSprites[i].color, colorSprites[i].sprite);
			}
		}
	}
    public void SetColor(ColorType newColor)
	{
        color = newColor;
        if (colorSpriteDict.ContainsKey(newColor))
		{
            sprite.sprite = colorSpriteDict[newColor];
		}
	}
}
