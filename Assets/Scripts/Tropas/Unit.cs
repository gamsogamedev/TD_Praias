using UnityEngine;

[RequireComponent(typeof(Health))]
public class Unit : MonoBehaviour
{
    [Header("Informações")]
    public Team team;

    [Header("Movimentação")]
    public float moveSpeed = 2f;

    [Header("Combate")]
    public int damage = 1;

    [Tooltip("Ataques por segundo")]
    public float attackRate = 1f;

    [Tooltip("Distância máxima para atacar")]
    public float attackRange = 0.6f;

    [Tooltip("Marque caso seja uma unidade à distância")]
    public bool isRanged = false;

    [Header("Base")]
    public int baseDamage = 1;

    [Header("Comportamento")]
    public bool movimentoIntermitente = false;
    public float tempoAndando = 3f;
    public float tempoPausado = 1f;

    [Header("Referências")]
    public Animator animator;

    [Header("Recompensa")]
    public int goldReward = 5;

    [HideInInspector]
    public Health health;

    [HideInInspector]
    public UnitMovement movement;

    [HideInInspector]
    public UnitCombat combat;

    [HideInInspector]
    public UnitTargeting targeting;

    private void Awake()
    {
        health = GetComponent<Health>();

        movement = GetComponent<UnitMovement>();
        combat = GetComponent<UnitCombat>();
        targeting = GetComponent<UnitTargeting>();

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    public bool IsAlive()
    {
        return health != null && health.GetHealth() > 0;
    }
}