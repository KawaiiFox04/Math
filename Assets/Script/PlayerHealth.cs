using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;

    [Header("Events")]
    public UnityEvent onDeath;                   // ลากใส่ใน Inspector ได้เลย
    public UnityEvent<int, int> onHealthChanged; // (currentHP, maxHP)

    private int currentHealth;
    private bool isDead = false;

    public int CurrentHealth => currentHealth;
    public bool IsDead       => isDead;

    void Start()
    {
        currentHealth = maxHealth;
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    // ============================================================
    //  Public API — เรียกจาก EnemyAI หรือ script อื่นๆ
    // ============================================================
    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Max(currentHealth - amount, 0);
        Debug.Log($"[PlayerHealth] HP: {currentHealth} / {maxHealth}  (took {amount} dmg)");

        onHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
            Die();
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log($"[PlayerHealth] Healed → HP: {currentHealth} / {maxHealth}");

        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    // ============================================================
    //  Death
    // ============================================================
    void Die()
    {
        isDead = true;
        Debug.Log("[PlayerHealth] Player has died!");

        onDeath?.Invoke();

        // ตัวอย่างสิ่งที่ทำตอนตาย — เปิด / ปิดตามต้องการ
        // Destroy(gameObject);
        // GetComponent<PlayerMovement>().enabled = false;
        // SceneManager.LoadScene("GameOver");
    }

    // ============================================================
    //  Debug — กด G ตอน Play เพื่อทดสอบ
    // ============================================================
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
            TakeDamage(1);
    }
}