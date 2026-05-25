using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float minY = -5f;  // kameranýn inebileceði min Y
    [SerializeField] private float maxY = 5f;   // kameranýn çýkabileceði max Y
    [SerializeField] private float mapHalfWidth = 50f; // haritanýn yarý geniþliði, sahneye göre ayarla
    private float halfScreenWidth;

    void Start()
    {
        halfScreenWidth = Camera.main.orthographicSize * Camera.main.aspect;
    }

    void LateUpdate()
    {
        float clampedX = Mathf.Clamp(
            target.position.x,
            -mapHalfWidth + halfScreenWidth,
            mapHalfWidth - halfScreenWidth
        );

        float clampedY = Mathf.Clamp(target.position.y, minY, maxY);

        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }
}