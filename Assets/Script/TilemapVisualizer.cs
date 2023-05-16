using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField]
    private Tilemap floorTM;
    [SerializeField]
    private TileBase floorTile;

    public void paintFloorTiles(IEnumerable<Vector2> floorPos)
    {
        paintTiles(floorPos, floorTM, floorTile);
    }

    private void paintTiles(IEnumerable<Vector2> position, Tilemap tilemap, TileBase tile)
    {
        foreach(Vector2 pos in position)
        {
            paintOneTile(tilemap, tile, pos);
        }
    }

    private void paintOneTile(Tilemap tilemap, TileBase tile, Vector2 pos)
    {
        var tilePos = tilemap.WorldToCell((Vector3)pos);
        tilemap.SetTile(tilePos, tile);
    }
}