using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private CardGameUIController uiController;

    private Inventory tempInventory;
    private Dictionary<string, int> tempRewardInventory;

    //[SerializeField] private ZoneConfig zoneConfig;
    [SerializeField] private SpinRewardConfig spinRewardConfig;

    private LinkedList<ISpinReward> spinRewards;

    private int zoneLevel; 

    [SerializeField] private int[] seeds;
    [SerializeField] private bool overrideSeed;
    [SerializeField] private int seed;

    private IEnumerator Start()
    {
        zoneLevel = 1;
        tempInventory = new Inventory();
        int initialSeed = overrideSeed ? seed : seeds[Random.Range(0, seeds.Length)];
        RandomManager.Instance.Initialize(seed: initialSeed, randomCount: 0);
        
        spinRewards = new LinkedList<ISpinReward>(spinRewardConfig.Rewards);
        
        var selectedRewards = spinRewardConfig.Rewards.OrderBy(r => RandomManager.Instance.GetRandomInt()).Take(uiController.Spin.RewardCount - 1)
        .Concat( new ISpinReward[] { new BombReward() }).OrderBy(r => RandomManager.Instance.GetRandomInt()).ToList();
        
        foreach (var reward in selectedRewards)
        {
            Debug.Log($"Selected reward: {reward.GetCurrency()} x{reward.GetAmount()}");
            spinRewards.Remove(reward);
        }

        uiController.Initialize(
            selectedRewards,
            zoneLevel);

        yield return new WaitForSeconds(0.1f);

        Application.targetFrameRate = 60;
    }

    void OnEnable()
    {
        uiController.Spin.OnSpinComplete += HandleSpinComplete;
        uiController.Spin.OnNewSpinBegin += HandleNewSpinBegin;
        uiController.SpinButton.onClick.AddListener(OnSpinButtonClicked);
        uiController.LeaveButton.onClick.AddListener(HandleLeaveButtonClicked);
        uiController.ExitButton.onClick.AddListener(HandleExitButtonClicked);
        LeavePopup.OnCollect += HandleCollect;
    }

    void OnDisable()
    {
        uiController.Spin.OnSpinComplete -= HandleSpinComplete;
        uiController.Spin.OnNewSpinBegin -= HandleNewSpinBegin;
        uiController.SpinButton.onClick.RemoveListener(OnSpinButtonClicked);
        uiController.LeaveButton.onClick.RemoveListener(HandleLeaveButtonClicked);
        uiController.ExitButton.onClick.RemoveListener(HandleExitButtonClicked);
        LeavePopup.OnCollect -= HandleCollect;
    }

    private void HandleCollect()
    {
        Debug.Log("Collecting rewards and returning to main menu.");
        // Here you would typically save the rewards to the player's profile or inventory and then load the main menu scene.
        // For this example, we'll just log the collected rewards and reset the game state.
    }

    private void HandleExitButtonClicked()
    {
        uiController.PopupManager.OpenExitPopup();
    }

    private void HandleLeaveButtonClicked()
    {
        uiController.PopupManager.OpenLeavePopup();
    }

    private void HandleNewSpinBegin()
    {
        Zone zone = ResourceManager.Instance.GetZone(zoneLevel);
        switch (zone.SpecialType)
        {
            case ZoneSpecialType.SpecialSpinRewardOverBomb:
                uiController.Spin.ChangeBombRewardOneTurn(zone.GetSpecialSpinRewards(1).FirstOrDefault());
                break;
            case ZoneSpecialType.AllSpecialSpinReward:
                uiController.Spin.ChangeRewardsOneTurn(zone.GetSpecialSpinRewards(uiController.Spin.RewardCount));
                break;
        }
    }

    private void OnSpinButtonClicked()
    {
        if (uiController.Spin.HasSpinAnimationPlayedThisTurn)
        {
            Debug.LogWarning("Spin animation has already been played this turn. Please wait for the next turn.");
            return;
        }

        int randomRewardIndex = RandomManager.Instance.GetRandomInt(0, uiController.Spin.RewardCount);
        int newRewardIndex = RandomManager.Instance.GetRandomInt(0, spinRewards.Count);
        var newReward = spinRewards.ElementAt(newRewardIndex);
        spinRewards.Remove(newReward);
        uiController.Spin.SpinWheelRequest(randomRewardIndex, newReward, ResourceManager.Instance.GetZone(zoneLevel).IsIncreaseRewardAmount());
        uiController.ExitButton.interactable = false;
        uiController.LeaveButton.interactable = false;
    }

    private void HandleSpinComplete(ISpinReward reward, int turnsNotClaimed)
    {
        if (reward.IsBomb())
        {
            Debug.Log("Hit a bomb! Resetting progress and rewards.");
            uiController.PopupManager.OpenBombPopup();
            return;
        }

        Debug.Log($"Received spin reward: {reward.GetCurrency()} x{reward.GetAmount(turnsNotClaimed)} (Turns not claimed: {turnsNotClaimed})");

        if (reward.GetCurrency() != null)
        {
            tempInventory.AddCurrency(reward.GetCurrency(), reward.GetAmount(turnsNotClaimed));
            spinRewards.AddFirst(reward);
        }
        if (reward.GetWeapon() != null)
        {
            tempInventory.AddWeapon(reward.GetWeapon(), reward.GetAmount(turnsNotClaimed));
        }
        if (reward.GetCase() != null)
        {
            tempInventory.AddCase(reward.GetCase(), reward.GetAmount(turnsNotClaimed));
        }
        
        zoneLevel++; 

        Sequence sequence = DOTween.Sequence();
        HandleScrollRect(reward, turnsNotClaimed, sequence);
        uiController.UpdateVisuals(ResourceManager.Instance.GetZone(zoneLevel));

        uiController.ProgressIndicator.AnimateNext();

        uiController.ExitButton.interactable = true;
        uiController.LeaveButton.interactable = true;

        if (ResourceManager.Instance.GetZone(zoneLevel).IsLeave)
        {
            uiController.LeaveButton.gameObject.SetActive(true);
            uiController.ExitButton.gameObject.SetActive(false);
        }
        else
        {
            uiController.LeaveButton.gameObject.SetActive(false);
            uiController.ExitButton.gameObject.SetActive(true);
        }

        sequence.AppendInterval(0.1f);

        sequence.AppendCallback(() =>
        {
            RewardInfinityScrollVisualItem visualItem = GetVisualItem(reward);
            if (visualItem == null)
            {
                Debug.LogWarning("Visual item for the reward not found in the scroll rect. Flight animation will not be played.");
                return;
            }
            FlightManager.Instance.Flight(
                uiController.Spin.FlightStartPoint.position, 
                visualItem.RewardIconTransform.position, 
                visualItem.RewardIconTransform.sizeDelta, 1.75f, Random.Range(3, 7), new Vector3(1, 1, 0) * 2f, reward.GetIcon());
        });
    }

    private RewardInfinityScrollVisualItem GetVisualItem(ISpinReward reward)
    {
        RewardInfinityScrollVisualItem visualItem = null;
        if (reward.GetCurrency() != null)
        {
            visualItem = uiController.RewardInfinityScrollRect.GetVisualItem(item => item.Data.rewardCurrency == reward.GetCurrency()?.ToString());
        }
        if (reward.GetWeapon() != null)
        {
            visualItem = uiController.RewardInfinityScrollRect.GetVisualItem(item => item.Data.rewardCurrency == reward.GetWeapon()?.ToString());
        }
        if (reward.GetCase() != null)
        {
            visualItem = uiController.RewardInfinityScrollRect.GetVisualItem(item => item.Data.rewardCurrency == reward.GetCase()?.ToString());
        }
        return visualItem;
    }

    public void HandleScrollRect(ISpinReward reward, int turnsNotClaimed, Sequence sequence)
    {
        if (reward.GetCurrency() != null)
        {
            if (uiController.RewardInfinityScrollRect.TryGetItem(item => item.rewardCurrency == reward.GetCurrency().Value.ToString(), out var existingItem, out int index))
            {
                existingItem.TargetValue = tempInventory.GetCurrencyAmount(reward.GetCurrency());
                uiController.RewardInfinityScrollRect.UpdateElements();
                sequence.Append(uiController.RewardInfinityScrollRect.GoToItem(index));
            }
            else
            {
                var newItem = new RewardInfinityScrollItem
                {
                    rewardCurrency = reward.GetCurrency().Value.ToString(),
                    rewardSprite = ResourceManager.Instance.GetCurrencySprite(reward.GetCurrency().Value),
                    TargetValue = tempInventory.GetCurrencyAmount(reward.GetCurrency())
                };
                uiController.RewardInfinityScrollRect.AddItem(newItem);
                Tween tween = uiController.RewardInfinityScrollRect.GoToLastItem();
                if (tween != null)
                {
                    sequence.Append(tween);
                }
                else
                {
                    Debug.LogWarning("Failed to scroll to the new currency reward item. It may not be visible in the scroll rect.");
                }
            }
            return;
        }

        if (reward.GetCase() != null)
        {
            if (uiController.RewardInfinityScrollRect.TryGetItem(item => item.rewardCurrency == reward.GetCase().Value.ToString(), out var existingItem, out int index))
            {
                existingItem.TargetValue = tempInventory.GetCaseAmount(reward.GetCase());
                uiController.RewardInfinityScrollRect.UpdateElements();
                sequence.Append(uiController.RewardInfinityScrollRect.GoToItem(index));
            }
            else
            {
                var newItem = new RewardInfinityScrollItem
                {
                    rewardCurrency = reward.GetCase().Value.ToString(),
                    rewardSprite = ResourceManager.Instance.GetCaseIcon(reward.GetCase().Value),
                    TargetValue = tempInventory.GetCaseAmount(reward.GetCase())
                };
                uiController.RewardInfinityScrollRect.AddItem(newItem);
                Tween tween = uiController.RewardInfinityScrollRect.GoToLastItem();
                if (tween != null)
                {
                    sequence.Append(tween);
                }
                else
                {
                    Debug.LogWarning("Failed to scroll to the new case reward item. It may not be visible in the scroll rect.");
                }
            }
            return;
        }

        if (reward.GetWeapon() != null)
        {
            if (uiController.RewardInfinityScrollRect.TryGetItem(item => item.rewardCurrency == reward.GetWeapon().Value.ToString(), out var existingItem, out int index))
            {
                existingItem.TargetValue = tempInventory.GetWeaponAmount(reward.GetWeapon());
                uiController.RewardInfinityScrollRect.UpdateElements();
                sequence.Append(uiController.RewardInfinityScrollRect.GoToItem(index));
            }
            else
            {
                var newItem = new RewardInfinityScrollItem
                {
                    rewardCurrency = reward.GetWeapon().Value.ToString(),
                    rewardSprite = ResourceManager.Instance.GetWeaponIcon(reward.GetWeapon().Value),
                    TargetValue = tempInventory.GetWeaponAmount(reward.GetWeapon())
                };
                uiController.RewardInfinityScrollRect.AddItem(newItem);
                Tween tween = uiController.RewardInfinityScrollRect.GoToLastItem();
                if (tween != null)
                {
                    sequence.Append(tween);
                }
                else
                {
                    Debug.LogWarning("Failed to scroll to the new weapon reward item. It may not be visible in the scroll rect.");
                }
            }
            return;
        }
    }
}
