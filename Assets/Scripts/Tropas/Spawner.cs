using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Spawn")]
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
        GameObject troop = Instantiate(
            unitPrefab,
            spawnPoint.position,
            Quaternion.identity
        );

        UnitMovement movement = troop.GetComponent<UnitMovement>();

        if (movement != null)
        {
            movement.SetPath(pathPoints);
        }
    }
}