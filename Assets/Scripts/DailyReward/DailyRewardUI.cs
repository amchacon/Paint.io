using UnityEngine;
using UnityEngine.UI;

public class DailyRewardUI : MonoBehaviour
{
    public bool showRewardName;

    [Header("UI Elements")]
    public Text m_TextDay;                  // Text containing the Day text eg. Day 12
    public Text m_TextReward;               // The Text containing the Reward amount
    public Image m_ImageRewardBackground;   // The Reward Image Background
    public Image[] m_RewardImages;          // The Reward Image
    private int m_ImagesAmount;              // Number of icons

    [Header("Internal")]
    public int m_Day;

    [HideInInspector]
    public Reward m_Reward;

    public DailyRewardState m_State;

    // The States a reward can have
    public enum DailyRewardState
    {
        UNCLAIMED_AVAILABLE,
        UNCLAIMED_UNAVAILABLE,
        CLAIMED
    }

    public void Initialize()
    {
        m_ImagesAmount = m_Reward.icons;
        m_TextDay.text = string.Format("Day {0}", m_Day.ToString());
        if (m_Reward.reward > 0)
        {
            if (showRewardName)
            {
                m_TextReward.text = m_Reward.reward + " " + m_Reward.name;
            }
            else
            {
                m_TextReward.text = m_Reward.reward.ToString();
            }
        }
        else
        {
            m_TextReward.text = m_Reward.name.ToString();
        }
        for (int i = 0; i < m_ImagesAmount; i++)
        {
            m_RewardImages[i].enabled = true;
            m_RewardImages[i].sprite = m_Reward.sprite;
        }
    }

    // Refreshes the UI
    public void Refresh()
    {
        switch (m_State)
        {
            case DailyRewardState.UNCLAIMED_AVAILABLE:
                m_ImageRewardBackground.enabled = true;
                break;
            case DailyRewardState.UNCLAIMED_UNAVAILABLE:
                m_ImageRewardBackground.enabled = false;
                break;
            case DailyRewardState.CLAIMED:
                m_ImageRewardBackground.enabled = true;
                break;
        }
    }
}