using UnityEngine;

public class BuildSpot : MonoBehaviour
{
    [Header("Path da Torre")]
    public Transform[] pathPoints;

    [Header("Torre Construída")]
    public Tower currentTower;

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
}