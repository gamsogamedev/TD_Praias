using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject unitPrefab;
    public Transform spawnPoint;
    public float spawnRate = 3f;

    [Header("Path")]
    public Transform[] pathPoints;

    void Start()
    {
        InvokeRepeating(nameof(SpawnUnit), 1f, spawnRate);
    }

    void SpawnUnit()
    {
        if (unitPrefab == null)
        {
            Debug.LogError("Unit Prefab não atribuído!");
            return;
        }

        if (pathPoints == null || pathPoints.Length == 0)
        {
            Debug.LogError("Path Points não configurados!");
            return;
        }

        GameObject unit = Instantiate(
            unitPrefab,
            spawnPoint.position,
            Quaternion.identity
        );

        Unit unitScript = unit.GetComponent<Unit>();

        if (unitScript != null)
        {
            unitScript.SetPath(pathPoints);
        }
        else
        {
            Debug.LogError("Prefab não possui script Unit!");
        }
    }
}