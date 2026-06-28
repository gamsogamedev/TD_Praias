using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Unit))]
public class Health : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 10;

    [Header("UI (Opcional)")]
    public Image healthFill;

    [Header("Effects")]
    public bool flashOnHit = true;
    public bool shakeOnHit = true;

    public static event Action<Unit> OnUnitDeath;

    private int currentHealth;

    private bool dead = false;
    private bool flashing = false;

    private SpriteRenderer spriteRenderer;
    private Unit unit;

    void Awake()
    {
        unit = GetComponent<Unit>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void TakeDamage(int damage)
    {
        if (dead)
            return;
            

        if (damage <= 0)
        
            return;
        Debug.Log($"{gameObject.name} recebeu {damage} de dano. HP: {currentHealth - damage}");
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        UpdateHealthBar();

        if (flashOnHit)
            StartCoroutine(Flash());

        if (shakeOnHit)
            StartCoroutine(Shake());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (dead)
            return;

        if (amount <= 0)
            return;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        UpdateHealthBar();
    }

    void Die()
    {
        if (dead)
            return;

        dead = true;

        // Notifica todos os sistemas interessados
        OnUnitDeath?.Invoke(unit);

        Destroy(gameObject);
    }

    void UpdateHealthBar()
    {
        if (healthFill != null)
        {
            healthFill.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    IEnumerator Flash()
    {
        if (flashing || spriteRenderer == null)
            yield break;

        flashing = true;

        for (int i = 0; i < 2; i++)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(0.05f);

            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(0.05f);
        }

        flashing = false;
    }

    IEnumerator Shake()
    {
        Vector3 original = transform.localPosition;

        transform.localPosition =
            original + (Vector3)UnityEngine.Random.insideUnitCircle * 0.05f;

        yield return new WaitForSeconds(0.05f);

        transform.localPosition = original;
    }

    //========================
    // Getters
    //========================

    public int GetHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public float GetHealthPercent()
    {
        return (float)currentHealth / maxHealth;
    }

    public bool IsAlive()
    {
        return !dead;
    }

    public bool IsDead()
    {
        return dead;
    }
}