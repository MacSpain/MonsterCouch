using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIManager : MonoBehaviour
{ 

    public enum UIState
    {
        MainMenu,
        Settings,
        Gameplay
    }

    [SerializeField]
    private GameObject[] uiStateRootObjects;

    private UIState currentState = UIState.MainMenu;

    private static UIManager _instance;
    public static UIManager Instance { get { return _instance; } }

    void Awake()
    {
        _instance = this;
    }


    public void MainMenu()
    {
        currentState = UIState.MainMenu;
        RefreshUIState();
    }
    public void Settings()
    {
        currentState = UIState.Settings;
        RefreshUIState();
    }
    public void Gameplay()
    {
        currentState = UIState.Gameplay;
        RefreshUIState();
    }

    public void Back()
    {
        switch(currentState)
        {
            case UIState.Settings:
                {
                    currentState = UIState.MainMenu;
                    RefreshUIState();
                }
                break;
        }
    }

    private void RefreshUIState()
    {
        for(int i = 0;  i < uiStateRootObjects.Length; i++)
        {
            if(i == (int)currentState)
            {
                uiStateRootObjects[i].SetActive(true);
            }
            else
            {
                uiStateRootObjects[i].SetActive(false);
            }
        }
    }
}
