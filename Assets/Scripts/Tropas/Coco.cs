using System.Collections.Generic;
using UnityEngine;

public class Coco : MonoBehaviour
{
    private CocoOrbit orbit;
    private int dano;
    private float cooldown;

    private float raio;
    private float velocidade;
    private float anguloAtual;

    // Rastreia cooldown por inimigo individualmente
    private Dictionary<Health, float> cooldowns = new Dictionary<Health, float>();

    public void Inicializar(CocoOrbit orbitRef, int danoCoco, float cooldownDano)
    {
        orbit = orbitRef;
        dano = danoCoco;
        cooldown = cooldownDano;
    }

    public void ConfigurarOrbita(float raioOrbita, float vel, float anguloInicial)
    {
        raio = raioOrbita;
        velocidade = vel;
        anguloAtual = anguloInicial;
    }

    void Update()
    {
        Orbitar();
        AtualizarCooldowns();
    }

    void Orbitar()
    {
        if (orbit == null)
            return;

        anguloAtual += velocidade * Time.deltaTime;

        if (anguloAtual >= 360f)
            anguloAtual -= 360f;

        float rad = anguloAtual * Mathf.Deg2Rad;

        Vector3 offset = new Vector3(
            Mathf.Cos(rad) * raio,
            Mathf.Sin(rad) * raio,
            0f
        );

        transform.position = orbit.transform.position + offset;
    }

    void AtualizarCooldowns()
    {
        List<Health> expirados = new List<Health>();

        foreach (var par in cooldowns)
        {
            if (par.Key == null || Time.time >= par.Value)
                expirados.Add(par.Key);
        }

        foreach (var key in expirados)
            cooldowns.Remove(key);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        TentarAcertar(other);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        TentarAcertar(other);
    }

    void TentarAcertar(Collider2D other)
    {
        Unit unit = other.GetComponent<Unit>();

        if (unit == null)
            return;

        // Só acerta inimigos (team oposto à torre)
        Tower torre = orbit.GetComponent<Tower>();

        if (torre == null)
            return;

        // Cocos pertencem à torre do jogador, só acertam inimigos
        if (unit.team != Team.Enemy)
            return;

        Health health = unit.health;

        if (health == null || health.IsDead())
            return;

        // Verifica cooldown individual por inimigo
        if (cooldowns.ContainsKey(health))
            return;

        health.TakeDamage(dano);
        cooldowns[health] = Time.time + cooldown;
    }
}