using UnityEngine;

public class ItemHeal : MonoBehaviour
{
    public float healAmount = 10f;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip healSound;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HealthBar hb = other.GetComponent<HealthBar>();
            if (hb != null)
            {
                hb.Heal(healAmount);

                HealVignetteEffect vignetteEffect = FindObjectOfType<HealVignetteEffect>();
                if (vignetteEffect != null)
                {
                    vignetteEffect.TriggerHealEffect();
                }

                if (healSound != null && Camera.main != null)
                {
                    AudioSource.PlayClipAtPoint(healSound, Camera.main.transform.position);
                }

                Destroy(gameObject);
            }
        }
    }
}