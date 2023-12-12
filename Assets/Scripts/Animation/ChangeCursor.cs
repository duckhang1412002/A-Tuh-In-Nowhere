using UnityEngine;

public class ChangeCursor : MonoBehaviour
{
    [SerializeField] private Sprite cursor;

    void Start()
    {
        // Hide the default cursor
        Cursor.visible = false;

        // Set the custom cursor texture
        Cursor.SetCursor(cursor.texture, Vector2.zero, CursorMode.ForceSoftware);
        DontDestroyOnLoad(this.gameObject);
    }

    void Update()
    {
        // Add your game logic here
    }
}