using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Vision Settings")]
    public float visionRadius = 5f;          // ระยะมองเห็นรอบตัว
    public float visionAngle = 90f;          // องศา Field of View (กรวยการมอง)
    public Color visionColor = new Color(1f, 1f, 0f, 0.3f);      // สีพื้นที่มองเห็น
    public Color visionOutlineColor = new Color(1f, 1f, 0f, 1f);  // สีขอบ
    public Color blindZoneColor = new Color(1f, 0f, 0f, 0.08f);   // สีด้านหลัง (จุดตาย)

    private Rigidbody rb;
    private Vector3 moveDirection;

    // ทิศที่ผู้เล่นหันหน้าอยู่ล่าสุด (ใช้วาด Gizmo)
    private Vector3 lastFacingDir = Vector3.forward;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.constraints = RigidbodyConstraints.FreezeRotationX
                       | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        // อัปเดตทิศหันหน้าเมื่อกำลังเดิน
        if (moveDirection != Vector3.zero)
            lastFacingDir = moveDirection;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(
            moveDirection.x * moveSpeed,
            rb.linearVelocity.y,
            moveDirection.z * moveSpeed
        );
    }

    // ========== GIZMOS ==========

    void OnDrawGizmos()
    {
        DrawVisionGizmo();
    }

    void DrawVisionGizmo()
    {
        Vector3 origin = transform.position;

        // ใช้ทิศที่กำลังเดิน หรือ forward ถ้ายืนอยู่
        Vector3 facing = Application.isPlaying ? lastFacingDir : transform.forward;
        facing.y = 0f;
        if (facing == Vector3.zero) facing = Vector3.forward;
        facing.Normalize();

        float halfAngle = visionAngle / 2f;
        int segments = 40; // ความละเอียดของส่วนโค้ง

        // ---- 1. วาดพื้นที่มองเห็น (กรวย FOV) ----
        Gizmos.color = visionColor;
        DrawFilledArc(origin, facing, halfAngle, visionRadius, segments);

        // ---- 2. วาดพื้นที่มองไม่เห็นด้านหลัง ----
        Gizmos.color = blindZoneColor;
        DrawFilledArc(origin, -facing, 180f - halfAngle, visionRadius, segments);

        // ---- 3. วาดขอบกรวย FOV ----
        Gizmos.color = visionOutlineColor;

        Vector3 leftBound = RotateY(facing, -halfAngle) * visionRadius;
        Vector3 rightBound = RotateY(facing, halfAngle) * visionRadius;

        Gizmos.DrawLine(origin, origin + leftBound);
        Gizmos.DrawLine(origin, origin + rightBound);

        // วาดส่วนโค้งขอบ FOV
        DrawArcOutline(origin, facing, halfAngle, visionRadius, segments);

        // ---- 4. วาดวงกลมระยะรับรู้รอบตัว (เส้นประ) ----
        Gizmos.color = new Color(visionOutlineColor.r, visionOutlineColor.g, visionOutlineColor.b, 0.4f);
        DrawCircle(origin, visionRadius, 60);

        // ---- 5. จุดที่ผู้เล่นยืน ----
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(origin, 0.08f);
    }

    // วาดพัดกลม (Filled Arc) บน XZ plane
    void DrawFilledArc(Vector3 center, Vector3 dir, float halfAngle, float radius, int segments)
    {
        Vector3 prev = center;
        for (int i = 0; i <= segments; i++)
        {
            float t = (float)i / segments;
            float angle = Mathf.Lerp(-halfAngle, halfAngle, t);
            Vector3 point = center + RotateY(dir, angle) * radius;

            if (i > 0)
            {
                // จำลอง filled ด้วย triangle fan (DrawLine หลายเส้นแทน mesh)
                Gizmos.DrawLine(center, point);
                Gizmos.DrawLine(prev, point);
            }
            prev = point;
        }
    }

    // วาดเฉพาะเส้นโค้ง Arc
    void DrawArcOutline(Vector3 center, Vector3 dir, float halfAngle, float radius, int segments)
    {
        Vector3 prev = Vector3.zero;
        for (int i = 0; i <= segments; i++)
        {
            float t = (float)i / segments;
            float angle = Mathf.Lerp(-halfAngle, halfAngle, t);
            Vector3 point = center + RotateY(dir, angle) * radius;
            if (i > 0) Gizmos.DrawLine(prev, point);
            prev = point;
        }
    }

    // วาดวงกลมรอบจุด
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

    // หมุน Vector บน XZ plane
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