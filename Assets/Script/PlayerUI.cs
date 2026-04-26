using UnityEngine;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [Header("HP Text")]
    public TMP_Text hpText;          // ลาก TextMeshPro object ใส่ตรงนี้ใน Inspector

    [Header("Game Over")]
    public TMP_Text gameOverText;    // ลาก TextMeshPro object ใส่ตรงนี้ใน Inspector

    void Start()
    {
        // ซ่อน Game Over ตอนเริ่มเกม
        if (gameOverText != null)
            gameOverText.gameObject.SetActive(false);
    }

    // ============================================================
    //  ลาก function นี้ใส่ onHealthChanged ใน Inspector
    //  (รับ 2 ค่า: currentHP, maxHP)
    // ============================================================
    public void OnHealthChanged(int current, int max)
    {
        if (hpText != null)
            hpText.text = $"HP: {current}";
    }

    // ============================================================
    //  ลาก function นี้ใส่ onDeath ใน Inspector
    // ============================================================
    public void OnPlayerDied()
    {
        if (hpText != null)
            hpText.gameObject.SetActive(false); // ซ่อน HP ตอนตาย

        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);
            gameOverText.text = "GAME OVER";
        }
    }
}