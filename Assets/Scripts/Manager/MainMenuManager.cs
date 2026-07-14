using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Painéis")]
    public GameObject painelInstrucoes;
    public GameObject painelCreditos;

    [Header("Transição")]
    public SceneTransition sceneTransition;

    private void Start()
    {
        // Garante que os painéis começam fechados
        if (painelInstrucoes != null)
            painelInstrucoes.SetActive(false);

        if (painelCreditos != null)
            painelCreditos.SetActive(false);
    }

    //====================================================
    // BOTÕES
    //====================================================

    public void BotaoJogar()
    {
        sceneTransition.IrParaCena("NagaScene");
    }

    public void BotaoInstrucoes()
    {
        FecharTodosPaineis();
        painelInstrucoes.SetActive(true);
    }

    public void BotaoCreditos()
    {
        FecharTodosPaineis();
        painelCreditos.SetActive(true);
    }

    public void BotaoSair()
    {
        Debug.Log("Saindo...");
        Application.Quit();
    }

    public void FecharPainel()
    {
        FecharTodosPaineis();
    }

    //====================================================
    // UTILITÁRIOS
    //====================================================

    private void FecharTodosPaineis()
    {
        if (painelInstrucoes != null)
            painelInstrucoes.SetActive(false);

        if (painelCreditos != null)
            painelCreditos.SetActive(false);
    }
}