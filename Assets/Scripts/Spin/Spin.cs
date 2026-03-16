using System.Collections;
using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Spin : MonoBehaviour
{
    public Action<ISpinReward, int> OnSpinComplete;
    public Action OnNewSpinBegin;


    [Header("Spin Object References")]
    [SerializeField] private Transform pivotOfObjects;
    [SerializeField] private SpinRewardView[] spinRewards;
    [SerializeField] private RectTransform flightStartPoint;

    public RectTransform FlightStartPoint => flightStartPoint;
    
    [Header("Spin Visual References")]
    [SerializeField] private Image spinBackground;
    [SerializeField] private Image spinArrow;
    [SerializeField] private TextMeshProUGUI zoneText;

    public int RewardCount => spinRewards.Length;


    [Header("Spin Animation Settings")]
    public float beginSpinAnimation = 360f;
    public float slowDownDuration = 2f;
    public float slowDownAmount = 50f;

    private int? currentStopIndex = null;
    private ISpinReward newReward;
    private bool increaseRewardAmount;

    public bool HasSpinAnimationPlayedThisTurn => currentStopIndex.HasValue;

    private int[] rewardNotClaimedTurn;

    [SerializeField] private int testStopIndex = 0;

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    private void SpinAnimationTest()
    {
        StopAllCoroutines();
        StartCoroutine(SpinAnimation(testStopIndex));
    }

    private void Awake()
    {
        rewardNotClaimedTurn = new int[spinRewards.Length];
    }

    private void OnEnable()
    {
        StartCoroutine(SpinAnimationLoop());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void UpdateVisuals(Zone zone)
    {
        spinBackground.sprite = zone.GetZoneIcon();
        spinArrow.sprite = zone.GetZoneIndicatorIcon();
        zoneText.text = zone.GetZoneName();
        zoneText.color = zone.GetZoneColor();
    }

    private IEnumerator SpinAnimationLoop()
    {
        while (true)
        {
            yield return new WaitUntil(() => currentStopIndex.HasValue);
            yield return SpinAnimation(currentStopIndex.Value);
            Debug.Log($"Spin stopped at index: {currentStopIndex.Value}, Reward: {spinRewards[currentStopIndex.Value].GetCurrentReward().GetAmount()}");
            
            OnSpinComplete?.Invoke(spinRewards[currentStopIndex.Value].GetCurrentReward(), rewardNotClaimedTurn[currentStopIndex.Value]);
            
            if (spinRewards[currentStopIndex.Value].GetCurrentReward().IsBomb())
            {
                Debug.Log("Hit a bomb! Resetting progress and rewards.");
                currentStopIndex = null;
                yield break;
            }

            for(int i = 0; i < spinRewards.Length; i++)
            {
                spinRewards[i].CheckClearRewardOneTurn();
            }

            spinRewards[currentStopIndex.Value].SetReward(newReward);

            if (increaseRewardAmount)
            {
                for(int i = 0; i < rewardNotClaimedTurn.Length; i++)
                {
                    rewardNotClaimedTurn[i]++;
                    
                }

                rewardNotClaimedTurn[currentStopIndex.Value] = 0;
            }


            for(int i = 0; i < spinRewards.Length; i++)
            {
                spinRewards[i].UpdateVisual(rewardNotClaimedTurn[i]);
            }

            OnNewSpinBegin?.Invoke();

            currentStopIndex = null;
        }
    }

    public void SpinWheelRequest(int stopIndex, ISpinReward newReward, bool increaseRewardAmount)
    {
        if (currentStopIndex.HasValue)
        {
            Debug.LogWarning("Spin is already in progress. Ignoring new spin request.");
            return;
        }

        currentStopIndex = stopIndex;
        this.newReward = newReward;
        this.increaseRewardAmount = increaseRewardAmount;
    }

    public void SetRewards(IList<ISpinReward> rewards)
    {
        if (rewards.Count != spinRewards.Length)
        {
            Debug.LogError($"Reward count does not match the number of reward views. Expected: {spinRewards.Length}, Actual: {rewards.Count}");
            return;
        }

        for (int i = 0; i < spinRewards.Length; i++)
        {
            spinRewards[i].SetReward(rewards[i]);
        }
    }

    public void ChangeBombRewardOneTurn(ISpinReward reward)
    {
        for (int i = 0; i < spinRewards.Length; i++)
        {
            if (spinRewards[i].SpinReward.IsBomb())
            {
                spinRewards[i].SetRewardOneTurn(reward);
            }
        }
    }

    public void ChangeRewardsOneTurn(ISpinReward[] reward)
    {
        if (reward.Length != spinRewards.Length)
        {
            Debug.LogError($"Reward count does not match the number of reward views. Expected: {spinRewards.Length}, Actual: {reward.Length}");
            return;
        }

        for (int i = 0; i < spinRewards.Length; i++)
        {
            spinRewards[i].SetRewardOneTurn(reward[i]);
        }
    }

    private IEnumerator SpinAnimation(int stopIndex)
    {
        float speed = beginSpinAnimation; // degrees per second  
        float minSpeed = beginSpinAnimation - slowDownAmount * slowDownDuration;

        while (speed > minSpeed)
        {
            SpinLoopAnimation(speed);
            speed -= slowDownAmount * Time.deltaTime; // gradually slow down
            yield return null;
        }

        yield return SpinStopAnimation(stopIndex, speed);
    }

    private void SpinLoopAnimation(float speed)
    {
        float angle = speed * Time.deltaTime;
        pivotOfObjects.transform.RotateAround(pivotOfObjects.position, Vector3.back, angle);

        foreach (var reward in spinRewards)
        {
            reward.UpdateRotation(pivotOfObjects.transform.rotation);
        }
    }

    private IEnumerator SpinStopAnimation(int index, float currentSpeed)
    {
        float targetAngle = 360f * index / spinRewards.Length;
        float currentAngle = pivotOfObjects.transform.rotation.eulerAngles.z;
        float elapsedTime = 0f;
        float duration = Mathf.Abs(MyMath.ReverseClockwiseLerp(currentAngle, targetAngle, 1) - currentAngle) / currentSpeed;

        while (elapsedTime < duration)
        {
            float ease = DOVirtual.EasedValue(0, 1, elapsedTime / duration, Ease.OutQuart);
            float angle = MyMath.ReverseClockwiseLerp(currentAngle, targetAngle, ease);
            pivotOfObjects.transform.rotation = Quaternion.Euler(0, 0, angle);
            elapsedTime += Time.deltaTime;

            foreach (var reward in spinRewards)
            {
                reward.UpdateRotation(pivotOfObjects.transform.rotation);
            }
            yield return null;
        }

        pivotOfObjects.transform.rotation = Quaternion.Euler(0, 0, targetAngle);
    }
}
