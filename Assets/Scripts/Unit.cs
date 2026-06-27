using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 2f;

    private Transform[] path;
    private int currentPointIndex = 0;

    [Header("Combat")]
    public string enemyTag;
    public string towerType;

    public int damage = 1;
    public float knockbackForce = 0.5f;

    private bool atacando = false;
    private bool hitCooldown = false;

    [Header("Base Damage")]
    public int baseDamage = 1;

    private Animator anim;

    [Header("Comportamento de Pausa")]
    public bool movimentoIntermitente = false;
    public float tempoAndando = 3f;
    public float tempoPausado = 1f;

    private bool estaPausado = false;

    void Start()
    {
        anim = GetComponent<Animator>();

        if (movimentoIntermitente)
        {
            StartCoroutine(RotinaDeMovimento());
        }
    }

    public void SetPath(Transform[] points)
    {
        path = points;

        if (path != null && path.Length > 0)
        {
            transform.position = path[0].position;
            currentPointIndex = 1;
        }
    }

    void Update()
    {
        if (atacando || estaPausado)
        {
            if (anim != null)
            {
                anim.SetBool("walking", false);
            }

            return;
        }

        if (path == null || currentPointIndex >= path.Length)
        {
            if (anim != null)
            {
                anim.SetBool("walking", false);
            }

            return;
        }

        Transform target = path[currentPointIndex];

        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        if (anim != null)
        {
            anim.SetBool("walking", true);
        }

        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            currentPointIndex++;

            if (currentPointIndex >= path.Length)
            {
                ReachedEnd();
            }
        }
    }

    IEnumerator RotinaDeMovimento()
    {
        while (true)
        {
            estaPausado = false;

            yield return new WaitForSeconds(tempoAndando);

            estaPausado = true;

            yield return new WaitForSeconds(tempoPausado);
        }
    }

    void ReachedEnd()
    {
        BaseHealth enemyBase = FindFirstObjectByType<BaseHealth>();

        if (enemyBase != null)
        {
            enemyBase.TakeDamage(baseDamage);
        }

        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hitCooldown)
            return;

        if (other.CompareTag(enemyTag))
        {
            StartCoroutine(ProcessHit(other));
        }
    }

    IEnumerator ProcessHit(Collider2D other)
    {
        hitCooldown = true;

        Health myHealth = GetComponent<Health>();
        Health enemyHealth = other.GetComponent<Health>();

        Unit enemyUnit = other.GetComponent<Unit>();

        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
        }

        if (enemyUnit != null && myHealth != null)
        {
            myHealth.TakeDamage(enemyUnit.damage);
        }

        ApplyKnockback(other.transform.position);

        if (enemyUnit != null)
        {
            enemyUnit.ApplyKnockback(transform.position);
        }

        yield return new WaitForSeconds(0.3f);

        hitCooldown = false;
    }

    public void ApplyKnockback(Vector3 sourcePosition)
    {
        StartCoroutine(KnockbackRoutine(sourcePosition));
    }

    IEnumerator KnockbackRoutine(Vector3 sourcePosition)
    {
        atacando = true;

        Vector2 direction = (transform.position - sourcePosition).normalized;

        Vector3 startPosition = transform.position;

        Vector3 targetPosition = startPosition + (Vector3)(direction * knockbackForce);

        float duration = 0.15f;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            transform.position = Vector3.Lerp(
                startPosition,
                targetPosition,
                timer / duration
            );

            yield return null;
        }

        atacando = false;
    }
}