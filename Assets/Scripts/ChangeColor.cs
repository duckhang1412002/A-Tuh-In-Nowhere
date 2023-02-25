using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    private Color COLOR_DEFAULT = new Color(255, 255, 255, 1);
    private Color COLOR_RED = new Color(255, 0, 0, 1);

    private Color COLOR_GREEN = new Color(0, 255, 0, 1);

    private Color COLOR_BLUE = new Color(0, 0, 255, 1);
    private SpriteRenderer spriteRenderer; 

    Dictionary<string, Color> colorConvertMap = new Dictionary<string, Color>();

    // Start is called before the first frame update
    public void Start()
    {
        colorConvertMap["Default"] = COLOR_DEFAULT;
        colorConvertMap["Red"] = COLOR_RED;
        colorConvertMap["Green"] = COLOR_GREEN;
        colorConvertMap["Blue"] = COLOR_BLUE;
    }

    public void ChangeSpriteColor(GameObject gameObject, string changedColor){
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.color = colorConvertMap[changedColor];
    }
}
