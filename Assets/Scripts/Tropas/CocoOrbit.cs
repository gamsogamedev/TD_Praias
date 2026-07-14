using System.Collections.Generic;
using UnityEngine;

public class CocoOrbit : MonoBehaviour
{
    private List<Coco> cocosAtivos = new List<Coco>();

    private GameObject cocoPrefab;
    private int quantidade;
    private float velocidade;
    private float raio;
    private int dano;
    private float cooldown;

    // Chamado pelo Tower.cs ao iniciar e ao evoluir
    public void AplicarNivel(
        GameObject prefab,
        int qtd,
        float vel,
        float raioOrbita,
        int danoCoco,
        float cooldownDano)
    {
        cocoPrefab = prefab;
        quantidade = qtd;
        velocidade = vel;
        raio = raioOrbita;
        dano = danoCoco;
        cooldown = cooldownDano;

        AtualizarCocos();
    }

    void AtualizarCocos()
    {
        // Remove cocos excedentes
        while (cocosAtivos.Count > quantidade)
        {
            Coco ultimo = cocosAtivos[cocosAtivos.Count - 1];
            cocosAtivos.RemoveAt(cocosAtivos.Count - 1);

            if (ultimo != null)
                Destroy(ultimo.gameObject);
        }

        // Adiciona cocos faltantes
        while (cocosAtivos.Count < quantidade)
        {
            if (cocoPrefab == null)
                break;

            GameObject obj = Instantiate(
                cocoPrefab,
                transform.position,
                Quaternion.identity,
                transform // filho da torre
            );

            Coco coco = obj.GetComponent<Coco>();

            if (coco != null)
            {
                cocosAtivos.Add(coco);
                coco.Inicializar(this, dano, cooldown);
            }
        }

        // Atualiza configurações e ângulos de todos os cocos
        ReposicionarCocos();
    }

    void ReposicionarCocos()
    {
        int total = cocosAtivos.Count;

        for (int i = 0; i < total; i++)
        {
            if (cocosAtivos[i] == null)
                continue;

            // Distribui os cocos igualmente em círculo
            float anguloInicial = (360f / total) * i;
            cocosAtivos[i].ConfigurarOrbita(raio, velocidade, anguloInicial);
        }
    }
}