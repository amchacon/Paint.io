using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardView : View<DailyRewardView>
{
    private DailyRewardManager m_DailyRewardManager;
    public GameObject m_DailyRewardPrefab;

    [Header("Panel Reward")]
    public Button m_ButtonClaim;                  // Claim Button
    public Button m_ButtonClose;                  // Close Button
    public Text m_TextTimeDue;                    // Text showing how long until the next claim
    public Transform m_DailyRewardsGroup;         // The transform that contains the rewards
    public ScrollRect m_ScrollRect;               // The Scroll Rect

    [Header("Panel Reward Message")]
    public GameObject m_PanelReward;              // Rewards panel
    public Text m_TextReward;                     // Reward Text to show an explanatory message to the player
    public Button m_ButtonCloseReward;            // The Button to close the Rewards Panel
    public Image m_ImageReward;                   // The image of the reward


    private bool m_ReadyToClaim;                  // Update flag
    private List<DailyRewardUI> m_DailyRewardsUI = new List<DailyRewardUI>();

    protected override void Awake()
    {
        base.Awake();
        m_DailyRewardManager = DailyRewardManager.Instance as DailyRewardManager;
    }

    void Start()
    {
        InitializeDailyRewardsUI();

        m_ButtonClose.gameObject.SetActive(false);

        m_ButtonClaim.onClick.AddListener(() =>
        {
            m_DailyRewardManager.ClaimPrize();
            m_ReadyToClaim = false;
            UpdateUI();
        });

        m_ButtonCloseReward.onClick.AddListener(() =>
        {
            m_PanelReward.SetActive(false);
        });

        m_ButtonClose.onClick.AddListener(() =>
        {
            if (m_Visible)
                Transition(false);
        });

        UpdateUI();

        if (!m_Visible && m_ReadyToClaim)
            Transition(true);
    }

    void OnEnable()
    {
        m_DailyRewardManager.onClaimPrize += OnClaimPrize;
        m_DailyRewardManager.onInitialize += OnInitialize;
    }

    void OnDisable()
    {
        if (m_DailyRewardManager != null)
        {
            m_DailyRewardManager.onClaimPrize -= OnClaimPrize;
            m_DailyRewardManager.onInitialize -= OnInitialize;
        }
    }

    protected override void OnGamePhaseChanged(GamePhase _GamePhase)
    {
        base.OnGamePhaseChanged(_GamePhase);
        switch (_GamePhase)
        {
            case GamePhase.MAIN_MENU:
                if (!m_Visible && m_ReadyToClaim)
                    Transition(true);
                break;
        }
    }

    // Initializes the UI List based on the rewards size
    private void InitializeDailyRewardsUI()
    {
        for (int i = 0; i < m_DailyRewardManager.rewards.Count; i++)
        {
            int day = i + 1;
            var reward = m_DailyRewardManager.GetReward(day);

            DailyRewardUI dailyRewardUI = Instantiate(m_DailyRewardPrefab).GetComponent<DailyRewardUI>();
            dailyRewardUI.transform.SetParent(m_DailyRewardsGroup);
            dailyRewardUI.transform.localScale = Vector2.one;
            dailyRewardUI.transform.SetAsFirstSibling();
            dailyRewardUI.m_Day = day;
            dailyRewardUI.m_Reward = reward;
            dailyRewardUI.Initialize();

            m_DailyRewardsUI.Add(dailyRewardUI);
        }
    }

    public void UpdateUI()
    {
        m_DailyRewardManager.CheckRewards();

        bool isRewardAvailableNow = false;

        var lastReward = m_DailyRewardManager.lastReward;
        var availableReward = m_DailyRewardManager.availableReward;

        foreach (var dailyRewardUI in m_DailyRewardsUI)
        {
            var day = dailyRewardUI.m_Day;

            if (day == availableReward)
            {
                dailyRewardUI.m_State = DailyRewardUI.DailyRewardState.UNCLAIMED_AVAILABLE;

                isRewardAvailableNow = true;
            }
            else if (day <= lastReward)
            {
                dailyRewardUI.m_State = DailyRewardUI.DailyRewardState.CLAIMED;
            }
            else
            {
                dailyRewardUI.m_State = DailyRewardUI.DailyRewardState.UNCLAIMED_UNAVAILABLE;
            }

            dailyRewardUI.Refresh();
        }

        m_ButtonClaim.gameObject.SetActive(isRewardAvailableNow);
        m_ButtonClose.gameObject.SetActive(!isRewardAvailableNow);
        if (isRewardAvailableNow)
        {
            SnapToReward();
            m_TextTimeDue.text = "You can claim your reward!";
        }
        m_ReadyToClaim = isRewardAvailableNow;
    }

    // Snap to the next reward
    public void SnapToReward()
    {
        Canvas.ForceUpdateCanvases();
        var lastRewardIdx = m_DailyRewardManager.lastReward;
        // Scrolls to the last reward element
        if (m_DailyRewardsUI.Count - 1 < lastRewardIdx)
            lastRewardIdx++;
        if (lastRewardIdx > m_DailyRewardsUI.Count - 1)
            lastRewardIdx = m_DailyRewardsUI.Count - 1;
        var target = m_DailyRewardsUI[lastRewardIdx].GetComponent<RectTransform>();
        var content = m_ScrollRect.content;
        float normalizePosition = (float)target.GetSiblingIndex() / (float)content.transform.childCount;
        m_ScrollRect.verticalNormalizedPosition = normalizePosition;
    }

    protected override void Update()
    {
        base.Update();
        m_DailyRewardManager.TickTime();
        CheckTimeDifference();
    }

    private void CheckTimeDifference()
    {
        if (!m_ReadyToClaim)
        {
            TimeSpan difference = m_DailyRewardManager.GetTimeDifference();
            // If the counter below 0 it means there is a new reward to claim
            if (difference.TotalSeconds <= 0)
            {
                m_ReadyToClaim = true;
                UpdateUI();
                SnapToReward();
                return;
            }
            string formattedTs = m_DailyRewardManager.GetFormattedTime(difference);
            m_TextTimeDue.text = string.Format("{0} for your next reward", formattedTs);
        }
    }

    // Delegate
    private void OnClaimPrize(int day)
    {
        m_PanelReward.SetActive(true);

        var reward = m_DailyRewardManager.GetReward(day);
        var unit = reward.name;
        var rewardQt = reward.reward;
        m_ImageReward.sprite = reward.sprite;
        if (rewardQt > 0)
        {
            m_TextReward.text = string.Format("You got {0} {1}!", reward.reward, unit);
        }
        else
        {
            m_TextReward.text = string.Format("You got {0}!", unit);
        }
    }

    private void OnInitialize()
    {
        var isRewardAvailable = m_DailyRewardManager.availableReward > 0;
        UpdateUI();
        SnapToReward();
        CheckTimeDifference();
        if (!m_Visible && isRewardAvailable)
            Transition(true);
    }
}