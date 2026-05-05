using UnityEngine;

public class KeyItem : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GameManager.Instance.CollectKey();
        Destroy(gameObject);
    }
}