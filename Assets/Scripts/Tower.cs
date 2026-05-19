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
        InvokeRepeating(nameof(SpawnTroop), 1f, GetCurrentLevel().spawnRate);
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

        Unit unit = troop.GetComponent<Unit>();

        if (unit != null)
        {
            unit.SetPath(pathPoints);
        }
    }

    void ApplyLevel()
    {
        spriteRenderer.sprite = GetCurrentLevel().towerSprite;
    }

public void UpgradeTower()
{
    if (currentLevel >= levels.Length - 1)
    {
        Debug.Log("Torre já está no nível máximo!");
        return;
    }

    StartCoroutine(UpgradeAnimation());

    currentLevel++;

    CancelInvoke(nameof(SpawnTroop));

    Instantiate(
    upgradeEffectPrefab,
    transform.position,
    Quaternion.identity
                );

    ApplyLevel();

    InvokeRepeating(
        nameof(SpawnTroop),
        1f,
        GetCurrentLevel().spawnRate
    );

    Debug.Log("Torre evoluiu para nível " + (currentLevel + 1));
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