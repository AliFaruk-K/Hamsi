using UnityEngine;

public class BirdScript : MonoBehaviour
{
    [SerializeField] private float swimSpeed = 3f;
    [SerializeField] private float fleeSpeed = 6f;
    [SerializeField] private float fleeRadius = 2f;
    [SerializeField] private float directionChangeInterval = 2f;
    [SerializeField] private float minY = 2f;   // gökyüzünün alt sýnýrý
    [SerializeField] private float maxY = 10f;  // gökyüzünün üst sýnýrý
    [SerializeField] private float mapHalfWidth = 50f;

    private Vector2 moveDirection;
    private float directionTimer;
    private Transform hamsi;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        hamsi = GameObject.FindWithTag("Player").transform;
        SetRandomDirection();
    }

    void Update()
    {
        directionTimer -= Time.deltaTime;
        if (directionTimer <= 0f)
            SetRandomDirection();

        float distToHamsi = Vector2.Distance(transform.position, hamsi.position);

        Vector2 moveDir;
        float speed;

        if (distToHamsi < fleeRadius)
        {
            moveDir = ((Vector2)transform.position - (Vector2)hamsi.position).normalized;
            speed = fleeSpeed;
        }
        else
        {
            moveDir = moveDirection;
            speed = swimSpeed;
        }

        // Gökyüzü sýnýrlarýnda tut
        float posY = transform.position.y;
        if (posY >= maxY && moveDir.y > 0f) moveDir.y = 0f;
        if (posY <= minY && moveDir.y < 0f) moveDir.y = 0f;

        rb.linearVelocity = moveDir * speed;

        if (moveDir != Vector2.zero)
        {
            float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle), 10f * Time.deltaTime);
        }

        WrapAround();
    }

    void SetRandomDirection()
    {
        float x = Random.Range(-1f, 1f);
        float y = Random.Range(-0.3f, 0.3f);
        moveDirection = new Vector2(x, y).normalized;
        directionTimer = directionChangeInterval;
    }

    void WrapAround()
    {
        Vector3 pos = transform.position;
        float buffer = 1.5f;
        if (pos.x > mapHalfWidth + buffer)
            transform.position = new Vector3(-mapHalfWidth - buffer, pos.y, pos.z);
        else if (pos.x < -mapHalfWidth - buffer)
            transform.position = new Vector3(mapHalfWidth + buffer, pos.y, pos.z);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            BirdSpawner.instance.OnBirdEaten(this);
    }
}