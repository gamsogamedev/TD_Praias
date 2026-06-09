using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 3;

    private int currentHealth;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool flashing = false;

    void Start()
    {
        currentHealth = maxHealth;

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        StartCoroutine(FlashWhite());
        StartCoroutine(HitShake());

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
        Debug.Log(spriteRenderer);
    }

   IEnumerator FlashWhite()
{
    if (flashing)
        yield break;

    flashing = true;

    for (int i = 0; i < 3; i++)
    {
        spriteRenderer.enabled = false;

        yield return new WaitForSeconds(0.04f);

        spriteRenderer.enabled = true;

        yield return new WaitForSeconds(0.04f);
    }

    flashing = false;
}
IEnumerator HitShake()
{
    Vector3 originalPosition = transform.position;

    transform.position = originalPosition +
                         (Vector3)Random.insideUnitCircle * 0.05f;

    yield return new WaitForSeconds(0.05f);

    transform.position = originalPosition;
}
}