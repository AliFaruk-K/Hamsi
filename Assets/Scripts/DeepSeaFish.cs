using UnityEngine;

public class DeepSeaFish : MonoBehaviour
{
    public enum FishState { Patrol, Chase, Return }

    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float patrolRadius = 3f;
    [SerializeField] private bool spriteFacesRight = true;

    private FishState state = FishState.Patrol;
    private Transform target;
    private Vector2 patrolTarget;
    private Rigidbody2D rb;
    private Vector2 startPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        startPosition = transform.position;
        SetNewPatrolTarget();
    }

    void Update()
    {
        switch (state)
        {
            case FishState.Patrol: Patrol(); break;
            case FishState.Chase: Chase(); break;
            case FishState.Return: ReturnToPatrol(); break;
        }
    }

    void Patrol()
    {
        MoveTowards(patrolTarget, patrolSpeed);
        if (Vector2.Distance(transform.position, patrolTarget) < 0.3f)
            SetNewPatrolTarget();
    }

    void Chase()
    {
        if (target == null) { state = FishState.Return; return; }
        MoveTowards(target.position, chaseSpeed);
    }

    void ReturnToPatrol()
    {
        MoveTowards(startPosition, patrolSpeed);
        if (Vector2.Distance(transform.position, startPosition) < 0.5f)
        {
            SetNewPatrolTarget();
            state = FishState.Patrol;
        }
    }

    void MoveTowards(Vector2 destination, float speed)
    {
        Vector2 dir = (destination - (Vector2)transform.position).normalized;
        if (dir == Vector2.zero) return;

        rb.linearVelocity = dir * speed;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (!spriteFacesRight) angle += 180f;

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.Euler(0, 0, angle),
            10f * Time.deltaTime
        );
    }

    void SetNewPatrolTarget()
    {
        float randomX = Random.Range(-patrolRadius, patrolRadius);
        float randomY = Random.Range(-patrolRadius * 0.3f, patrolRadius * 0.3f);
        patrolTarget = startPosition + new Vector2(randomX, randomY);
    }

    public void StartChase(Transform hamsi)
    {
        target = hamsi;
        state = FishState.Chase;
    }

    public void StopChase()
    {
        target = null;
        state = FishState.Return;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HamsiScript hamsi = other.GetComponent<HamsiScript>();
            if (hamsi != null)
                hamsi.Die();
        }
    }
}