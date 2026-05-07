using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class WindZone2D : MonoBehaviour
{
    [Header("Wind Settings")]
    [SerializeField] private Vector2 windDirection = Vector2.right;
    [SerializeField] private float windStrength = 5f;
    [SerializeField] private float windStrengthMult = 10f;
    [SerializeField] private bool useGustEffect = true;
    [SerializeField] private float gustFrequency = 2f;
    [SerializeField] private float gustStrength = 3f;

    [Header("Airborne Settings")]
    [SerializeField] private float airborneMultiplier = 2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Particle System")]
    [SerializeField] private ParticleSystem windParticles;

    private BoxCollider2D windZone;
    private AreaEffector2D windEffector;
    private float gustTimer;

    private void Awake()
    {
        windZone = GetComponent<BoxCollider2D>();
        windZone.isTrigger = true;
        windZone.usedByEffector = true;

        windEffector = GetComponent<AreaEffector2D>();
        if (windEffector == null)
        {
            windEffector = gameObject.AddComponent<AreaEffector2D>();
        }
        windEffector.forceMagnitude = windStrength * windStrengthMult;
        windEffector.forceAngle = Mathf.Atan2(windDirection.y, windDirection.x) * Mathf.Rad2Deg;

        if (windParticles == null)
        {
            windParticles = GetComponentInChildren<ParticleSystem>();
        }

        SetupParticleSystem();
    }

    private void Update()
    {
        if (useGustEffect)
        {
            gustTimer += Time.deltaTime;
            float gustMultiplier = 1f + Mathf.Sin(gustTimer * gustFrequency) * (gustStrength / windStrength);
            windEffector.forceMagnitude = windStrength * windStrengthMult * gustMultiplier;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                float currentStrength = windStrength;

                if (useGustEffect)
                {
                    float gustMultiplier = 1f + Mathf.Sin(gustTimer * gustFrequency) * (gustStrength / windStrength);
                    currentStrength *= gustMultiplier;
                }

                bool isGrounded = IsPlayerGrounded(other);
                if (!isGrounded)
                {
                    currentStrength *= airborneMultiplier;
                }

                windEffector.forceMagnitude = currentStrength * windStrengthMult;
                rb.AddForce(windDirection.normalized * currentStrength * Time.fixedDeltaTime * 50f, ForceMode2D.Force);
            }
        }
    }

    private bool IsPlayerGrounded(Collider2D playerCollider)
    {
        PlayerMovement player = playerCollider.GetComponent<PlayerMovement>();
        if (player != null)
        {
            return player.IsGrounded();
        }

        BoxCollider2D box = playerCollider as BoxCollider2D;
        if (box != null)
        {
            Vector2 checkPosition = (Vector2)playerCollider.transform.position + box.offset + Vector2.down * (box.size.y * 0.5f + 0.1f);
            return Physics2D.OverlapCircle(checkPosition, 0.1f, groundLayer);
        }

        return false;
    }

    private void SetupParticleSystem()
    {
        if (windParticles == null) return;

        var main = windParticles.main;
        main.startSpeed = windStrength * 2f;
        main.startLifetime = 2f;
        main.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.1f);
        main.maxParticles = 100;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = windParticles.emission;
        emission.rateOverTime = 50;

        var shape = windParticles.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(windZone.size.x, windZone.size.y, 0.1f);

        var velocityOverLifetime = windParticles.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.space = ParticleSystemSimulationSpace.World;
        velocityOverLifetime.x = windDirection.normalized.x * windStrength;
        velocityOverLifetime.y = windDirection.normalized.y * windStrength;

        var renderer = windParticles.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Stretch;
        renderer.lengthScale = 2f;
        renderer.velocityScale = 0.5f;
    }

    private void OnDrawGizmos()
    {
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col != null)
        {
            Gizmos.color = new Color(0.5f, 0.8f, 1f, 0.3f);
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(col.offset, col.size);

            Gizmos.color = Color.cyan;
            Vector3 center = transform.TransformPoint(col.offset);
            Vector3 arrowEnd = center + (Vector3)(windDirection.normalized * 2f);
            Gizmos.DrawLine(center, arrowEnd);

            Vector3 arrowHead1 = arrowEnd - (Vector3)(windDirection.normalized * 0.5f) + new Vector3(-windDirection.normalized.y, windDirection.normalized.x, 0) * 0.3f;
            Vector3 arrowHead2 = arrowEnd - (Vector3)(windDirection.normalized * 0.5f) - new Vector3(-windDirection.normalized.y, windDirection.normalized.x, 0) * 0.3f;
            Gizmos.DrawLine(arrowEnd, arrowHead1);
            Gizmos.DrawLine(arrowEnd, arrowHead2);
        }
    }

    public void SetWindDirection(Vector2 direction)
    {
        windDirection = direction;
        SetupParticleSystem();
    }

    public void SetWindStrength(float strength)
    {
        windStrength = strength;
        SetupParticleSystem();
    }
}
