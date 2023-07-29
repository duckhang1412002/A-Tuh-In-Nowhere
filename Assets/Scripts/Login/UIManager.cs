using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Login
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager instance;

        public GameObject loginUI;
        public GameObject registerUI;


        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != null)
            {
                Debug.Log("Instance already exists, destroying object!");
                Destroy(this);
            }
        }

        //Turn off all screens
        public void ClearScreen()
        {
            loginUI.SetActive(false);
            registerUI.SetActive(false);
        }
        //Button to turn on the LoginUI
        public void LoginScreen()
        {
            ClearScreen();
            loginUI.SetActive(true);
        }
        //Button to turn on the RegisterUI
        public void RegisterScreen()
        {
            ClearScreen();
            registerUI.SetActive(true);
        }

    }
}
