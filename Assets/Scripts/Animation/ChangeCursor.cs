using UnityEngine;

public class ChangeCursor : MonoBehaviour
{
    [SerializeField] private Sprite cursorSprite;

    void Start()
    {
        // Hide the default cursor
        //Cursor.visible = false;

        if (cursorSprite != null)
        {
            Texture2D customCursorTexture = cursorSprite.texture;
            Cursor.SetCursor(customCursorTexture, Vector2.zero, CursorMode.ForceSoftware);
            Debug.Log("Custom cursor set");
        }
        else
        {
            Debug.LogError("Custom cursor sprite not assigned in the Unity Inspector.");
        }

        DontDestroyOnLoad(this.gameObject);
    }
}