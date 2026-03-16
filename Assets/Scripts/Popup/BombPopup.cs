using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BombPopup : APopup
{
    [SerializeField] private Button bombButton;

    private void OnEnable()
    {
        bombButton.onClick.AddListener(HandleBombButtonClicked);
    }

    private void OnDisable()
    {
        bombButton.onClick.RemoveListener(HandleBombButtonClicked);
    }

    private void HandleBombButtonClicked()
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
