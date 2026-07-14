using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

public class UpgradeMenuUI : MonoBehaviour
{
    [Header("Botão de Evolução")]
    public GameObject upgradeButton;
    public UpgradeButton upgradeButtonComponent;
    public Image upgradeIcon;
    public TMP_Text upgradeCostText;
    public Image upgradeBackground;

    [Header("Painel Nível Máximo")]
    public GameObject maxLevelPanel;

    [Header("Cores")]
    public Color colorNormal = new Color(0.1f, 0.1f, 0.1f, 0.85f);
    public Color colorNoGold = new Color(0.3f, 0.3f, 0.3f, 0.6f);
    public Color costColorNormal = new Color(1f, 0.85f, 0f, 1f);
    public Color costColorNoGold = new Color(0.9f, 0.2f, 0.2f, 1f);

    [Header("Configuração")]
    public float radius = 80f;
    [Header("Dica de Teclado")]
    public GameObject textoPressioneE;
    private Canvas canvas;
    private Camera mainCamera;
    private Tower towerAtual;
    private bool podeFechar = false;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        mainCamera = Camera.main;
        Hide();
    }

    //====================================================
    // MOSTRAR
    //====================================================

    public void Show(Vector3 worldPosition, Tower tower)
    {
        towerAtual = tower;
        podeFechar = false;

        gameObject.SetActive(true);
        SetPosition(worldPosition);

        if (tower.IsMaxLevel())
            MostrarNivelMaximo();
        else
            MostrarBotaoUpgrade(tower.GetNextLevel());

        StartCoroutine(HabilitarFechamento());
    }

    void MostrarNivelMaximo()
    {
        if (upgradeButton != null)
            upgradeButton.SetActive(false);

        if (upgradeButtonComponent != null)
            upgradeButtonComponent.SetInteragivel(false);

        if (maxLevelPanel != null)
        {
            maxLevelPanel.transform.localScale = Vector3.zero;
            maxLevelPanel.SetActive(true);
            maxLevelPanel.GetComponent<RectTransform>()
                .anchoredPosition = new Vector2(0, radius);
            StartCoroutine(Animar(maxLevelPanel));
        }
        if (textoPressioneE != null)
            textoPressioneE.SetActive(false);
    }

    void MostrarBotaoUpgrade(Tower.TowerLevel proximoNivel)
    {
        if (maxLevelPanel != null)
            maxLevelPanel.SetActive(false);

        if (upgradeButton == null)
            return;

        bool temOuro = EconomyManager.Instance.TemOuro(proximoNivel.upgradeCost);

        // Ícone
        if (upgradeIcon != null && proximoNivel.upgradeIcon != null)
            upgradeIcon.sprite = proximoNivel.upgradeIcon;

        // Custo
        if (upgradeCostText != null)
            upgradeCostText.text = proximoNivel.upgradeCost.ToString();

        // Cores
        if (upgradeBackground != null)
            upgradeBackground.color = temOuro ? colorNormal : colorNoGold;

        if (upgradeCostText != null)
            upgradeCostText.color = temOuro ? costColorNormal : costColorNoGold;

        if (upgradeIcon != null)
            upgradeIcon.color = temOuro ? Color.white : new Color(0.5f, 0.5f, 0.5f, 1f);

        // Interatividade — só hover/clique se tiver ouro
        if (upgradeButtonComponent != null)
            upgradeButtonComponent.SetInteragivel(temOuro);

        // Posiciona e anima
        upgradeButton.transform.localScale = Vector3.zero;
        upgradeButton.GetComponent<RectTransform>()
            .anchoredPosition = new Vector2(0, radius);
        upgradeButton.SetActive(true);
        if (textoPressioneE != null)
    {
        textoPressioneE.transform.localScale = Vector3.zero;
        textoPressioneE.SetActive(temOuro);
        if (temOuro)
        StartCoroutine(Animar(textoPressioneE));
    }
    }

    //====================================================
    // FECHAR
    //====================================================

    public void Hide()
    {
        towerAtual = null;
        podeFechar = false;
        gameObject.SetActive(false);
        if (textoPressioneE != null)
            textoPressioneE.SetActive(false);
    }

    private IEnumerator HabilitarFechamento()
    {
        yield return null; // espera um frame para não fechar no mesmo clique que abriu
        podeFechar = true;
    }

    //====================================================
    // FECHAR AO CLICAR FORA
    //====================================================

   private void Update()
    {
    if (!gameObject.activeSelf || !podeFechar)
        return;

    // Tecla E é capturada pelo UpgradeButton, não fecha o menu
    if (Input.GetKeyDown(KeyCode.E))
        return;

    if (Input.GetMouseButtonDown(0))
    {
        if (!EventSystem.current.IsPointerOverGameObject())
            Hide();
    }

    if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        Hide();
    }

    //====================================================
    // ANIMAÇÃO
    //====================================================

    void SetPosition(Vector3 worldPosition)
    {
        Vector2 screenPos = mainCamera.WorldToScreenPoint(worldPosition);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            screenPos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera,
            out Vector2 localPos
        );

        GetComponent<RectTransform>().anchoredPosition = localPos;
    }

    IEnumerator Animar(GameObject obj)
    {
        obj.transform.localScale = Vector3.zero;

        float t = 0f;

        while (t < 1f)
        {
            if (obj == null) yield break;
            t += Time.deltaTime * 10f;
            obj.transform.localScale = Vector3.one * Mathf.SmoothStep(0f, 1f, t);
            yield return null;
        }

        if (obj != null)
            obj.transform.localScale = Vector3.one;
    }
}