using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CardGameUIController : MonoBehaviour
{
    [SerializeField] private RewardInfinityScrollRect rewardInfinityScrollRect;
    [SerializeField] private Spin spin;
    [SerializeField] private ProgressIndicator progressIndicator;
    [SerializeField] private Button spinButton;
    [SerializeField] private PopupManager popupManager;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button leaveButton;

    public Spin Spin => spin;
    public RewardInfinityScrollRect RewardInfinityScrollRect => rewardInfinityScrollRect;
    public ProgressIndicator ProgressIndicator => progressIndicator;
    public Button SpinButton => spinButton;
    public Button ExitButton => exitButton;
    public Button LeaveButton => leaveButton;
    public PopupManager PopupManager => popupManager;

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
    }

    public void UpdateVisuals(Zone zone)
    {
        spin.UpdateVisuals(zone);
    }

    public void Initialize(IList<ISpinReward> rewards, int currentProgress)
    {
        SetSpinRewardList(rewards);
        progressIndicator.Initialize(currentProgress, int.MaxValue);
    }

    public void SetSpinRewardList(IList<ISpinReward> rewardList)
    {
        spin.SetRewards(rewardList);
    }
}
