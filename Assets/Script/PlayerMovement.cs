using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Vision Settings")]
    public float visionRadius = 5f;
    public float visionAngle = 90f;
    public Color visionColor = new Color(1f, 1f, 0f, 0.3f);
    public Color visionOutlineColor = new Color(1f, 1f, 0f, 1f);
    public Color blindZoneColor = new Color(1f, 0f, 0f, 0.08f);

    private Rigidbody rb;
    private Vector3 moveDirection;
    private Vector3 lastFacingDir = Vector3.forward;
    
    private PlayerHealth playerHealth;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX
                       | RigidbodyConstraints.FreezeRotationZ;

        playerHealth = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        if (playerHealth != null && playerHealth.IsDead) return;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical   = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        if (moveDirection != Vector3.zero)
            lastFacingDir = moveDirection;
    }

    void FixedUpdate()
    {
        if (playerHealth != null && playerHealth.IsDead)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            return;
        }

        rb.linearVelocity = new Vector3(
            moveDirection.x * moveSpeed,
            rb.linearVelocity.y,
            moveDirection.z * moveSpeed
        );
    }
    
    void OnDrawGizmos()
    {
        DrawVisionGizmo();
    }

    void DrawVisionGizmo()
    {
        Vector3 origin = transform.position;

        Vector3 facing = Application.isPlaying ? lastFacingDir : transform.forward;
        facing.y = 0f;
        if (facing == Vector3.zero) facing = Vector3.forward;
        facing.Normalize();

        float halfAngle = visionAngle / 2f;
        int segments = 40;

        Gizmos.color = visionColor;
        DrawFilledArc(origin, facing, halfAngle, visionRadius, segments);

        Gizmos.color = blindZoneColor;
        DrawFilledArc(origin, -facing, 180f - halfAngle, visionRadius, segments);

        Gizmos.color = visionOutlineColor;

        Vector3 leftBound  = RotateY(facing, -halfAngle) * visionRadius;
        Vector3 rightBound = RotateY(facing,  halfAngle) * visionRadius;

        Gizmos.DrawLine(origin, origin + leftBound);
        Gizmos.DrawLine(origin, origin + rightBound);

        DrawArcOutline(origin, facing, halfAngle, visionRadius, segments);

        Gizmos.color = new Color(visionOutlineColor.r, visionOutlineColor.g, visionOutlineColor.b, 0.4f);
        DrawCircle(origin, visionRadius, 60);

        Gizmos.color = Color.white;
        Gizmos.DrawSphere(origin, 0.08f);
    }

    void DrawFilledArc(Vector3 center, Vector3 dir, float halfAngle, float radius, int segments)
    {
        Vector3 prev = center;
        for (int i = 0; i <= segments; i++)
        {
            float t     = (float)i / segments;
            float angle = Mathf.Lerp(-halfAngle, halfAngle, t);
            Vector3 point = center + RotateY(dir, angle) * radius;

            if (i > 0)
            {
                Gizmos.DrawLine(center, point);
                Gizmos.DrawLine(prev, point);
            }
            prev = point;
        }
    }

    void DrawArcOutline(Vector3 center, Vector3 dir, float halfAngle, float radius, int segments)
    {
        Vector3 prev = Vector3.zero;
        for (int i = 0; i <= segments; i++)
        {
            float t     = (float)i / segments;
            float angle = Mathf.Lerp(-halfAngle, halfAngle, t);
            Vector3 point = center + RotateY(dir, angle) * radius;
            if (i > 0) Gizmos.DrawLine(prev, point);
            prev = point;
        }
    }

    void DrawCircle(Vector3 center, float radius, int segments)
    {
        Vector3 prev = center + new Vector3(radius, 0f, 0f);
        for (int i = 1; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            Vector3 next = center + new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * radius;
            Gizmos.DrawLine(prev, next);
            prev = next;
        }
    }

    Vector3 RotateY(Vector3 dir, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        return new Vector3(
            dir.x * cos - dir.z * sin,
            0f,
            dir.x * sin + dir.z * cos
        );
    }
}