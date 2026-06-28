using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Unit))]
public class UnitTargeting : MonoBehaviour
{
    private Unit unit;

    [Header("Alvo Atual")]
    [SerializeField]
    private Unit currentTarget;

    [Header("Inimigos em Alcance")]
    public List<Unit> enemiesInRange = new List<Unit>();

    void Awake()
    {
        unit = GetComponent<Unit>();
    }

    void Update()
    {
        LimparLista();

        if (currentTarget == null)
        {
            ProcurarNovoAlvo();
        }
    }

    void LimparLista()
    {
        enemiesInRange.RemoveAll(enemy =>
            enemy == null ||
            !enemy.IsAlive());
    }

    void ProcurarNovoAlvo()
    {
        float menorDistancia = Mathf.Infinity;
        Unit melhorAlvo = null;

        foreach (Unit enemy in enemiesInRange)
        {
            float distancia =
                Vector2.Distance(
                    transform.position,
                    enemy.transform.position);

            if (distancia < menorDistancia)
            {
                menorDistancia = distancia;
                melhorAlvo = enemy;
            }
        }

        currentTarget = melhorAlvo;
    }
    public Unit GetTarget()
    {
    return currentTarget;
    }

    public bool HasTarget()
    {
        return currentTarget != null;
    }

    public void ClearTarget()
    {
        currentTarget = null;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Unit enemy = other.GetComponent<Unit>();

        if (enemy == null)
            return;

        if (enemy.team == unit.team)
            return;

        if (!enemiesInRange.Contains(enemy))
        {
            enemiesInRange.Add(enemy);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Unit enemy = other.GetComponent<Unit>();

        if (enemy == null)
            return;

        enemiesInRange.Remove(enemy);

        if (currentTarget == enemy)
        {
            currentTarget = null;
        }
    }
}