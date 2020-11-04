using System;
using UnityEngine;
using UnityEngine.UI;

public class ShopElement : MonoBehaviour
{
    public Button m_Button;

	public void Init (SkinData _skin, int index, Action<int> callback)
	{
        BrushMenu brush = Instantiate(_skin.MenuBrush.m_Prefab, transform).GetComponent<BrushMenu>();
        brush.SetNewColor(_skin.Color.m_Colors[0]);
        RectTransform rect = brush.gameObject.AddComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0);
        rect.localPosition = new Vector3(-20, 60, -20);
        rect.rotation = Quaternion.Euler(0, 90, 0);
        rect.localScale = Vector3.one * 100;
        m_Button.onClick.AddListener(() => callback(index));
    }
}
