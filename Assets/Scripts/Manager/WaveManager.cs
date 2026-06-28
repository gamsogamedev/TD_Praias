using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
    public float tempoEntreWaves = 8f;

    [Header("UI")]
    public TMP_Text waveText;
    public TMP_Text countdownText;

    private bool waveEmAndamento = false;
    private int inimigosVivos = 0;

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
    // WAVES
    //====================================================

    IEnumerator IniciarPrimeiraWave()
    {
        yield return Countdown(tempoPrimeiraWave);

        IniciarProximaWave();
    }

    public void IniciarProximaWave()
    {
        if (waveEmAndamento)
            return;

        waveAtual++;

        AtualizarUI();

        StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave()
    {
        waveEmAndamento = true;

        int quantidade =
            Mathf.RoundToInt(
                inimigosBase + Mathf.Pow(waveAtual, 1.3f)
            );

        Debug.Log($"Wave {waveAtual} iniciada com {quantidade} inimigos!");

        for (int i = 0; i < quantidade; i++)
        {
            SpawnEnemy();

            yield return new WaitForSeconds(tempoEntreSpawns);
        }

        while (inimigosVivos > 0)
            yield return null;

        Debug.Log($"Wave {waveAtual} concluída!");

        waveEmAndamento = false;

        yield return Countdown(tempoEntreWaves);

        IniciarProximaWave();
    }

    //====================================================
    // SPAWN
    //====================================================

    void SpawnEnemy()
    {
        EnemyData enemy = EscolherInimigo();

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

    EnemyData EscolherInimigo()
    {
        List<EnemyData> disponiveis = new();
        List<int> pesos = new();

        int pesoTotal = 0;

        foreach (EnemyData enemy in enemies)
        {
            if (waveAtual < enemy.waveMinima)
                continue;

            int peso =
                enemy.chanceInicial +
                (waveAtual - enemy.waveMinima) *
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

    // Chamado quando um inimigo morre em combate (via Health.OnUnitDeath)
    private void DecrementarInimigos()
    {
        inimigosVivos = Mathf.Max(0, inimigosVivos - 1);
    }

    // Chamado quando um inimigo chega à base inimiga (via UnitMovement)
    public void EnemyReachedBase()
    {
        inimigosVivos = Mathf.Max(0, inimigosVivos - 1);
    }

    //====================================================
    // CONTADOR ENTRE WAVES
    //====================================================

    IEnumerator Countdown(float tempo)
    {
        float restante = tempo;

        while (restante > 0)
        {
            if (countdownText != null)
            {
                countdownText.text =
                    "Próxima Wave em: " + Mathf.CeilToInt(restante);
            }

            restante -= Time.deltaTime;

            yield return null;
        }

        if (countdownText != null)
        {
            countdownText.text = "";
        }
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

    public bool WaveEmAndamento() => waveEmAndamento;

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
        if (!waveEmAndamento)
            IniciarProximaWave();
    }
}