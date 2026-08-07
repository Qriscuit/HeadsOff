using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerInputName : MonoBehaviour
{
    public NM_GC _NMGC;

    public TMP_InputField InputField;
    public Button ContinueButton;

    public string DisplayName;

    public static PlayerInputName Inst;
    private void Awake()
    {
        Inst = this;
    }

    void Update()
    {
        if(!string.IsNullOrEmpty(InputField.text))
        {
            ContinueButton.interactable = true;
        }
        else
        {
            ContinueButton.interactable = false;
        }
    }

    public void SetName()
    {
        DisplayName = InputField.text;
        Debug.Log(DisplayName);
        _NMGC.LocalPlayersName = InputField.text;

        LogInMainMenu.Inst.GeneralImage.SetActive(true);
        LogInMainMenu.Inst.GameImagesPan.PlayFeedbacks();
    }

    public void StartLocalServer()
    {
        _NMGC.StartServer();
    }
}
