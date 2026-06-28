using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Unit))]
[RequireComponent(typeof(UnitTargeting))]
public class UnitMovement : MonoBehaviour
{
    private Unit unit;
    private UnitTargeting targeting;

    private Transform[] path;
    private int currentPointIndex = 0;

    private bool paused = false;

    void Awake()
    {
        unit = GetComponent<Unit>();
        targeting = GetComponent<UnitTargeting>();
    }

    void Start()
    {
        if (unit.movimentoIntermitente)
        {
            StartCoroutine(PauseRoutine());
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
    if (paused)
    {
        StopWalkingAnimation();
        return;
    }

    if (targeting.HasTarget())
    {
        Unit target = targeting.GetTarget();

        float distance = Vector2.Distance(
            transform.position,
            target.transform.position);

        if (distance <= unit.attackRange)
        {
            StopWalkingAnimation();
            return;
        }

        transform.position = Vector2.MoveTowards(
            transform.position,
            target.transform.position,
            unit.moveSpeed * Time.deltaTime);

        if (unit.animator != null)
            unit.animator.SetBool("walking", true);

        return;
    }

    if (path == null || currentPointIndex >= path.Length)
    {
        StopWalkingAnimation();
        return;
    }

    Transform pathTarget = path[currentPointIndex]; // ← renomeado

    transform.position = Vector2.MoveTowards(
        transform.position,
        pathTarget.position,
        unit.moveSpeed * Time.deltaTime
    );

    if (unit.animator != null)
        unit.animator.SetBool("walking", true);

    if (Vector2.Distance(transform.position, pathTarget.position) < 0.05f)
    {
        currentPointIndex++;

        if (currentPointIndex >= path.Length)
            ReachEnemyBase();
    }
}
    void ReachEnemyBase()
    {
        // Aplica dano à base inimiga
        BaseHealth[] bases =
            FindObjectsByType<BaseHealth>(FindObjectsSortMode.None);

        foreach (BaseHealth b in bases)
        {
            if (b.team != unit.team)
            {
                b.TakeDamage(unit.baseDamage);
                break;
            }
        }

        // Notifica o WaveManager que este inimigo saiu de campo
        // (por chegar à base, não por morrer em combate)
        if (unit.team == Team.Enemy)
        {
            WaveManager.Instance?.EnemyReachedBase();
        }

        Destroy(gameObject);
    }

    IEnumerator PauseRoutine()
    {
        while (true)
        {
            paused = false;

            yield return new WaitForSeconds(unit.tempoAndando);

            paused = true;

            yield return new WaitForSeconds(unit.tempoPausado);
        }
    }

    void StopWalkingAnimation()
    {
        if (unit.animator != null)
        {
            unit.animator.SetBool("walking", false);
        }
    }
}