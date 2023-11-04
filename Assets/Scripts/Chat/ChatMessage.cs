using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatMessage : MonoBehaviour
{
    public Image img;
    public RectTransform rectTrans;
    Button button;
    ChatMenu chatMenu;
    int index;
    public Text message;
    // Start is called before the first frame update
    void Awake()
    {
        img = GetComponent<Image>();
        rectTrans = GetComponent<RectTransform>();
        chatMenu = rectTrans.parent.GetComponent<ChatMenu>();
        index = rectTrans.GetSiblingIndex() - 1;

        //add click listener
        button = GetComponent<Button>();
        button.onClick.AddListener(OnItemClick);
        message = transform.GetChild(0).GetComponent<Text>();
    }

    void OnItemClick()
    {
        chatMenu.OnItemClick(index);
    }

    void OnDestroy()
    {
        //remove click listener to avoid memory leaks
        button.onClick.RemoveListener(OnItemClick);
    }
}
