using UnityEngine;
using UnityEditor;

public class SetupWindParticles
{
    public static void Execute()
    {
        GameObject windZone = GameObject.Find("WindZone2D");
        if (windZone == null)
        {
            Debug.LogError("WindZone2D not found!");
            return;
        }

        Transform particlesTransform = windZone.transform.Find("WindParticles");
        if (particlesTransform == null)
        {
            Debug.LogError("WindParticles not found!");
            return;
        }

        ParticleSystem ps = particlesTransform.GetComponent<ParticleSystem>();
        if (ps == null)
        {
            Debug.LogError("ParticleSystem component not found!");
            return;
        }

        // Main Module
        var main = ps.main;
        main.duration = 5f;
        main.loop = true;
        main.startLifetime = new ParticleSystem.MinMaxCurve(2f, 3f);
        main.startSpeed = 0f;
        main.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.1f);
        main.startColor = new Color(1f, 1f, 1f, 0.4f);
        main.gravityModifier = 0f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = 150;

        // Emission
        var emission = ps.emission;
        emission.rateOverTime = 40f;

        // Shape
        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(5f, 3f, 0.1f);

        // Velocity over Lifetime
        var velocity = ps.velocityOverLifetime;
        velocity.enabled = true;
        velocity.space = ParticleSystemSimulationSpace.World;
        velocity.x = 5f; // Wind direction right
        velocity.y = 0f;
        velocity.z = 0f;

        // Size over Lifetime
        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        AnimationCurve sizeCurve = new AnimationCurve();
        sizeCurve.AddKey(0f, 1f);
        sizeCurve.AddKey(1f, 0.5f);
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

        // Color over Lifetime
        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(Color.white, 0f),
                new GradientColorKey(Color.white, 1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(0f, 0f),
                new GradientAlphaKey(0.4f, 0.2f),
                new GradientAlphaKey(0.4f, 0.8f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);

        // Renderer
        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Stretch;
        renderer.lengthScale = 2.5f;
        renderer.velocityScale = 0.4f;
        renderer.sortingOrder = 5;

        // Assign material
        string materialPath = "Assets/Particle Systems/WindMaterial.mat";
        Material windMaterial = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        if (windMaterial != null)
        {
            renderer.material = windMaterial;
        }
        else
        {
            Debug.LogWarning("WindMaterial not found at: " + materialPath);
        }

        // Set reference in WindZone2D component
        WindZone2D windZoneScript = windZone.GetComponent<WindZone2D>();
        if (windZoneScript != null)
        {
            SerializedObject so = new SerializedObject(windZoneScript);
            SerializedProperty particlesProp = so.FindProperty("windParticles");
            particlesProp.objectReferenceValue = ps;
            so.ApplyModifiedProperties();
        }

        EditorUtility.SetDirty(windZone);
        Debug.Log("Wind Particle System configured successfully!");
    }
}
