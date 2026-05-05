using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;

    [Header("Events")]
    public UnityEvent onDeath;                   
    public UnityEvent<int, int> onHealthChanged; 

    private int currentHealth;
    private bool isDead = false;

    public int CurrentHealth => currentHealth;
    public bool IsDead       => isDead;

    void Start()
    {
        currentHealth = maxHealth;
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    
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
    
    void Die()
    {
        isDead = true;
        Debug.Log("[PlayerHealth] Player has died!");

        onDeath?.Invoke();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
            TakeDamage(1);
    }
}