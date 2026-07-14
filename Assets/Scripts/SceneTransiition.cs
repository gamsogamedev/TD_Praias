using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    [Header("Wipe")]
    [Tooltip("Image preta que cobre a tela em diagonal")]
    public RectTransform wipePanel;

    [Tooltip("Duração do efeito em segundos")]
    public float duracao = 0.8f;

    private string cenaDestino;

    private void Start()
    {
        // Ao iniciar a cena, faz o wipe de entrada (revelar a tela)
        if (wipePanel != null)
            StartCoroutine(WipeEntrada());
    }

    public void IrParaCena(string nomeCena)
    {
        cenaDestino = nomeCena;
        StartCoroutine(WipeSaida());
    }

    //====================================================
    // WIPE ENTRADA — revela a tela ao abrir
    //====================================================

    IEnumerator WipeEntrada()
    {
        // Começa cobrindo a tela
        wipePanel.anchoredPosition = new Vector2(0, 0);

        float tempo = 0f;
        Vector2 inicio = Vector2.zero;
        Vector2 fim = new Vector2(Screen.width * 1.5f, 0);

        while (tempo < duracao)
        {
            tempo += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, tempo / duracao);
            wipePanel.anchoredPosition = Vector2.Lerp(inicio, fim, t);
            yield return null;
        }

        wipePanel.anchoredPosition = fim;
    }

    //====================================================
    // WIPE SAÍDA — cobre a tela antes de trocar de cena
    //====================================================

    IEnumerator WipeSaida()
    {
        float tempo = 0f;
        Vector2 inicio = new Vector2(-Screen.width * 1.5f, 0);
        Vector2 fim = Vector2.zero;

        wipePanel.anchoredPosition = inicio;

        while (tempo < duracao)
        {
            tempo += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, tempo / duracao);
            wipePanel.anchoredPosition = Vector2.Lerp(inicio, fim, t);
            yield return null;
        }

        wipePanel.anchoredPosition = fim;

        SceneManager.LoadScene(cenaDestino);
    }
}