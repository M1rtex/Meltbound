using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using UnityEditor.Tilemaps;

[CustomGridBrush(false, false, false, "Transform Brush")]
public class TransformBrush : GridBrush
{
    public override void MoveStart(GridLayout grid, GameObject layer, BoundsInt position)
    {
        var tilemap = layer.GetComponent<Tilemap>();
        if (tilemap == null) return;
        base.MoveStart(grid, layer, position);
    }

    public override void Pick(GridLayout grid, GameObject layer, BoundsInt position, Vector3Int pivot)
    {
        base.Pick(grid, layer, position, pivot);
        var tilemap = layer.GetComponent<Tilemap>();
        if (tilemap == null) return;

        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                var localPos = new Vector3Int(x, y, 0);
                var tilePos = position.min + localPos;
                int index = x + y * size.x;
                cells[index].matrix = tilemap.GetTransformMatrix(tilePos);
            }
        }
    }
}