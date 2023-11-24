using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatMenu : MonoBehaviourPun
{
    public Vector2 spacing;
    
    [SerializeField] private Button btn_ChatMenu;
    public ChatMessage[] chatMessages;
    bool isExpand = false;

    public Vector2 btn_ChatMenuPos;
    int itemCnt;

    public Text otherChatMessage;
    public MessagePU messagePU;
    public Image otherAvatar;

    void Start()
    {
        itemCnt = transform.childCount - 1;
        chatMessages= new ChatMessage[itemCnt];
        for (int i = 0; i < itemCnt; i++) {
            chatMessages[i] = transform.GetChild(i + 1).GetComponent<ChatMessage>();
        }
        if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("GM") || PhotonNetwork.LocalPlayer.CustomProperties["GM"].ToString() == "Versus")
        {
            btn_ChatMenu.gameObject.SetActive(false);
        }

        btn_ChatMenu.onClick.AddListener(ToggleMenu);
        btn_ChatMenu.transform.SetAsLastSibling();

        btn_ChatMenuPos = btn_ChatMenu.GetComponent<RectTransform>().anchoredPosition;

        ResetPosition();

        btn_ChatMenuPos.y -= 25;
    }

    private void ResetPosition()
    {
        for (int i = 0; i < itemCnt; i++)
        {
            chatMessages[i].rectTrans.anchoredPosition = btn_ChatMenuPos;
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
                chatMessages[i].rectTrans.anchoredPosition = btn_ChatMenuPos + spacing * (i+1);
                chatMessages[i].gameObject.SetActive(true);
            }
        } else
        {
            ResetPosition();
        }
    }

    public void OnItemClick(int index)
    {
        if (messagePU.gameObject.activeSelf) return; //there is already a message showing
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

        /*        Transform parentTransform = otherChatMessage.transform.parent;
                GameObject otherChatObject = parentTransform.gameObject;
                otherChatObject.SetActive(true);*/

        messagePU.gameObject.SetActive(true); 
    }

    private void OnDestroy()
    {
        btn_ChatMenu.onClick.RemoveListener(ToggleMenu);
    }
}
