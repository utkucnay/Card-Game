using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExitPopup : APopup
{
    public Button stayButton;
    public Button exitButton;

    private void OnEnable()
    {
        stayButton.onClick.AddListener(HandleStayButtonClicked);
        exitButton.onClick.AddListener(HandleExitButtonClicked);
    }

    private void OnDisable()
    {
        stayButton.onClick.RemoveListener(HandleStayButtonClicked);
        exitButton.onClick.RemoveListener(HandleExitButtonClicked);
    }

    private void HandleStayButtonClicked()
    {
        Close();
    }

    private void HandleExitButtonClicked()
    {
        SceneManager.LoadScene(SceneManager.GetSceneAt(0).name);
    }

    public override void Open()
    {
        gameObject.SetActive(true);
    }

    public override void Close()
    {
        gameObject.SetActive(false);
        OnClose?.Invoke();
    }
}
