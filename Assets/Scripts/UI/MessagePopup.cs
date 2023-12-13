using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MessagePopup : MonoBehaviour
{
    public TMP_Text errorText;
    public TMP_Text successText;

    private void Start()
    {
        HidePopup();
    }

    public void ShowErrorPopup(string message)
    {
        errorText.text = message;
        gameObject.SetActive(true);
        StartCoroutine(HidePopupAfterDelay(1.5f)); // 1.5 seconds delay
    }

    public void ShowSuccessPopup(string message){
        successText.text = message;
        gameObject.SetActive(true);
        StartCoroutine(HidePopupAfterDelay(1.5f)); // 1.5 seconds delay
    }

    private System.Collections.IEnumerator HidePopupAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HidePopup();
    }

    public void HidePopup()
    {
        gameObject.SetActive(false);
    }
}
