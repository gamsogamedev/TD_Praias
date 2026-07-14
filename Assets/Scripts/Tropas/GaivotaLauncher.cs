using System.Collections;
using UnityEngine;

public class GaivotaLauncher : MonoBehaviour
{
    private GameObject gaivotaPrefab;
    private float alcance;
    private float intervalo;
    private int dano;
    private float raioExplosao;
    private GameObject efeitoImpacto;

    private bool pronto = false;

    // Chamado pelo Tower.cs ao iniciar e ao evoluir
    public void AplicarNivel(
        GameObject prefab,
        float alcanceDisparo,
        float intervaloDisparo,
        int danoExplosao,
        float raio,
        GameObject efeito)
    {
        gaivotaPrefab = prefab;
        alcance = alcanceDisparo;
        intervalo = intervaloDisparo;
        dano = danoExplosao;
        raioExplosao = raio;
        efeitoImpacto = efeito;

        // Reinicia o ciclo de disparo
        StopAllCoroutines();
        StartCoroutine(CicloDisparo());
    }

    IEnumerator CicloDisparo()
    {
        // Pequeno delay inicial para não atirar imediatamente ao construir
        yield return new WaitForSeconds(1f);

        while (true)
        {
            Unit alvo = BuscarAlvoMaisProximo();

            if (alvo != null)
                Disparar(alvo);

            yield return new WaitForSeconds(intervalo);
        }
    }

    Unit BuscarAlvoMaisProximo()
    {
        Unit maisProximo = null;
        float menorDistancia = Mathf.Infinity;

        // Busca todos os inimigos na cena
        Unit[] unidades = FindObjectsByType<Unit>(FindObjectsSortMode.None);

        foreach (Unit u in unidades)
        {
            if (u.team != Team.Enemy)
                continue;

            if (!u.IsAlive())
                continue;

            float dist = Vector2.Distance(transform.position, u.transform.position);

            if (dist <= alcance && dist < menorDistancia)
            {
                menorDistancia = dist;
                maisProximo = u;
            }
        }

        return maisProximo;
    }

    void Disparar(Unit alvo)
    {
        if (gaivotaPrefab == null)
            return;

        GameObject obj = Instantiate(
            gaivotaPrefab,
            transform.position,
            Quaternion.identity
        );

        Gaivota gaivota = obj.GetComponent<Gaivota>();

        if (gaivota != null)
        {
            gaivota.Inicializar(
                alvo.transform.position,
                dano,
                raioExplosao,
                efeitoImpacto
            );
        }
    }

    // Desenha o alcance no editor para facilitar configuração
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, alcance);
    }
}