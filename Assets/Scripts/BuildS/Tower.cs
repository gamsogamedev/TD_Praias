using UnityEngine;
using System.Collections;

public class Tower : MonoBehaviour
{
    [System.Serializable]
    public class TowerLevel
    {
        public Sprite towerSprite;
        public GameObject troopPrefab;
        public float spawnRate = 3f;

        [Tooltip("Custo em ouro para evoluir para este nível")]
        public int upgradeCost = 50;
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

    void Start()
    {
        ApplyLevel();

        InvokeRepeating(
            nameof(SpawnTroop),
            1f,
            GetCurrentLevel().spawnRate
        );
    }

    TowerLevel GetCurrentLevel()
    {
        return levels[currentLevel];
    }

    void SpawnTroop()
    {
        GameObject troop = Instantiate(
            GetCurrentLevel().troopPrefab,
            spawnPoint.position,
            Quaternion.identity
        );

        UnitMovement movement = troop.GetComponent<UnitMovement>();

        if (movement != null)
        {
            movement.SetPath(pathPoints);
        }
        else
        {
            Debug.LogWarning($"A unidade {troop.name} não possui UnitMovement.");
        }
    }

    void ApplyLevel()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = GetCurrentLevel().towerSprite;
        }
    }

    public void UpgradeTower()
    {
        if (currentLevel >= levels.Length - 1)
        {
            Debug.Log("Torre já está no nível máximo!");
            return;
        }

        int custo = levels[currentLevel + 1].upgradeCost;

        if (!EconomyManager.Instance.TemOuro(custo))
        {
            Debug.Log($"Ouro insuficiente! Necessário: {custo}, Atual: {EconomyManager.Instance.GetOuro()}");
            return;
        }

        EconomyManager.Instance.GastarOuro(custo);

        currentLevel++;

        CancelInvoke(nameof(SpawnTroop));

        if (upgradeEffectPrefab != null)
        {
            Instantiate(
                upgradeEffectPrefab,
                transform.position,
                Quaternion.identity
            );
        }

        ApplyLevel();

        InvokeRepeating(
            nameof(SpawnTroop),
            1f,
            GetCurrentLevel().spawnRate
        );

        StartCoroutine(UpgradeAnimation());

        Debug.Log($"Torre evoluída para nível {currentLevel + 1}! Ouro restante: {EconomyManager.Instance.GetOuro()}");
    }

    void OnMouseDown()
    {
        UpgradeTower();
    }

    IEnumerator UpgradeAnimation()
    {
        Vector3 originalScale = transform.localScale;

        transform.localScale = originalScale * 1.3f;

        yield return new WaitForSeconds(0.2f);

        transform.localScale = originalScale;
    }
}