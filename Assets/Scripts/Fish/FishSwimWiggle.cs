using UnityEngine;

public class FishSwimWiggle : MonoBehaviour
{
    [Header("Movement")]
    public float swimSpeed = 2f;
    public float waveAmplitude = 0.2f;
    public float waveFrequency = 2f;
    public float swimRange = 3f; // how far left/right from start position

    [Header("Wiggle (child only)")]
    public Transform graphicTransform;
    public float wiggleAngle = 10f;
    public float wiggleSpeed = 8f;
    public float wiggleScaleAmount = 0.05f;

    private float baseY;
    private float startX;
    private float leftLimit;
    private float rightLimit;
    private Vector3 graphicBaseScale;
    private int direction = 1;

    void Start()
    {
        baseY = transform.position.y;
        startX = transform.position.x;

        leftLimit = startX - swimRange;
        rightLimit = startX + swimRange;

        graphicBaseScale = graphicTransform.localScale;
    }

    void Update()
    {
        float t = Time.time;

        // --- Change direction based on relative limits ---
        if (transform.position.x > rightLimit)
            direction = -1;
        else if (transform.position.x < leftLimit)
            direction = 1;

        // --- Move ---
        float newY = baseY + Mathf.Sin(t * waveFrequency) * waveAmplitude;
        transform.position += Vector3.right * swimSpeed * direction * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // --- Flip root only ---
        Vector3 rootScale = transform.localScale;
        float magnitude = Mathf.Abs(rootScale.x);
        if (magnitude < 0.0001f) magnitude = 1f;

        rootScale.x = magnitude * direction;
        transform.localScale = rootScale;

        // --- Wiggle child only ---
        float angle = Mathf.Sin(t * wiggleSpeed) * wiggleAngle;
        graphicTransform.localRotation = Quaternion.Euler(0, 0, angle);

        float scaleOffset = Mathf.Sin(t * wiggleSpeed) * wiggleScaleAmount;
        graphicTransform.localScale = new Vector3(
            graphicBaseScale.x + scaleOffset,
            graphicBaseScale.y - scaleOffset,
            graphicBaseScale.z
        );
    }
}
