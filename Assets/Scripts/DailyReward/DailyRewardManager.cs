using UnityEngine;
using System;
using System.Globalization;
using System.Collections.Generic;

public class DailyRewardManager : DailyRewardsCore
{
    public List<Reward> rewards;        // Rewards list 
    public DateTime lastRewardTime;     // The last time the user clicked in a reward
    public int availableReward;         // The available reward position the player claim
    public int lastReward;              // the last reward the player claimed

    // Delegates
    public delegate void OnClaimPrize(int day);                 // When the player claims the prize
    public event OnClaimPrize onClaimPrize;

    public DebugManager m_DebugManager;

    protected override void Awake()
    {
        base.Awake();
        m_DebugManager = DebugManager.Instance;
    }

    void Start()
    {
        if (!m_DebugManager.DailyRewardFeature)
            return;

        // Initializes the timer with the current time
        InitializeTimer();
    }

    private void InitializeTimer()
    {
        InitializeDate();
        CheckRewards();
        if (onInitialize != null)
            onInitialize();
    }

    protected override void OnApplicationPause(bool pauseStatus)
    {
        base.OnApplicationPause(pauseStatus);
        CheckRewards();
    }

    public TimeSpan GetTimeDifference()
    {
        TimeSpan difference = lastRewardTime - now;
        return difference.Add(new TimeSpan(0, 24, 0, 0));
    }

    // Check if the player have unclaimed prizes
    public void CheckRewards()
    {
        string lastClaimedTimeStr = PlayerPrefs.GetString(GetLastRewardTimeKey());
        lastReward = PlayerPrefs.GetInt(GetLastRewardKey());

        // It is not the first time the user claimed.
        // We need to know if he can claim another reward or not
        if (!string.IsNullOrEmpty(lastClaimedTimeStr))
        {
            lastRewardTime = DateTime.ParseExact(lastClaimedTimeStr, Constants.c_FMT, CultureInfo.InvariantCulture);

            TimeSpan diff = lastRewardTime - DateTime.Now;
            Debug.Log(" Last claim was " + (long)diff.TotalHours + " hours ago.");

            int days = (int)(Math.Abs(diff.TotalHours) / 24);
            if (days == 0)
            {
                // No claim for you. Try tomorrow
                availableReward = 0;
                return;
            }

            // The player can only claim if he logs between the following day and the next.
            if (days >= 1 && days < 2)
            {
                // If reached the last reward, resets to the first restarting the cicle
                if (lastReward == rewards.Count)
                {
                    availableReward = 1;
                    lastReward = 0;
                    return;
                }

                availableReward = lastReward + 1;

                Debug.Log(" Player can claim prize " + availableReward);
                return;
            }

            if (days >= 2)
            {
                // The player loses the following day reward and resets the prize
                availableReward = 1;
                lastReward = 0;
                Debug.Log(" Prize reset ");
            }
        }
        else
        {
            // Is this the first time? Shows only the first reward
            availableReward = 1;
        }
    }

    // Checks if the player claim the prize and claims it by calling the delegate. Avoids duplicate call
    public void ClaimPrize()
    {
        if (availableReward > 0)
        {
            // Delegate
            if (onClaimPrize != null)
                onClaimPrize(availableReward);

            Debug.Log(" Reward [" + rewards[availableReward - 1] + "] Claimed!");
            PlayerPrefs.SetInt(GetLastRewardKey(), availableReward);

            string lastClaimedStr = now.ToString(Constants.c_FMT);
            PlayerPrefs.SetString(GetLastRewardTimeKey(), lastClaimedStr);
        }
        else if (availableReward == 0)
        {
            Debug.LogError("Error! The player is trying to claim the same reward twice.");
        }

        CheckRewards();
    }

    //Returns the lastReward playerPrefs key depending on instanceId
    private string GetLastRewardKey()
    {
        return Constants.c_LastReward;
    }

    //Returns the lastRewardTime playerPrefs key depending on instanceId
    private string GetLastRewardTimeKey()
    {
        return Constants.c_LastRewardTime;
    }

    // Returns the daily Reward of the day
    public Reward GetReward(int day)
    {
        return rewards[day - 1];
    }

    // Resets the Daily Reward for testing purposes
    public void Reset()
    {
        PlayerPrefs.DeleteKey(GetLastRewardKey());
        PlayerPrefs.DeleteKey(GetLastRewardTimeKey());
    }
}