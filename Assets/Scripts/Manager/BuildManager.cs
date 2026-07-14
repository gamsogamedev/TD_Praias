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
    public UpgradeMenuUI upgradeMenuUI;

    private BuildSpot selectedSpot;
    private Tower selectedTower;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    //====================================================
    // MENU DE CONSTRUÇÃO
    //====================================================

    public void OpenMenu(BuildSpot spot)
    {
        if (selectedSpot == spot)
        {
            CloseMenu();
            return;
        }

        CloseAll();

        selectedSpot = spot;

        buildMenuUI.ShowBuildMenu(
            spot.transform.position,
            new TowerOption[] { crabTower, turtleTower, octopusTower }
        );
    }

    //====================================================
    // MENU DE EVOLUÇÃO
    //====================================================

    public void OpenUpgradeMenu(Tower tower)
    {
        if (selectedTower == tower)
        {
            CloseAll();
            return;
        }

        CloseAll();

        selectedTower = tower;
        upgradeMenuUI.Show(tower.transform.position, tower);
    }

    //====================================================
    // FECHAR
    //====================================================

    public void CloseMenu()
    {
        selectedSpot = null;
        buildMenuUI.Hide();
    }

    public void CloseUpgradeMenu()
    {
        selectedTower = null;
        upgradeMenuUI.Hide();
    }

    public void CloseAll()
    {
        selectedSpot = null;
        selectedTower = null;
        buildMenuUI.Hide();
        upgradeMenuUI.Hide();
    }

    //====================================================
    // CONSTRUIR
    //====================================================

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
            tower.pathPoints = selectedSpot.pathPoints;

        selectedSpot.currentTower = tower;
        selectedSpot.HideSpotVisual();

        Debug.Log($"{option.nome} construída!");

        CloseMenu();
    }

    //====================================================
    // EVOLUIR
    //====================================================

    public void UpgradeTower()
    {
    if (selectedTower == null)
        return;

    Tower torre = selectedTower; // ← guarda antes de fechar
    CloseUpgradeMenu();          // ← fecha primeiro
    torre.UpgradeTower();        // ← depois evolui
    }

    //====================================================
    // UPDATE
    //====================================================

    private void Update()
    {
        if (selectedSpot == null)
            return;

        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            CloseMenu();
    }
}