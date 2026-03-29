using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    private Rigidbody rb;
    private Vector3 moveDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // ล็อคการหมุนของ Rigidbody ไม่ให้ล้ม
        rb.constraints = RigidbodyConstraints.FreezeRotationX 
                       | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        // รับ Input จาก WASD
        float horizontal = Input.GetAxisRaw("Horizontal"); // A/D
        float vertical = Input.GetAxisRaw("Vertical");     // W/S

        // Top-down: เดินบน XZ plane
        moveDirection = new Vector3(horizontal, 0f, vertical).normalized;
    }

    void FixedUpdate()
    {
        // เคลื่อนที่ด้วย Rigidbody
        rb.linearVelocity = new Vector3(
            moveDirection.x * moveSpeed,
            rb.linearVelocity.y,  // คง gravity ไว้
            moveDirection.z * moveSpeed
        );
    }
}