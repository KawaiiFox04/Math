using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Key Settings")]
    public int totalKeys = 3;

    [Header("UI")]
    public TMP_Text keyCountText;    
    public TMP_Text messageText;     

    private int collectedKeys = 0;
    private bool gameEnded = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateKeyUI();

        if (messageText != null)
            messageText.gameObject.SetActive(false);
    }
    
    public void CollectKey()
    {
        if (gameEnded) return;

        collectedKeys++;
        Debug.Log($"[GameManager] Keys: {collectedKeys} / {totalKeys}");
        UpdateKeyUI();
    }

    public bool HasAllKeys() => collectedKeys >= totalKeys;
    
    public void Victory()
    {
        if (gameEnded) return;
        gameEnded = true;

        ShowMessage("VICTORY!");
        StopGame();
        Debug.Log("[GameManager] Victory!");
    }

    public void ShowKeyWarning()
    {
        if (gameEnded) return;

        ShowMessage($"You need all {totalKeys} keys to open this door!\n({collectedKeys} / {totalKeys} collected)");
        Debug.Log("[GameManager] Not enough keys.");
    }
    
    void UpdateKeyUI()
    {
        if (keyCountText != null)
            keyCountText.text = $"Keys: {collectedKeys} / {totalKeys}";
    }

    void ShowMessage(string msg)
    {
        if (messageText == null) return;

        messageText.gameObject.SetActive(true);
        messageText.text = msg;
    }

    void StopGame()
    {
        Time.timeScale = 0f;
    }
}