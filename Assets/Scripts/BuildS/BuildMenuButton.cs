using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class BuildMenuButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Referências")]
    public Image iconImage;
    public Image background;
    public TMP_Text costText;
    public Image coinIcon;

    [Header("Cores")]
    public Color colorNormal = new Color(0.1f, 0.1f, 0.1f, 0.75f);
    public Color colorHover = new Color(0.2f, 0.2f, 0.2f, 0.95f);
    public Color colorNoGold = new Color(0.3f, 0.3f, 0.3f, 0.5f);
    public Color costColorNormal = new Color(1f, 0.85f, 0f, 1f);   // Amarelo ouro
    public Color costColorNoGold = new Color(0.8f, 0.3f, 0.3f, 1f); // Vermelho

    private int towerIndex;
    private int cost;

    public void Setup(int index, BuildManager.TowerOption option)
    {
        towerIndex = index;
        cost = option.buildCost;

        if (iconImage != null && option.icon != null)
            iconImage.sprite = option.icon;

        if (costText != null)
            costText.text = option.buildCost.ToString();

        AtualizarCor();
    }

    void AtualizarCor()
    {
        bool temOuro = EconomyManager.Instance.TemOuro(cost);

        if (background != null)
            background.color = temOuro ? colorNormal : colorNoGold;

        if (costText != null)
            costText.color = temOuro ? costColorNormal : costColorNoGold;

        if (iconImage != null)
            iconImage.color = temOuro ? Color.white : new Color(1f, 1f, 1f, 0.4f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        eventData.Use();
        BuildManager.Instance.BuildTower(towerIndex);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!EconomyManager.Instance.TemOuro(cost))
            return;

        if (background != null)
            background.color = colorHover;

        transform.localScale = Vector3.one * 1.15f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = Vector3.one;
        AtualizarCor();
    }
}