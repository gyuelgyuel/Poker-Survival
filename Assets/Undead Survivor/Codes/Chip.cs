using UnityEngine;

public class Chip : MonoBehaviour
{
    public int amount;

    void OnEnable()
    {
        amount = 100;   // <- prefab 초기값 무시하고 고정
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.instance.GetChip(amount);
            gameObject.SetActive(false);
        }
    }
}
