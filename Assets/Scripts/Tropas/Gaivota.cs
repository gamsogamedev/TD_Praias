using System.Collections;
using UnityEngine;

public class Gaivota : MonoBehaviour
{
    [Header("Arco")]
    [Tooltip("Altura máxima da parábola")]
    public float alturaArco = 2f;

    [Tooltip("Velocidade do voo")]
    public float velocidade = 4f;

    private Vector3 origem;
    private Vector3 destino;
    private int dano;
    private float raioExplosao;
    private GameObject efeitoImpacto;

    private float progresso = 0f;
    private bool explodiu = false;

    public void Inicializar(
        Vector3 posicaoAlvo,
        int danoExplosao,
        float raio,
        GameObject efeito)
    {
        origem = transform.position;
        destino = posicaoAlvo;
        dano = danoExplosao;
        raioExplosao = raio;
        efeitoImpacto = efeito;
    }

    void Update()
    {
        if (explodiu)
            return;

        Voar();
    }

    void Voar()
    {
        float distanciaTotal = Vector3.Distance(origem, destino);

        if (distanciaTotal <= 0f)
        {
            Explodir();
            return;
        }

        // Avança o progresso baseado na velocidade
        progresso += (velocidade / distanciaTotal) * Time.deltaTime;
        progresso = Mathf.Clamp01(progresso);

        // Posição linear entre origem e destino
        Vector3 posicaoLinear = Vector3.Lerp(origem, destino, progresso);

        // Adiciona o arco em Y (parábola)
        float arco = alturaArco * Mathf.Sin(progresso * Mathf.PI);
        transform.position = posicaoLinear + new Vector3(0f, arco, 0f);

        // Rotaciona a gaivota na direção do movimento
        if (progresso < 1f)
        {
            Vector3 proximaPosicao = Vector3.Lerp(origem, destino, progresso + 0.01f);
            proximaPosicao.y += alturaArco * Mathf.Sin((progresso + 0.01f) * Mathf.PI);

            Vector3 direcao = proximaPosicao - transform.position;

            if (direcao != Vector3.zero)
            {
                float angulo = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, angulo);
            }
        }

        if (progresso >= 1f)
            Explodir();
    }

    void Explodir()
    {
        if (explodiu)
            return;

        explodiu = true;

        // Spawna efeito visual de impacto
        if (efeitoImpacto != null)
        {
            GameObject efeito = Instantiate(
                efeitoImpacto,
                transform.position,
                Quaternion.identity
            );

            // Destroi o efeito automaticamente se tiver duração definida
            ParticleSystem ps = efeito.GetComponent<ParticleSystem>();
            if (ps != null)
                Destroy(efeito, ps.main.duration + ps.main.startLifetime.constantMax);
            else
                Destroy(efeito, 2f);
        }

        // Aplica dano em área em todos os inimigos no raio
        Unit[] unidades = FindObjectsByType<Unit>(FindObjectsSortMode.None);

        foreach (Unit u in unidades)
        {
            if (u.team != Team.Enemy)
                continue;

            if (!u.IsAlive())
                continue;

            float dist = Vector2.Distance(transform.position, u.transform.position);

            if (dist <= raioExplosao)
            {
                u.health.TakeDamage(dano);
            }
        }

        Destroy(gameObject);
    }

    // Mostra o raio de explosão no editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, raioExplosao);
    }
}