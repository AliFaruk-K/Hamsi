using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class HamsiScript : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed = 6f;
    [SerializeField] private float airDriftStrength = 2f;
    [SerializeField] private float airRotationSpeed = 5f;
    [SerializeField] private float seaEnterDelay = 0.15f;
    [SerializeField] private float wingDuration = 10f;
    [SerializeField] private float wingSpeed = 8f;
    [SerializeField] private DeepSeaManager deepSeaManager;
    [SerializeField] private float mapHalfWidth = 50f;

    private Vector2 moveInput;
    private bool inSea = false;
    private bool hasWings = false;
    private bool isFalling = false;
    private Coroutine wingCoroutine;

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void FixedUpdate()
    {
        WrapAround();

        isFalling = !inSea && !hasWings && rb.linearVelocity.y < -0.1f;

        if (inSea)
        {
            hasWings = false;
            rb.gravityScale = 0f;

            if (moveInput != Vector2.zero)
            {
                // Ývmeli hýzlan, max hýza kadar
                float targetSpeed = speed;
                float currentSpeed = rb.linearVelocity.magnitude;
                float newSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, 4f * Time.fixedDeltaTime);
                rb.linearVelocity = moveInput.normalized * newSpeed;

                float angle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle), 15f * Time.fixedDeltaTime);
            }
            else
            {
                // Input býrakýnca yavaþça dur
                rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, 5f * Time.fixedDeltaTime);
            }
        }
        else if (hasWings)
        {
            rb.gravityScale = 0f;

            if (moveInput != Vector2.zero)
            {
                rb.linearVelocity = moveInput.normalized * wingSpeed;
                float angle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle), 15f * Time.fixedDeltaTime);
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
        else if (isFalling)
        {
            rb.gravityScale = 2f;

            // Düþerken sadece hafif x etkisi
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x + moveInput.x * airDriftStrength * Time.fixedDeltaTime,
                rb.linearVelocity.y
            );

            if (rb.linearVelocity.sqrMagnitude > 0.1f)
            {
                float targetAngle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
                float smoothAngle = Mathf.LerpAngle(rb.rotation, targetAngle, airRotationSpeed * Time.fixedDeltaTime);
                rb.MoveRotation(smoothAngle);
            }
        }
        else
        {
            // Zýplama aþamasý
            rb.gravityScale = 2f;

            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x + moveInput.x * airDriftStrength * Time.fixedDeltaTime,
                rb.linearVelocity.y
            );

            if (rb.linearVelocity.sqrMagnitude > 0.1f)
            {
                float targetAngle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
                float smoothAngle = Mathf.LerpAngle(rb.rotation, targetAngle, airRotationSpeed * Time.fixedDeltaTime);
                rb.MoveRotation(smoothAngle);
            }
        }
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
        if (other.CompareTag("Sea"))
            StartCoroutine(SeaEnterDelay());

        if (other.CompareTag("DeepSea"))
            deepSeaManager.HamsiEnteredDeepSea();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Sea"))
        {
            if (transform.position.y > 0f)
                inSea = false;
        }

        if (other.CompareTag("DeepSea"))
            deepSeaManager.HamsiExitedDeepSea();
    }

    private IEnumerator SeaEnterDelay()
    {
        rb.gravityScale = 0f;
        yield return new WaitForSeconds(seaEnterDelay);
        inSea = true;
        Vector2 entryVelocity = rb.linearVelocity;
        rb.linearVelocity = entryVelocity * 0.4f;
    }

    public void ActivateWings()
    {
        if (wingCoroutine != null)
            StopCoroutine(wingCoroutine);
        wingCoroutine = StartCoroutine(WingPowerUp());
    }

    private IEnumerator WingPowerUp()
    {
        hasWings = true;
        float timer = wingDuration;

        while (timer > 0f)
        {
            if (inSea)
            {
                DeactivateWings();
                yield break;
            }
            timer -= Time.deltaTime;
            yield return null;
        }

        DeactivateWings();
    }

    private void DeactivateWings()
    {
        hasWings = false;
        ScoreManager.instance.OnPowerUpEnded();
        wingCoroutine = null;
    }

    public void Die()
    {
        GameUIManager.instance.ShowDeathScreen(ScoreManager.instance.GetScore());
        gameObject.SetActive(false);
    }
}