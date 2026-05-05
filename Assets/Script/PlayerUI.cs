using UnityEngine;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [Header("HP Text")]
    public TMP_Text hpText;

    [Header("Game Over")]
    public TMP_Text gameOverText;

    void Start()
    {
        if (gameOverText != null)
            gameOverText.gameObject.SetActive(false);
    }
    
    public void OnHealthChanged(int current, int max)
    {
        if (hpText != null)
            hpText.text = $"HP: {current}";
    }
    
    public void OnPlayerDied()
    {
        if (hpText != null)
            hpText.gameObject.SetActive(false); 

        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);
            gameOverText.text = "GAME OVER";
        }
    }
}