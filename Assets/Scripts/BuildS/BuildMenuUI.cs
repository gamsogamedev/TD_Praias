using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BuildMenuUI : MonoBehaviour
{
    [Header("Botões do Menu")]
    public BuildMenuButton[] buttons;

    [Header("Configuração Circular")]
    [Tooltip("Raio do círculo em pixels UI")]
    public float radius = 100f;

    [Tooltip("Ângulo inicial do primeiro botão (90 = cima)")]
    public float startAngle = 90f;

    [Header("Fundo do Menu")]
    [Tooltip("Image circular semitransparente de fundo. Opcional.")]
    public Image backgroundCircle;

    private Canvas canvas;
    private Camera mainCamera;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        mainCamera = Camera.main;

        Hide();
    }

    public void Show(Vector3 worldPosition, BuildManager.TowerOption[] options)
    {
        gameObject.SetActive(true);

        Vector2 screenPos = mainCamera.WorldToScreenPoint(worldPosition);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            screenPos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera,
            out Vector2 localPos
        );

        GetComponent<RectTransform>().anchoredPosition = localPos;

        int count = Mathf.Min(buttons.Length, options.Length);
        float angleStep = 360f / count;

        for (int i = 0; i < buttons.Length; i++)
        {
            if (i < count)
            {
                float angle = (startAngle + angleStep * i) * Mathf.Deg2Rad;

                Vector2 offset = new Vector2(
                    Mathf.Cos(angle) * radius,
                    Mathf.Sin(angle) * radius
                );

                buttons[i].GetComponent<RectTransform>().anchoredPosition = offset;
                buttons[i].Setup(i, options[i]);
                buttons[i].gameObject.SetActive(true);

                // Animação de surgimento
                buttons[i].transform.localScale = Vector3.zero;
                StartCoroutine(AnimateButton(buttons[i].gameObject, i * 0.05f));
            }
            else
            {
                buttons[i].gameObject.SetActive(false);
            }
        }

        // Anima o fundo
        if (backgroundCircle != null)
        {
            backgroundCircle.gameObject.SetActive(true);
            StartCoroutine(AnimateButton(backgroundCircle.gameObject, 0f));
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    IEnumerator AnimateButton(GameObject btn, float delay)
    {
        yield return new WaitForSeconds(delay);

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * 10f;

            float scale = Mathf.SmoothStep(0f, 1f, t);
            btn.transform.localScale = Vector3.one * scale;

            yield return null;
        }

        btn.transform.localScale = Vector3.one;
    }
}