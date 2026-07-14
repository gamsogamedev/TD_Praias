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
        BuildManager.Instance.OpenMenu(this);
    else
        Debug.Log("Já existe uma torre aqui!");
    }   
    
   public void HideSpotVisual()
    {
    SpriteRenderer sr = GetComponent<SpriteRenderer>();
    if (sr != null)
        sr.enabled = false;

    // Esconde também o collider para não ser clicável novamente
    Collider2D col = GetComponent<Collider2D>();
    if (col != null)
        col.enabled = false;
    }

    public void ShowSpotVisual()
    {
        if (spotVisual != null)
            spotVisual.SetActive(true);
    }
}