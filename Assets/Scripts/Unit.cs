using UnityEngine;

public class Unit : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 2f;

    private Transform[] path;
    private int currentPointIndex = 0;

    [Header("Combat")]
    public string enemyTag;
    public string towerType;
    public float tempoDeDestruir = 2f;
    private bool atacando = false;

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
        if (atacando) 
            return;

        if (path == null || currentPointIndex >= path.Length)
            return;

        Transform target = path[currentPointIndex];

        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        RotateTowards(target);

        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            currentPointIndex++;

            if (currentPointIndex >= path.Length)
            {
                ReachedEnd();
            }
        }
    }

    void RotateTowards(Transform target)
    {
        Vector2 direction = target.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void ReachedEnd()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("O inimigo bateu em: " + other.gameObject.name + "Tag: " + other.tag);
        if (other.CompareTag(enemyTag) || other.CompareTag(towerType))
        {
            atacando = true;
            Destroy(other.gameObject, tempoDeDestruir);
            Destroy(gameObject, tempoDeDestruir);
        }
    }
}