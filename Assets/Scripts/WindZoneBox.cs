using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class WindZoneBox : MonoBehaviour
{
    public Vector3 boxSize = new Vector3(10f, 5f, 10f);
    private ParticleSystem ps;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();

        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = boxSize;
    }

    // Визуализация зоны в редакторе
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.3f, 0.8f, 1f, 0.3f);
        Gizmos.DrawCube(transform.position, boxSize);
        Gizmos.color = new Color(0.3f, 0.8f, 1f, 0.8f);
        Gizmos.DrawWireCube(transform.position, boxSize);
    }
}