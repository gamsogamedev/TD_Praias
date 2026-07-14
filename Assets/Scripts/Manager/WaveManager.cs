using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    [System.Serializable]
    public class EnemyData
    {
        [Header("Informações")]
        public string nome;
        public GameObject enemyPrefab;

        [Header("Wave mínima")]
        public int waveMinima = 1;

        [Header("Probabilidade")]
        [Range(0, 100)]
        public int chanceInicial = 100;

        [Tooltip("Quanto aumenta a chance por wave.")]
        public int aumentoChancePorWave = 0;

        [Range(1, 100)]
        public int chanceMaxima = 100;
    }

    [Header("Spawn")]
    public Transform spawnPoint;
    public Transform[] pathPoints;

    [Header("Lista de Inimigos")]
    public List<EnemyData> enemies = new();

    [Header("Configuração das Waves")]
    public int waveAtual = 0;
    public int inimigosBase = 8;
    public float tempoEntreSpawns = 1f;
    public float tempoPrimeiraWave = 3f;

   [Header("Tempo entre Waves")]
    public float tempoInicial = 5f;
    public float aumentoTempoPorWave = 2f;
    public float tempoMaximo = 30f;

    [Header("UI")]
    public TMP_Text waveText;
    public TMP_Text countdownText;

    [Header("Próxima Wave")]
    public Button nextWaveButton;
    private int inimigosVivos = 0;
    private float tempoRestanteWave = 0f;
    private Coroutine autoWaveCoroutine;
    private bool primeiraWave = true;
    private void Awake()
    {
        Instance = this;
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
    AtualizarUI();

    if (nextWaveButton != null)
        nextWaveButton.gameObject.SetActive(true);

    StartCoroutine(IniciarPrimeiraWave());
    }
    //====================================================
    // EVENTOS
    //====================================================
    private void OnUnitDeath(Unit unit)
    {
        if (unit == null)
            return;

        if (unit.team != Team.Enemy)
            return;

        DecrementarInimigos();
    }

    //====================================================
    // PRIMEIRA WAVE
    //====================================================
   IEnumerator IniciarPrimeiraWave()
    {
    yield return new WaitForSeconds(tempoPrimeiraWave);

    primeiraWave = false;

    ComecarWaveAgora();
    }
    //====================================================
    // CONTROLE DAS WAVES
    //====================================================
    IEnumerator SpawnWave()
    {
    int waveDestaCoroutine = waveAtual;

    int quantidade =
        Mathf.RoundToInt(
            inimigosBase + Mathf.Pow(waveDestaCoroutine, 1.3f)
        );

    Debug.Log($"Wave {waveDestaCoroutine} iniciada com {quantidade} inimigos!");

    for (int i = 0; i < quantidade; i++)
    {
        SpawnEnemy(waveDestaCoroutine);

        yield return new WaitForSeconds(tempoEntreSpawns);
    }
    }

   public void ComecarWaveAgora()
    {
    if (autoWaveCoroutine != null)
    {
        StopCoroutine(autoWaveCoroutine);
        autoWaveCoroutine = null;
    }

    int recompensa = Mathf.CeilToInt(tempoRestanteWave);

    if (recompensa > 0)
    {
        EconomyManager.Instance.AdicionarOuro(recompensa);
    }

    if (countdownText != null)
        countdownText.text = "";

    tempoRestanteWave = 0;

    waveAtual++;

    AtualizarUI();

    StartCoroutine(SpawnWave());

    ReiniciarTimer();
    }

    //====================================================
    // SPAWN
    //====================================================

    void SpawnEnemy(int wave)
    {
    EnemyData enemy = EscolherInimigo(wave);

    if (enemy == null)
        return;

    GameObject obj = Instantiate(
        enemy.enemyPrefab,
        spawnPoint.position,
        Quaternion.identity
    );

    UnitMovement movement = obj.GetComponent<UnitMovement>();

    if (movement != null)
    {
        movement.SetPath(pathPoints);
    }

    inimigosVivos++;
    }

    EnemyData EscolherInimigo(int wave)
    {
        List<EnemyData> disponiveis = new();
        List<int> pesos = new();

        int pesoTotal = 0;

        foreach (EnemyData enemy in enemies)
        {
            if (wave < enemy.waveMinima)
                continue;

            int peso =
                enemy.chanceInicial +
                (wave - enemy.waveMinima) *
                enemy.aumentoChancePorWave;

            peso = Mathf.Clamp(
                peso,
                1,
                enemy.chanceMaxima
            );

            disponiveis.Add(enemy);
            pesos.Add(peso);

            pesoTotal += peso;
        }

        if (disponiveis.Count == 0)
            return null;

        int numero = Random.Range(0, pesoTotal);

        int soma = 0;

        for (int i = 0; i < disponiveis.Count; i++)
        {
            soma += pesos[i];

            if (numero < soma)
                return disponiveis[i];
        }

        return disponiveis[0];
    }

    //====================================================
    // CONTAGEM DE INIMIGOS
    //====================================================
    private void DecrementarInimigos()
    {
        inimigosVivos = Mathf.Max(0, inimigosVivos - 1);
    }

    public void EnemyReachedBase()
    {
        inimigosVivos = Mathf.Max(0, inimigosVivos - 1);
    }
    float GetTempoEntreWaves()
    {
        return Mathf.Min(
            tempoInicial + Mathf.Sqrt(waveAtual) * aumentoTempoPorWave,
            tempoMaximo
        );
    }
    //====================================================
    // UI
    //====================================================
    void AtualizarUI()
    {
        if (waveText != null)
        {
            waveText.text = $"Wave {waveAtual}";
        }
    }

    //====================================================
    // GETTERS
    //====================================================
    public int GetWaveAtual() => waveAtual;

    public int GetInimigosVivos() => inimigosVivos;

    public int GetQuantidadeInimigosDaWave()
    {
        return Mathf.RoundToInt(
            inimigosBase + Mathf.Pow(waveAtual, 1.3f)
        );
    }
    //====================================================
    // DEBUG
    //====================================================

    [ContextMenu("Iniciar Próxima Wave")]
    public void DebugStartWave()
    {
        ComecarWaveAgora();
    }
    void ReiniciarTimer()
{
    if (primeiraWave)
        return;

    if (autoWaveCoroutine != null)
        StopCoroutine(autoWaveCoroutine);

    autoWaveCoroutine = StartCoroutine(AutoWaveTimer());
}

IEnumerator AutoWaveTimer()
{
    tempoRestanteWave = GetTempoEntreWaves();

    while (tempoRestanteWave > 0)
    {
        if (countdownText != null)
        {
            countdownText.text =
                "Próxima Wave em: " +
                Mathf.CeilToInt(tempoRestanteWave);
        }

        tempoRestanteWave -= Time.deltaTime;

        yield return null;
    }

    tempoRestanteWave = 0;

    ComecarWaveAgora();
}
}