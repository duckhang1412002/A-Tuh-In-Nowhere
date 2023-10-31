using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    private Color COLOR_DEFAULT = new Color(255, 255, 255, 1);
    private Color COLOR_OPACITY = new Color(255, 255, 255, 0.5f);
    private Color COLOR_RED = new Color(Divider(255f), Divider(0f), Divider(0), 1);
    private Color COLOR_GREEN = new Color(Divider(0), Divider(255f), Divider(0), 1);
    private Color COLOR_BLUE = new Color(Divider(0f), Divider(0f), Divider(255f), 1);
    private Color COLOR_PINK = new Color(Divider(255f), Divider(102f), Divider(178f), 1);
    private Color COLOR_PURPLE = new Color(Divider(255f), Divider(102f), Divider(255f), 1);
    private Color COLOR_YELLOW = new Color(Divider(255f), Divider(255f), Divider(0), 1);
    private Color COLOR_ORANGE = new Color(Divider(255f), Divider(128f), Divider(0), 1);
    private Color COLOR_AQUA = new Color(Divider(0f), Divider(255f), Divider(255f), 1);
    private Color COLOR_INDIGO = new Color(Divider(75f), Divider(0f), Divider(130f), 1);
    private Color COLOR_AZURE = new Color(Divider(255f), Divider(250f), Divider(240f), 1);
    private Color COLOR_STEAL = new Color(Divider(176f), Divider(196f), Divider(222f), 1);

    private SpriteRenderer spriteRenderer;
    private IEnumerator coroutine = null;

    Dictionary<string, Color> colorConvertMap = new Dictionary<string, Color>();

    public void Awake(){
        colorConvertMap["Default"] = COLOR_DEFAULT;
        colorConvertMap["Opacity"] = COLOR_OPACITY;
        colorConvertMap["Red"] = COLOR_RED;
        colorConvertMap["Green"] = COLOR_GREEN;
        colorConvertMap["Blue"] = COLOR_BLUE;
        colorConvertMap["Pink"] = COLOR_PINK;
        colorConvertMap["Purple"] = COLOR_PURPLE;
        colorConvertMap["Yellow"] = COLOR_YELLOW;
        colorConvertMap["Orange"] = COLOR_ORANGE;
        colorConvertMap["Aqua"] = COLOR_AQUA;
        colorConvertMap["Indigo"] = COLOR_INDIGO;
        colorConvertMap["Azure"] = COLOR_AZURE;
        colorConvertMap["Steal"] = COLOR_STEAL;
    }
    // Start is called before the first frame update
    public void Start()
    {
        colorConvertMap["Default"] = COLOR_DEFAULT;
        colorConvertMap["Opacity"] = COLOR_OPACITY;
        colorConvertMap["Red"] = COLOR_RED;
        colorConvertMap["Green"] = COLOR_GREEN;
        colorConvertMap["Blue"] = COLOR_BLUE;
        colorConvertMap["Pink"] = COLOR_PINK;
        colorConvertMap["Purple"] = COLOR_PURPLE;
        colorConvertMap["Yellow"] = COLOR_YELLOW;
        colorConvertMap["Orange"] = COLOR_ORANGE;
        colorConvertMap["Aqua"] = COLOR_AQUA;
        colorConvertMap["Indigo"] = COLOR_INDIGO;
        colorConvertMap["Azure"] = COLOR_AZURE;
        colorConvertMap["Steal"] = COLOR_STEAL;
    }

    public string GetColorCode(Color color)
    {
        foreach (var kvp in colorConvertMap)
        {
            if (kvp.Value == color)
            {
                return kvp.Key;
            }
        }
        return "Not found";
    }

    public void ChangeSpriteColor(GameObject gameObject, string changedColor){
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.color = colorConvertMap[changedColor];
    }

    private static float Divider(float x){
        return x/255f;
    }

    public void StartPipeEffect(GameObject pipeObject, string pipeColor){
        coroutine = PipeEffect(pipeObject,pipeColor);
        StartCoroutine(coroutine);
    }

    public IEnumerator PipeEffect(GameObject pipeObject, string pipeColor){
        for(int i=0; i<2; i++){
            ChangeSpriteColor(pipeObject, "Default");
            yield return new WaitForSeconds(0.1f);
            ChangeSpriteColor(pipeObject, pipeColor);
            yield return new WaitForSeconds(0.1f);
        }

        StopCoroutine(coroutine); 
    }
}
