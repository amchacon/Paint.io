using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuView : View<MainMenuView>
{
    private const string m_BestScorePrefix = "BEST SCORE ";

    public Text m_BestScoreText;
    public Image m_BestScoreBar;
    public GameObject m_BestScoreObject;
    public InputField m_InputField;
    public List<Image> m_ColoredImages;
    public List<Text> m_ColoredTexts;

    public GameObject m_BrushGroundLight;
    public GameObject m_BrushesPrefab;
    public int m_IdSkin = 0;

    public GameObject m_DefaultSkinSelection;
    public GameObject m_MainMenuButtons;

    public GameObject m_PointsPerRank;
    public RankingView m_RankingView;

    public Text m_TotalCoins;

    [Header("Ranks")]
    public string[] m_Ratings;

    private StatsManager m_StatsManager;

    private DebugManager m_DebugManager;

    protected override void Awake()
    {
        base.Awake();
        m_StatsManager = StatsManager.Instance;
        m_DebugManager = DebugManager.Instance;
        m_IdSkin = m_StatsManager.FavoriteSkin;

        m_DefaultSkinSelection.SetActive(!m_DebugManager.ShopFeature);
        m_MainMenuButtons.SetActive(m_DebugManager.ShopFeature);
        m_TotalCoins.gameObject.SetActive(m_DebugManager.DailyRewardFeature);
    }

    public void OnPlayButton()
    {
        if (m_GameManager.currentPhase == GamePhase.MAIN_MENU)
            m_GameManager.ChangePhase(GamePhase.LOADING);
    }

    public void OnNoAdsButton()
    {
        Debug.Log("IAP/RemoveAds");
    }

    public void OnSkinShopButton()
    {
        if (m_GameManager.currentPhase == GamePhase.MAIN_MENU)
            m_GameManager.ChangePhase(GamePhase.SHOP);
    }

    protected override void OnGamePhaseChanged(GamePhase _GamePhase)
    {
        base.OnGamePhaseChanged(_GamePhase);

        switch (_GamePhase)
        {
            case GamePhase.MAIN_MENU:
                Transition(true);
                break;

            case GamePhase.LOADING:
                m_DefaultSkinSelection.SetActive(false);
                if (m_Visible)
                    Transition(false);
                break;

            case GamePhase.SHOP:
                if (m_Visible)
                    Transition(false);
                break;
        }
    }

    public void RefreshTotalCoinsText(int _TotalCoins)
    {
        m_TotalCoins.text = _TotalCoins.ToString();
    }

    public void SetTitleColor(Color _Color)
    {
        if (!m_DebugManager.ShopFeature)
        {
            m_BrushesPrefab.SetActive(true);
            m_BrushGroundLight.SetActive(true);
            int favoriteSkin = Mathf.Min(m_StatsManager.FavoriteSkin, m_GameManager.m_Skins.Count - 1);
            m_BrushesPrefab.GetComponent<BrushMainMenu>().Set(m_GameManager.m_Skins[favoriteSkin]);
        }

            string playerName = m_StatsManager.GetNickname();

        if (playerName != null)
            m_InputField.text = playerName;

        for (int i = 0; i < m_ColoredImages.Count; ++i)
            m_ColoredImages[i].color = _Color;

        for (int i = 0; i < m_ColoredTexts.Count; i++)
            m_ColoredTexts[i].color = _Color;
            
        m_RankingView.gameObject.SetActive(true);
        m_RankingView.RefreshNormal();
    }

    public void OnSetPlayerName(string _Name)
    {
        m_StatsManager.SetNickname(_Name);
    }

    public string GetRanking(int _Rank)
    {
        return m_Ratings[_Rank];
    }

    public int GetRankingCount()
    {
        return m_Ratings.Length;
    }

    public void LeftButtonBrush()
    {
        ChangeBrush(m_IdSkin - 1);
    }

    public void RightButtonBrush()
    {
        ChangeBrush(m_IdSkin + 1);
    }

    public void ChangeBrush(int _NewBrush)
    {
        _NewBrush = Mathf.Clamp(_NewBrush, 0, GameManager.Instance.m_Skins.Count);
        m_IdSkin = _NewBrush;
        if (m_IdSkin >= GameManager.Instance.m_Skins.Count)
            m_IdSkin = 0;
        GameManager.Instance.m_PlayerSkinID = m_IdSkin;
        int favoriteSkin = Mathf.Min(m_StatsManager.FavoriteSkin, m_GameManager.m_Skins.Count - 1);
        m_BrushesPrefab.GetComponent<BrushMainMenu>().Set(GameManager.Instance.m_Skins[favoriteSkin]);
        m_StatsManager.FavoriteSkin = m_IdSkin;
        GameManager.Instance.SetColor(GameManager.Instance.ComputeCurrentPlayerColor(true, 0));
    }
}
