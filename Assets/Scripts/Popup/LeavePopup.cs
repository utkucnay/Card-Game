using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LeavePopup : APopup
{
    public static event System.Action OnCollect;

    [SerializeField] private Button collectButton;
    [SerializeField] private Button stayButton;

    private void OnEnable()
    {
        collectButton.onClick.AddListener(HandleCollectButtonClicked);
        stayButton.onClick.AddListener(HandleStayButtonClicked);
    }

    private void OnDisable()
    {
        collectButton.onClick.RemoveListener(HandleCollectButtonClicked);
        stayButton.onClick.RemoveListener(HandleStayButtonClicked);
    }

    private void HandleCollectButtonClicked()
    {
        OnCollect?.Invoke();
        SceneManager.LoadScene(SceneManager.GetSceneAt(0).name);
    }

    private void HandleStayButtonClicked()
    {
        Close();
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
