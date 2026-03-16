using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private IEnumerator Start()
    {
        zoneLevel = 1;
        tempInventory = new Inventory();
        RandomManager.Instance.Initialize(seed: seeds[Random.Range(0, seeds.Length)], randomCount: 0);
        
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

        HandleScrollRect(reward, turnsNotClaimed);
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

        var visualItem = uiController.RewardInfinityScrollRect.GetVisualItem(item => item.Data.rewardCurrency == reward.GetCurrency()?.ToString());
        FlightManager.Instance.Flight(
                uiController.Spin.FlightStartPoint.position, 
                visualItem.RewardIconTransform.position, 
                visualItem.RewardIconTransform.sizeDelta, 1.75f, Random.Range(3, 7), new Vector3(1, 1, 0) * 2f, reward.GetIcon());
    }

    public void HandleScrollRect(ISpinReward reward, int turnsNotClaimed)
    {
        if (reward.GetCurrency() != null)
        {
            if (uiController.RewardInfinityScrollRect.TryGetItem(item => item.rewardCurrency == reward.GetCurrency().Value.ToString(), out var existingItem, out int index))
            {
                existingItem.TargetValue = tempInventory.GetCurrencyAmount(reward.GetCurrency());
                uiController.RewardInfinityScrollRect.UpdateElements();
                //uiController.RewardInfinityScrollRect.GoToItem(index);
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
            }
            return;
        }

        if (reward.GetCase() != null)
        {
            if (uiController.RewardInfinityScrollRect.TryGetItem(item => item.rewardCurrency == reward.GetCase().Value.ToString(), out var existingItem, out int index))
            {
                existingItem.TargetValue = tempInventory.GetCaseAmount(reward.GetCase());
                uiController.RewardInfinityScrollRect.UpdateElements();
                //uiController.RewardInfinityScrollRect.GoToItem(index);
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
            }
            return;
        }

        if (reward.GetWeapon() != null)
        {
            if (uiController.RewardInfinityScrollRect.TryGetItem(item => item.rewardCurrency == reward.GetWeapon().Value.ToString(), out var existingItem, out int index))
            {
                existingItem.TargetValue = tempInventory.GetWeaponAmount(reward.GetWeapon());
                uiController.RewardInfinityScrollRect.UpdateElements();
                //uiController.RewardInfinityScrollRect.GoToItem(index);
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
            }
            return;
        }
    }
}
