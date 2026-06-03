using UnityEngine;

/// <summary>
/// Вспомогательный класс для быстрой настройки WindZone2D
/// </summary>
public static class WindZone2DHelper
{
    /// <summary>
    /// Создаёт WindZone2D с заданными параметрами
    /// </summary>
    public static GameObject CreateWindZone(Vector3 position, Vector2 size, Vector2 windDirection, float windStrength = 5f)
    {
        GameObject windZone = new GameObject("WindZone2D");
        windZone.transform.position = position;

        WindZone2D wind = windZone.AddComponent<WindZone2D>();
        BoxCollider2D collider = windZone.GetComponent<BoxCollider2D>();
        collider.size = size;

        wind.SetWindDirection(windDirection);
        wind.SetWindStrength(windStrength);

        return windZone;
    }

    /// <summary>
    /// Настраивает цвет партиклов ветра
    /// </summary>
    public static void SetWindColor(WindZone2D windZone, Color color)
    {
        ParticleSystem ps = windZone.GetComponentInChildren<ParticleSystem>();
        if (ps != null)
        {
            var main = ps.main;
            main.startColor = color;

            var renderer = ps.GetComponent<ParticleSystemRenderer>();
            if (renderer != null && renderer.material != null)
            {
                renderer.material.SetColor("_Color", color);
            }
        }
    }

    /// <summary>
    /// Настраивает интенсивность визуализации ветра
    /// </summary>
    public static void SetWindVisualIntensity(WindZone2D windZone, float intensity)
    {
        ParticleSystem ps = windZone.GetComponentInChildren<ParticleSystem>();
        if (ps != null)
        {
            var emission = ps.emission;
            emission.rateOverTime = 40f * intensity;

            var main = ps.main;
            Color currentColor = main.startColor.color;
            currentColor.a = Mathf.Clamp01(0.4f * intensity);
            main.startColor = currentColor;
        }
    }

    /// <summary>
    /// Предустановки для разных типов ветра
    /// </summary>
    public enum WindPreset
    {
        LightBreeze,
        ModerateWind,
        StrongWind,
        Storm
    }

    public static void ApplyPreset(WindZone2D windZone, WindPreset preset)
    {
        switch (preset)
        {
            case WindPreset.LightBreeze:
                windZone.SetWindStrength(3f);
                SetWindVisualIntensity(windZone, 0.6f);
                SetWindColor(windZone, new Color(0.9f, 0.95f, 1f, 0.25f));
                break;

            case WindPreset.ModerateWind:
                windZone.SetWindStrength(7f);
                SetWindVisualIntensity(windZone, 1f);
                SetWindColor(windZone, new Color(0.8f, 0.9f, 1f, 0.4f));
                break;

            case WindPreset.StrongWind:
                windZone.SetWindStrength(12f);
                SetWindVisualIntensity(windZone, 1.5f);
                SetWindColor(windZone, new Color(0.7f, 0.85f, 1f, 0.5f));
                break;

            case WindPreset.Storm:
                windZone.SetWindStrength(20f);
                SetWindVisualIntensity(windZone, 2f);
                SetWindColor(windZone, new Color(0.6f, 0.7f, 0.8f, 0.6f));
                break;
        }
    }
}
