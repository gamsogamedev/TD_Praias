using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UpgradeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Hover")]
    public float escalaHover = 1.15f;
    public float velocidade = 12f;

    private Vector3 escalaAlvo;
    private bool podeInteragir = false;

    private void Awake()
    {
        escalaAlvo = Vector3.one;
    }

    private void Update()
    {
        // Interpola escala suavemente
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            escalaAlvo,
            Time.deltaTime * velocidade
        );

        // Tecla E para evoluir
        if (podeInteragir && Input.GetKeyDown(KeyCode.E))
            Evoluir();
    }

    public void SetInteragivel(bool pode)
    {
        podeInteragir = pode;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!podeInteragir)
            return;

        escalaAlvo = Vector3.one * escalaHover;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        escalaAlvo = Vector3.one;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!podeInteragir)
            return;

        eventData.Use();
        escalaAlvo = Vector3.one;
        Evoluir();
    }

    private void Evoluir()
    {
        BuildManager.Instance.UpgradeTower();
    }
}