using UnityEngine;

public class Door : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (GameManager.Instance.HasAllKeys())
            GameManager.Instance.Victory();
        else
            GameManager.Instance.ShowKeyWarning();
    }
}