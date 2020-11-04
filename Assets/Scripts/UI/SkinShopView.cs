using UnityEngine;
using UnityEngine.UI;

//TODO: implement shader for masking models
public class SkinShopView : View<SkinShopView>
{
    private const int c_ItemsPerRow = 3;

    public Image m_BackButton;
    public Image m_SkinCounter;
    public GameObject m_BrushesPrefab;
    public GameObject m_BottomPanel;
    public GameObject m_TopPanel;
    public GameObject m_RowPrefab;
    public GameObject m_ElementPrefab;
    public Transform m_ContentParent;

    private int m_IdSkin = 0;
    private StatsManager m_StatsManager;

    protected override void Awake()
    {
        base.Awake();
        m_StatsManager = StatsManager.Instance;
        m_IdSkin = m_StatsManager.FavoriteSkin;
    }

    protected override void OnGamePhaseChanged(GamePhase _GamePhase)
    {
        base.OnGamePhaseChanged(_GamePhase);

        switch (_GamePhase)
        {
            case GamePhase.SHOP:
                m_TopPanel.SetActive(true);
                m_BottomPanel.SetActive(true);
                Setup();
                Transition(true);
                break;

            case GamePhase.MAIN_MENU:
                m_TopPanel.SetActive(false);
                m_BottomPanel.SetActive(false);
                if (m_Visible)
                    Transition(false);
                break;
        }
    }

    private void Setup()
    {
        int skinsCount = m_GameManager.m_Skins.Count;
        int numRows = skinsCount / c_ItemsPerRow;
        if (skinsCount % c_ItemsPerRow > 0)
            numRows++;

        if (m_ContentParent.childCount > 0)
            return;

        int index = 0;
        for (int i = 0; i < numRows; i++)
        {
            Transform row = Instantiate(m_RowPrefab, m_ContentParent).transform;
            while (row.childCount < c_ItemsPerRow)
            {
                ShopElement element = Instantiate(m_ElementPrefab, row).GetComponent<ShopElement>();
                if (index < m_GameManager.m_Skins.Count)
                {
                    element.Init(m_GameManager.m_Skins[index], index, ChangeBrush);
                    index++;
                }
            }
        }
    }

    public void OnBackButton()
    {
        if (m_GameManager.currentPhase == GamePhase.SHOP)
            m_GameManager.ChangePhase(GamePhase.MAIN_MENU);
    }

    public void SetTitleColor(Color _Color)
    {
        m_BrushesPrefab.SetActive(true);
        int favoriteSkin = Mathf.Min(m_StatsManager.FavoriteSkin, m_GameManager.m_Skins.Count - 1);
        m_BrushesPrefab.GetComponent<BrushMainMenu>().Set(m_GameManager.m_Skins[favoriteSkin]);

        m_BackButton.color = _Color;
        m_SkinCounter.color = _Color;
    }

    public void ChangeBrush(int _NewBrush)
    {
        _NewBrush = Mathf.Clamp(_NewBrush, 0, GameManager.Instance.m_Skins.Count);
        m_IdSkin = _NewBrush;
        GameManager.Instance.m_PlayerSkinID = m_IdSkin;
        int favoriteSkin = Mathf.Min(m_StatsManager.FavoriteSkin, m_GameManager.m_Skins.Count - 1);
        m_BrushesPrefab.GetComponent<BrushMainMenu>().Set(GameManager.Instance.m_Skins[favoriteSkin]);
        m_StatsManager.FavoriteSkin = m_IdSkin;
        GameManager.Instance.SetColor(GameManager.Instance.ComputeCurrentPlayerColor(true, 0));
    }

}
