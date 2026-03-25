using System;
using System.Collections.Generic;
using UnityEngine;

public static class WallGenerator
{
    public static void CreateWalls(HashSet<Vector2Int> floorPosition, TileMapVisualization tilemapVizualization)
    {
        var cornerWallPositions = FindWallsInDirections(floorPosition, Direction2D.eightDirectionsList);
        CreateCornerWall(tilemapVizualization, cornerWallPositions, floorPosition);
    }

    private static void CreateCornerWall(TileMapVisualization tilemapVizualization, HashSet<Vector2Int> cornerWallPositions, HashSet<Vector2Int> floorPosition)
    {
        foreach(var position in cornerWallPositions)
        {
            string neighboursBinaryType = "";
            foreach ( var direction in Direction2D.eightDirectionsList)
            {
                var neighbourPosition = position + direction;
                if (floorPosition.Contains(neighbourPosition))
                {
                    neighboursBinaryType += "1";
                }
                else
                {
                    neighboursBinaryType += "0";
                }
            }
            tilemapVizualization.PaintHoles(position, neighboursBinaryType);
        }
        foreach (var position in cornerWallPositions)
        {
            string neighboursBinaryType = "";
            foreach (var direction in Direction2D.eightDirectionsList)
            {
                var neighbourPosition = position + direction;
                if (floorPosition.Contains(neighbourPosition))
                {
                    neighboursBinaryType += "1";
                }
                else
                {
                    neighboursBinaryType += "0";
                }
            }
            tilemapVizualization.PaintSingleCornerWall(position, neighboursBinaryType);
        }
    }

    private static HashSet<Vector2Int> FindWallsInDirections(HashSet<Vector2Int> floorPosition, List<Vector2Int> directionsList)
    {
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
        foreach (var positions in floorPosition)
        {
            foreach (var direction in directionsList)
            {
                var neighbourPosition = positions + direction;
                if(floorPosition.Contains(neighbourPosition) == false)
                    wallPositions.Add(neighbourPosition);
            }
        }
        return wallPositions;
    }
}
