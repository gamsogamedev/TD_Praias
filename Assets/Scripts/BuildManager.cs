using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;

    [Header("Torres Disponíveis")]
    public GameObject crabTowerPrefab;
    public GameObject turtleTowerPrefab;
    public GameObject octopusTowerPrefab;

    private BuildSpot selectedSpot;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OpenMenu(BuildSpot spot)
    {
        selectedSpot = spot;

        Debug.Log("Área selecionada.");

        Debug.Log("Pressione:");
        Debug.Log("1 - Caranguejo");
        Debug.Log("2 - Tartaruga");
        Debug.Log("3 - Polvo");
    }

    private void Update()
    {
        if (selectedSpot == null)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            BuildTower(crabTowerPrefab);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            BuildTower(turtleTowerPrefab);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            BuildTower(octopusTowerPrefab);
        }
    }

    public void BuildTower(GameObject towerPrefab)
    {
        if (selectedSpot == null)
            return;

        if (selectedSpot.currentTower != null)
            return;

        GameObject towerObject = Instantiate(
            towerPrefab,
            selectedSpot.transform.position,
            Quaternion.identity
        );

        Tower tower = towerObject.GetComponent<Tower>();

        if (tower != null)
        {
            tower.pathPoints = selectedSpot.pathPoints;
        }

        selectedSpot.currentTower = tower;

        Debug.Log("Torre construída!");

        selectedSpot = null;
    }
}