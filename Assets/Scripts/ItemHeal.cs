using UnityEngine;

public class ItemHeal : MonoBehaviour
{
    public float healAmount = 10f; // Сколько лечит эта аптечка

    // Метод срабатывает, когда что-то входит в триггер аптечки
    // Обязательно добавь "2D" в название метода и тип аргумента
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))
        {
            HealthBar hb = other.GetComponent<HealthBar>();
            if (hb != null)
            {
                hb.Heal(healAmount);
                Destroy(gameObject);
            }
        }
    }
}