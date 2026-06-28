using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class BaseHealth : MonoBehaviour
{
    [Header("Time")]
    public Team team;

    [Header("Base HP")]
    public int maxHealth = 20;

    [Header("UI")]
    public Image healthFill;

    private int currentHealth;

    private SpriteRenderer[] spriteRenderers;

    void Start()
    {
        currentHealth = maxHealth;

        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        UpdateHealthBar();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        currentHealth = Mathf.Max(currentHealth, 0);

        UpdateHealthBar();

        StartCoroutine(DamageEffect());
        StartCoroutine(ShakeBase());

        if (currentHealth <= 0)
        {
            DestroyBase();
        }
    }

    void UpdateHealthBar()
    {
        if (healthFill != null)
        {
            healthFill.fillAmount =
                (float)currentHealth / maxHealth;
        }
    }

    IEnumerator DamageEffect()
    {
        if (spriteRenderers == null || spriteRenderers.Length == 0)
            yield break;

        for (int i = 0; i < 3; i++)
        {
            foreach (SpriteRenderer sr in spriteRenderers)
            {
                Color c = sr.color;
                c.a = 0.3f;
                sr.color = c;
            }

            yield return new WaitForSeconds(0.05f);

            foreach (SpriteRenderer sr in spriteRenderers)
            {
                Color c = sr.color;
                c.a = 1f;
                sr.color = c;
            }

            yield return new WaitForSeconds(0.05f);
        }
    }

    IEnumerator ShakeBase()
    {
        Vector3 originalPos = transform.position;

        for (int i = 0; i < 5; i++)
        {
            transform.position =
                originalPos +
                (Vector3)Random.insideUnitCircle * 0.1f;

            yield return new WaitForSeconds(0.03f);
        }

        transform.position = originalPos;
    }

    void DestroyBase()
    {
        if (team == Team.Player)
        {
            Debug.Log("Você perdeu!");
        }
        else
        {
            Debug.Log("Você venceu!");
        }

        LoadNextLevel();
    }

    void LoadNextLevel()
    {
        int currentScene =
            SceneManager.GetActiveScene().buildIndex;

        if (currentScene + 1 < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(currentScene + 1);
        }
        else
        {
            Debug.Log("Última fase concluída!");
        }
    }

    public float GetHealthPercent()
    {
        return (float)currentHealth / maxHealth;
    }

    public int GetHealth()
    {
        return currentHealth;
    }
}