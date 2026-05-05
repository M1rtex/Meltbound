using UnityEngine;

public class ItemHeal : MonoBehaviour
{
    public float healAmount = 10f; 
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