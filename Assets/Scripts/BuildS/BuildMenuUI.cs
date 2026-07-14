using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BuildMenuUI : MonoBehaviour
{
    [Header("Botões do Menu")]
    public BuildMenuButton[] buttons;

    [Header("Painel Nível Máximo")]
    public GameObject maxLevelPanel;
    public TMP_Text maxLevelText;

    [Header("Configuração Circular")]
    public float radius = 100f;
    public float startAngle = 90f;

    [Header("Fundo do Menu")]
    public Image backgroundCircle;

    private Canvas canvas;
    private Camera mainCamera;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        mainCamera = Camera.main;

        Hide();
    }

    //====================================================
    // MENU DE CONSTRUÇÃO
    //====================================================

    public void ShowBuildMenu(Vector3 worldPosition, BuildManager.TowerOption[] options)
    {
        gameObject.SetActive(true);
        SetPosition(worldPosition);

        if (maxLevelPanel != null)
            maxLevelPanel.SetActive(false);

        int count = Mathf.Min(buttons.Length, options.Length);
        float angleStep = 360f / count;

        for (int i = 0; i < buttons.Length; i++)
        {
            if (i < count)
            {
                // Zera escala ANTES de ativar para evitar piscada
                buttons[i].transform.localScale = Vector3.zero;
                PositionButton(buttons[i].GetComponent<RectTransform>(), i, angleStep);
                buttons[i].SetupBuild(i, options[i]);
                buttons[i].gameObject.SetActive(true);
                StartCoroutine(AnimateRoutine(buttons[i].gameObject, i * 0.05f));
            }
            else
            {
                buttons[i].gameObject.SetActive(false);
            }
        }

        if (backgroundCircle != null)
        {
            backgroundCircle.transform.localScale = Vector3.zero;
            backgroundCircle.gameObject.SetActive(true);
            StartCoroutine(AnimateRoutine(backgroundCircle.gameObject, 0f));
        }
    }

    //====================================================
    // UTILITÁRIOS
    //====================================================

    public void Hide()
    {
        gameObject.SetActive(false);
    }

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

    void PositionButton(RectTransform rt, int index, float angleStep)
    {
        if (rt == null) return;

        float angle = (startAngle + angleStep * index) * Mathf.Deg2Rad;

        rt.anchoredPosition = new Vector2(
            Mathf.Cos(angle) * radius,
            Mathf.Sin(angle) * radius
        );
    }

    void PositionButton(BuildMenuButton btn, int index, float angleStep)
    {
        PositionButton(btn.GetComponent<RectTransform>(), index, angleStep);
    }

    IEnumerator AnimateRoutine(GameObject obj, float delay)
    {
        // Garante escala zero antes de qualquer frame ser renderizado
        obj.transform.localScale = Vector3.zero;

        if (delay > 0)
            yield return new WaitForSeconds(delay);

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