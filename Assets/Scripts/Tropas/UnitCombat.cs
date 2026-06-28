using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Unit))]
[RequireComponent(typeof(UnitTargeting))]
public class UnitCombat : MonoBehaviour
{
    private Unit unit;
    private UnitTargeting targeting;

    private bool attacking = false;

    private void Awake()
    {
        unit = GetComponent<Unit>();
        targeting = GetComponent<UnitTargeting>();
    }

    private void Update()
    {
        if (!attacking && targeting.HasTarget())
        {
            StartCoroutine(AttackRoutine());
        }
    }

    IEnumerator AttackRoutine()
    {
        attacking = true;

        while (targeting.HasTarget())
        {
            Unit target = targeting.GetTarget();

            if (target == null || !target.IsAlive())
            {
                targeting.ClearTarget();
                break;
            }

            float distance = Vector2.Distance(
                transform.position,
                target.transform.position);

            if (distance > unit.attackRange)
            {
                // Aguarda sem desistir do alvo —
                // o UnitTargeting remove sozinho se sair do trigger
                yield return null;
                continue;
            }

            target.health.TakeDamage(unit.damage);

            StartCoroutine(HitEffect());

            yield return new WaitForSeconds(1f / unit.attackRate);
        }

        attacking = false;
    }

    IEnumerator HitEffect()
    {
        Vector3 originalScale = transform.localScale;
        transform.localScale = originalScale * 1.05f;
        yield return new WaitForSeconds(0.05f);
        transform.localScale = originalScale;
    }
}