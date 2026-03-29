using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;        // ลาก Capsule มาใส่ช่องนี้ใน Inspector
    public Vector3 offset = new Vector3(0, 10, 0);  // ระยะห่างจากผู้เล่น

    void LateUpdate()
    {
        // กล้องตามตำแหน่งผู้เล่น แต่ไม่หมุนตาม
        transform.position = target.position + offset;
    }
}