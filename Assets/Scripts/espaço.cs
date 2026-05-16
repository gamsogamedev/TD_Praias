using UnityEngine;

public class espaço : MonoBehaviour
{
    public Color hoverColor;
    private Renderer rend;
    private Color startColor;

    void Start(){

        rend = GetComponent<Renderer>();
        startColor = rend.material.color;
    }

    void onMouse(){
        rend.material.color = hoverColor;
    }
    void OnMouseExit(){
        rend.material.color = startColor;
    }
}
