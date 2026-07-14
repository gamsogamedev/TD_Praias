using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Animação")]
    public float escalaHover = 1.08f;
    public float escalaClick = 0.92f;
    public float velocidadeAnimacao = 12f;

    [Header("Sons (Opcional)")]
    public AudioClip somHover;
    public AudioClip somClick;

    private Vector3 escalaOriginal;
    private Vector3 escalaAlvo;
    private AudioSource audioSource;

    private void Awake()
    {
        escalaOriginal = transform.localScale;
        escalaAlvo = escalaOriginal;
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        // Interpola suavemente para a escala alvo
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            escalaAlvo,
            Time.deltaTime * velocidadeAnimacao
        );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        escalaAlvo = escalaOriginal * escalaHover;

        if (somHover != null && audioSource != null)
            audioSource.PlayOneShot(somHover);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        escalaAlvo = escalaOriginal;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (somClick != null && audioSource != null)
            audioSource.PlayOneShot(somClick);

        StartCoroutine(AnimacaoClick());
    }

    IEnumerator AnimacaoClick()
    {
        // Encolhe rapidamente e volta
        escalaAlvo = escalaOriginal * escalaClick;
        yield return new WaitForSeconds(0.08f);
        escalaAlvo = escalaOriginal * escalaHover;
    }
}