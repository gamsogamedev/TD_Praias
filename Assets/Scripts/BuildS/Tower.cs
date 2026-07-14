using UnityEngine;
using System.Collections;

public class Tower : MonoBehaviour
{
    [System.Serializable]
    public class TowerLevel
    {
        public Sprite towerSprite;
        public float spawnRate = 3f;

        [Tooltip("Custo em ouro para evoluir para este nível")]
        public int upgradeCost = 50;

        [Tooltip("Ícone exibido no botão de evolução")]
        public Sprite upgradeIcon;

        [Header("Tropas (opcional)")]
        public GameObject troopPrefab;

        [Header("Cocos (opcional)")]
        public GameObject cocoPrefab;
        public int quantidadeCocos = 0;
        public float velocidadeOrbita = 90f;
        public float raioOrbita = 1.5f;
        public int danoCoco = 1;
        public float cooldownDanoCoco = 0.5f;

        [Header("Gaivota Morteiro (opcional)")]
        public GameObject gaivotaPrefab;
        public float alcanceGaivota = 5f;
        public float intervaloDisparo = 2f;
        public int danoExplosao = 3;
        public float raioExplosao = 1.2f;
        public GameObject efeitoImpactoPrefab;
    }

    [Header("Tower Levels")]
    public TowerLevel[] levels;

    [Header("References")]
    public SpriteRenderer spriteRenderer;
    public Transform spawnPoint;
    public Transform[] pathPoints;

    [Header("Upgrade Effects")]
    public GameObject upgradeEffectPrefab;

    private int currentLevel = 0;
    private CocoOrbit cocoOrbit;
    private GaivotaLauncher gaivotaLauncher;

    void Start()
    {
        cocoOrbit = GetComponent<CocoOrbit>();
        gaivotaLauncher = GetComponent<GaivotaLauncher>();
        ApplyLevel();

        if (GetCurrentLevel().troopPrefab != null)
        {
            InvokeRepeating(nameof(SpawnTroop), 1f, GetCurrentLevel().spawnRate);
        }
    }

    public TowerLevel GetCurrentLevel()
    {
        return levels[currentLevel];
    }

    public bool IsMaxLevel()
    {
        return currentLevel >= levels.Length - 1;
    }

    public TowerLevel GetNextLevel()
    {
        if (IsMaxLevel()) return null;
        return levels[currentLevel + 1];
    }

    public int GetCurrentLevelIndex()
    {
        return currentLevel;
    }

    void SpawnTroop()
    {
        if (GetCurrentLevel().troopPrefab == null)
            return;

        GameObject troop = Instantiate(
            GetCurrentLevel().troopPrefab,
            spawnPoint.position,
            Quaternion.identity
        );

        UnitMovement movement = troop.GetComponent<UnitMovement>();

        if (movement != null)
            movement.SetPath(pathPoints);
        else
            Debug.LogWarning($"A unidade {troop.name} não possui UnitMovement.");
    }

    void ApplyLevel()
    {
        TowerLevel level = GetCurrentLevel();

        if (spriteRenderer != null && level.towerSprite != null)
            spriteRenderer.sprite = level.towerSprite;

        if (cocoOrbit != null && level.cocoPrefab != null)
        {
            cocoOrbit.AplicarNivel(
                level.cocoPrefab,
                level.quantidadeCocos,
                level.velocidadeOrbita,
                level.raioOrbita,
                level.danoCoco,
                level.cooldownDanoCoco
            );
        }

        if (gaivotaLauncher != null && level.gaivotaPrefab != null)
        {
            gaivotaLauncher.AplicarNivel(
                level.gaivotaPrefab,
                level.alcanceGaivota,
                level.intervaloDisparo,
                level.danoExplosao,
                level.raioExplosao,
                level.efeitoImpactoPrefab
            );
        }
    }

    public void UpgradeTower()
    {
        if (IsMaxLevel())
        {
            Debug.Log("Torre já está no nível máximo!");
            return;
        }

        int custo = GetNextLevel().upgradeCost;

        if (!EconomyManager.Instance.TemOuro(custo))
        {
            Debug.Log($"Ouro insuficiente! Necessário: {custo}");
            return;
        }

        EconomyManager.Instance.GastarOuro(custo);
        currentLevel++;
        CancelInvoke(nameof(SpawnTroop));

        if (upgradeEffectPrefab != null)
            Instantiate(upgradeEffectPrefab, transform.position, Quaternion.identity);

        ApplyLevel();

        if (GetCurrentLevel().troopPrefab != null)
            InvokeRepeating(nameof(SpawnTroop), 1f, GetCurrentLevel().spawnRate);

        StartCoroutine(UpgradeAnimation());

        Debug.Log($"Torre evoluída para nível {currentLevel + 1}!");
    }

    private void OnMouseDown()
    {
        BuildManager.Instance.OpenUpgradeMenu(this);
    }

    IEnumerator UpgradeAnimation()
    {
        Vector3 originalScale = transform.localScale;
        transform.localScale = originalScale * 1.3f;
        yield return new WaitForSeconds(0.2f);
        transform.localScale = originalScale;
    }
}