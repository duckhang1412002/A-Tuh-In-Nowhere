using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    private Sprite[] wallSprites;
    private static float[] rotation = { 0f, 90.0f, 180.0f, 270.0f };
    private void Awake()
    {
        wallSprites = new Sprite[6];
        wallSprites[0] = Resources.Load<Sprite>("Sprites/Wall/wall-0");
        wallSprites[1] = Resources.Load<Sprite>("Sprites/Wall/wall-1");
        wallSprites[2] = Resources.Load<Sprite>("Sprites/Wall/wall-2");
        wallSprites[3] = Resources.Load<Sprite>("Sprites/Wall/wall-3");
        wallSprites[4] = Resources.Load<Sprite>("Sprites/Wall/wall-4");
        wallSprites[5] = Resources.Load<Sprite>("Sprites/Wall/wall");
    }

    public void RenderSprite(int spriteIndex, int rotationIndex)
    {
        SpriteRenderer spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        Transform transform = this.gameObject.GetComponent<Transform>();

        spriteRenderer.sprite = wallSprites[spriteIndex];
        transform.Rotate(0f, 0f, rotation[rotationIndex]);
    }
}
