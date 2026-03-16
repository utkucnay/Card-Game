using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    [SerializeField] private APopup ExitPopup;
    [SerializeField] private APopup BombPopup;
    [SerializeField] private APopup LeavePopup;
    [SerializeField] private CanvasGroup canvasGroup;

    private void OnEnable()
    {
        ExitPopup.OnClose += ClosePopup;
        BombPopup.OnClose += ClosePopup;
        LeavePopup.OnClose += ClosePopup;
    }

    private void OnDisable()
    {
        ExitPopup.OnClose -= ClosePopup;
        BombPopup.OnClose -= ClosePopup;
        LeavePopup.OnClose -= ClosePopup;
    }

    public void OpenLeavePopup()
    {
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.DOFade(1f, 0.25f).OnComplete(() =>
        {
            LeavePopup.Open();
        });
    }

    public void OpenExitPopup()
    {
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.DOFade(1f, 0.25f).OnComplete(() =>
        {
            ExitPopup.Open();
        });
    }

    public void OpenBombPopup()
    {
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.DOFade(1f, 0.25f).OnComplete(() =>
        {
            BombPopup.Open();
        });
    }

    public void CloseLeavePopup()
    {
        LeavePopup.Close();
    }

    public void CloseExitPopup()
    {
        ExitPopup.Close();
    }

    private void ClosePopup()
    {
        canvasGroup.DOFade(0f, 0.25f).OnComplete(() =>
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        });
    }

    public void CloseBombPopup()
    {
        BombPopup.Close();
    }
}