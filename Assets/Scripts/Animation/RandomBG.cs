using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomBG : MonoBehaviour
{
   [SerializeField]
   private Sprite[] images;

    [SerializeField]
    private GameObject bg;

    private void Start()
    {
        if (images.Length > 0)
        {
            int randomIndex = Random.Range(0, images.Length);
            ChangeImage(images[randomIndex]);
        }
    }

    private void ChangeImage(Sprite img)
    {
        bg.GetComponent<Image>().sprite = img;
    }
}
