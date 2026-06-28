using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;

    [System.Serializable]
    public class TowerOption
    {
        public string nome;
        public GameObject prefab;
        public int buildCost;
        public Sprite icon;
    }

    [Header("Torres Disponíveis")]
    public TowerOption crabTower;
    public TowerOption turtleTower;
    public TowerOption octopusTower;

    [Header("UI")]
    public BuildMenuUI buildMenuUI;

    private BuildSpot selectedSpot;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void OpenMenu(BuildSpot spot)
    {
        // Se clicou no mesmo spot, fecha o menu
        if (selectedSpot == spot)
        {
            CloseMenu();
            return;
        }

        selectedSpot = spot;

        buildMenuUI.Show(
            spot.transform.position,
            new TowerOption[] { crabTower, turtleTower, octopusTower }
        );
    }

    public void CloseMenu()
    {
        selectedSpot = null;
        buildMenuUI.Hide();
    }

    public void BuildTower(int index)
    {
        if (selectedSpot == null)
            return;

        if (selectedSpot.currentTower != null)
            return;

        TowerOption[] options = new TowerOption[]
        {
            crabTower,
            turtleTower,
            octopusTower
        };

        if (index < 0 || index >= options.Length)
            return;

        TowerOption option = options[index];

        if (!EconomyManager.Instance.GastarOuro(option.buildCost))
        {
            Debug.Log("Ouro insuficiente!");
            return;
        }

        GameObject towerObject = Instantiate(
            option.prefab,
            selectedSpot.transform.position,
            Quaternion.identity
        );

        Tower tower = towerObject.GetComponent<Tower>();

        if (tower != null)
        {
            tower.pathPoints = selectedSpot.pathPoints;
        }

        selectedSpot.currentTower = tower;
        selectedSpot.HideSpotVisual();

        Debug.Log($"{option.nome} construída!");

        CloseMenu();
    }

    private void Update()
    {
        // Fecha o menu ao clicar com botão direito ou apertar Escape
        if (selectedSpot == null)
            return;

        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            CloseMenu();
    }
}