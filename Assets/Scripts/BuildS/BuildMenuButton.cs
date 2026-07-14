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

    [Header("Cores")]
    public Color colorNormal = new Color(0.1f, 0.1f, 0.1f, 0.75f);
    public Color colorHover = new Color(0.2f, 0.2f, 0.2f, 0.95f);
    public Color colorNoGold = new Color(0.3f, 0.3f, 0.3f, 0.5f);
    public Color costColorNormal = new Color(1f, 0.85f, 0f, 1f);
    public Color costColorNoGold = new Color(0.8f, 0.3f, 0.3f, 1f);

    private int towerIndex = -1;
    private int cost;
    private bool isUpgradeButton = false;

    //====================================================
    // SETUP
    //====================================================

    // Chamado pelo menu de construção
    public void SetupBuild(int index, BuildManager.TowerOption option)
    {
        isUpgradeButton = false;
        towerIndex = index;
        cost = option.buildCost;

        if (iconImage != null && option.icon != null)
            iconImage.sprite = option.icon;

        if (costText != null)
            costText.text = option.buildCost.ToString();

        AtualizarCor();
    }

    // Chamado pelo menu de evolução
    public void SetupUpgrade(Tower.TowerLevel proximoNivel)
    {
        isUpgradeButton = true;
        towerIndex = -1;
        cost = proximoNivel.upgradeCost;

        if (iconImage != null && proximoNivel.upgradeIcon != null)
            iconImage.sprite = proximoNivel.upgradeIcon;

        if (costText != null)
            costText.text = proximoNivel.upgradeCost.ToString();

        AtualizarCor();
    }

    //====================================================
    // INTERAÇÃO
    //====================================================
    public void OnPointerClick(PointerEventData eventData)
    {
    Debug.Log($"Botão clicado! isUpgradeButton={isUpgradeButton}");
    eventData.Use();

    if (isUpgradeButton)
        BuildManager.Instance.UpgradeTower();
    else
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

    //====================================================
    // VISUAL
    //====================================================

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
}