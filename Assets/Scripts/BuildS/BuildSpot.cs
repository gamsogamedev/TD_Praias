using UnityEngine;

public class BuildSpot : MonoBehaviour
{
    [Header("Path da Torre")]
    public Transform[] pathPoints;

    [Header("Torre Construída")]
    public Tower currentTower;

    [Header("Visual")]
    [Tooltip("Objeto visual do slot vazio (ícone no chão). Opcional.")]
    public GameObject spotVisual;

    public bool IsOccupied()
    {
        return currentTower != null;
    }

    private void OnMouseDown()
    {
        if (currentTower == null)
        {
            BuildManager.Instance.OpenMenu(this);
        }
        else
        {
            Debug.Log("Já existe uma torre aqui!");
        }
    }

    public void HideSpotVisual()
    {
        if (spotVisual != null)
            spotVisual.SetActive(false);
    }

    public void ShowSpotVisual()
    {
        if (spotVisual != null)
            spotVisual.SetActive(true);
    }
}