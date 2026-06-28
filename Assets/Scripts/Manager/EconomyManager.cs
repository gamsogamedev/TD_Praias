using TMPro;
using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance;

    [Header("Economia")]
    [SerializeField] private int ouroInicial = 100;

    private int ouroAtual;

    [Header("Interface")]
    [SerializeField] private TMP_Text ouroText;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void OnEnable()
    {
        Health.OnUnitDeath += OnUnitDeath;
    }

    private void OnDisable()
    {
        Health.OnUnitDeath -= OnUnitDeath;
    }

    private void Start()
    {
        ouroAtual = ouroInicial;
        AtualizarUI();
    }

    /// <summary>
    /// Chamado automaticamente quando uma unidade morre.
    /// </summary>
    private void OnUnitDeath(Unit unit)
    {
        if (unit == null)
            return;

        // Apenas inimigos dão recompensa
        if (unit.team != Team.Enemy)
            return;

        AdicionarOuro(unit.goldReward);
    }

    /// <summary>
    /// Tenta gastar ouro.
    /// </summary>
    public bool GastarOuro(int valor)
    {
        if (valor <= 0)
            return true;

        if (ouroAtual < valor)
            return false;

        ouroAtual -= valor;
        AtualizarUI();

        return true;
    }

    /// <summary>
    /// Adiciona ouro ao jogador.
    /// </summary>
    public void AdicionarOuro(int valor)
    {
        if (valor <= 0)
            return;

        ouroAtual += valor;
        AtualizarUI();
    }

    /// <summary>
    /// Remove ouro sem validar.
    /// </summary>
    public void RemoverOuro(int valor)
    {
        if (valor <= 0)
            return;

        ouroAtual = Mathf.Max(0, ouroAtual - valor);

        AtualizarUI();
    }

    /// <summary>
    /// Define diretamente o ouro.
    /// </summary>
    public void DefinirOuro(int valor)
    {
        ouroAtual = Mathf.Max(0, valor);
        AtualizarUI();
    }

    /// <summary>
    /// Retorna o ouro atual.
    /// </summary>
    public int GetOuro()
    {
        return ouroAtual;
    }

    /// <summary>
    /// Verifica se há ouro suficiente.
    /// </summary>
    public bool TemOuro(int valor)
    {
        return ouroAtual >= valor;
    }

    private void AtualizarUI()
    {
        if (ouroText != null)
            ouroText.text = ouroAtual.ToString();
    }
}