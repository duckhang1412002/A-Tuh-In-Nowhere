using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatMenu : MonoBehaviourPun
{
    public Vector2 spacing;
    Button mainBtn;
    public ChatMessage[] chatMessages;
    bool isExpand = false;

    public Vector2 mainBtnPos;
    int itemCnt;

    public Text otherChatMessage;
    public MessagePU messagePU;

    void Start()
    {
        itemCnt = transform.childCount - 1;
        chatMessages= new ChatMessage[itemCnt];
        for (int i = 0; i < itemCnt; i++) {
            chatMessages[i] = transform.GetChild(i + 1).GetComponent<ChatMessage>();
        }

        mainBtn = transform.GetChild(0).GetComponent<Button>();
        mainBtn.onClick.AddListener(ToggleMenu);
        mainBtn.transform.SetAsLastSibling();

        mainBtnPos = mainBtn.GetComponent<RectTransform>().anchoredPosition;

        ResetPosition();

        mainBtnPos.y -= 25;
    }

    private void ResetPosition()
    {
        for (int i = 0; i < itemCnt; i++)
        {
            chatMessages[i].rectTrans.anchoredPosition = mainBtnPos;
            chatMessages[i].gameObject.SetActive(false);
        }
    }

    void ToggleMenu()
    {
        Debug.Log("Chat menu: " + isExpand);
        isExpand = !isExpand;
        if (isExpand)
        {
            for (int i = 0; i < itemCnt; ++i)
            {
                chatMessages[i].rectTrans.anchoredPosition = mainBtnPos + spacing * (i+1);
                chatMessages[i].gameObject.SetActive(true);
            }
        } else
        {
            ResetPosition();
        }
    }

    public void OnItemClick(int index)
    {
        otherChatMessage.text = chatMessages[index].message.text;
        ResetPosition();
        isExpand = false;
        //Debug.Log(chatMessages[index].message.text);
        photonView.RPC("SendMessage", RpcTarget.Others, chatMessages[index].message.text.ToString());
    }

    [PunRPC]
    private void SendMessage(string msg)
    {
        Debug.Log("Other say: " + msg);
        otherChatMessage.text = msg;
        otherChatMessage.gameObject.SetActive(true);
        messagePU.IsActive = true;
    }

    private void OnDestroy()
    {
        mainBtn.onClick.RemoveListener(ToggleMenu);
    }
}
