using UnityEngine;

public class SmallFish : MonoBehaviour
{
    [SerializeField] private float swimSpeed = 2f;
    [SerializeField] private float fleeSpeed = 5f;
    [SerializeField] private float fleeRadius = 2f;
    [SerializeField] private float directionChangeInterval = 2f;
    [SerializeField] private float seaSurfaceY = 0f;
    [SerializeField] private float seaBottomY = -10f;
    private float mapHalfWidth = 50f; // haritanýn yarý geniþliði, sahneye göre ayarla
    private Vector2 swimDirection;
    private float directionTimer;
    private Transform hamsi;
    private Rigidbody2D rb;
    private float mapWidth;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        hamsi = GameObject.FindWithTag("Player").transform;
        mapWidth = mapHalfWidth;
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
            moveDir = swimDirection;
            speed = swimSpeed;
        }

        float posY = transform.position.y;
        if (posY >= seaSurfaceY - 0.1f && moveDir.y > 0f)
            moveDir.y = 0f;
        if (posY <= seaBottomY + 0.1f && moveDir.y < 0f)
            moveDir.y = 0f;

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
        swimDirection = new Vector2(x, y).normalized;
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
            FishSpawner.instance.OnFishEaten(this);
    }
}