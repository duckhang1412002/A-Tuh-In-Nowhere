using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class LobbySettingUI : MonoBehaviour
{
    private static string codeName;
    [SerializeField] private GameObject inputFieldObj;
    [SerializeField] private GameObject lobbySettingBtnObj;
    [SerializeField] private GameObject lobbyListBtnObj;
    [SerializeField] private GameObject settingUIObj;
    [SerializeField] private GameObject lobbyListUIObj;

    private TMP_InputField inp_code;
    private Button lobbySettingBtn;
    private Button lobbyListBtn;


    // Start is called before the first frame update
    void Start()
    {
        inp_code = inputFieldObj.GetComponent<TMP_InputField>();
        inp_code.text = "";
        codeName = "";

        if(this.gameObject.name == "LobbySettingUI"){            
            lobbySettingBtn = lobbySettingBtnObj.GetComponent<Button>();
            lobbyListBtn = lobbyListBtnObj.GetComponent<Button>();
            lobbyListUIObj.SetActive(false);
            settingUIObj.SetActive(true);
            lobbyListBtn.onClick.AddListener(SwitchToLobbyList);
            lobbySettingBtn.onClick.AddListener(SwitchToLobbySettings);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ConcatStringCode(GameObject btn){
        if(inp_code.text.Length == 6) return;
        
        inp_code.text += btn.GetComponent<Button>().GetComponentInChildren<TMP_Text>().text.Trim();
        codeName = inp_code.text;
    }

    public void RemoveStringCode(){
        inp_code.text = "";
        codeName = "";
    }


    private void SwitchToLobbyList()
    {
        lobbySettingBtn.interactable = true;
        lobbyListBtn.interactable = false;
        lobbyListUIObj.SetActive(true);
        settingUIObj.SetActive(false);
    }

    private void SwitchToLobbySettings()
    {
        lobbySettingBtn.interactable = false;
        lobbyListBtn.interactable = true;
        lobbyListUIObj.SetActive(false);
        settingUIObj.SetActive(true);
    }

    public string AutoGenerateCode(){
        codeName = "";
        for(int i=0; i<6; i++){
            codeName += Random.Range(1, 9);
        }

        return codeName;
    }

    public void CreateLobby(){
        this.gameObject.GetComponent<LobbyManager2>().CreateRoom(AutoGenerateCode());
    }

    public void JoinLobby(){
        this.gameObject.GetComponent<LobbyManager2>().JoinRoom(codeName);
    }

    public void GetListLobby(){
        /**/

    /**/
    }
}
