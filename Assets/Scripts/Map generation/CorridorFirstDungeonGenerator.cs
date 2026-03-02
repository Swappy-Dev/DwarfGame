using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CorridorFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    [SerializeField]
    private int corridorLength = 14, corridorCount = 5;
    [SerializeField]
    [Range(0.1f,1)]
    private float roomPercent;


    protected override void RunProceduralGeneration()
    {
        CorridorFirstGeneration();
    }

    private void CorridorFirstGeneration()
    {
        HashSet<Vector2Int> floorpositions = new HashSet<Vector2Int>();
        HashSet<Vector2Int> potentialRoomPositions = new HashSet<Vector2Int>();
        
        CreateCorridors(floorpositions, potentialRoomPositions);
        
        HashSet<Vector2Int> roomPositions = CreateRooms(potentialRoomPositions);
        
        List<Vector2Int> deadEnds = FindAllDeadEnds(floorpositions);

        CreateRoomsAtDeadEnd(deadEnds, roomPositions);
        
        floorpositions.UnionWith(roomPositions);
        
        tileMapVisualization.PaintFloorTiles(floorpositions);
        WallGenerator.CreateWalls(floorpositions, tileMapVisualization);
    }

    private void CreateRoomsAtDeadEnd(List<Vector2Int> deadEnds, HashSet<Vector2Int> roomFloors)
    {
        foreach (var position in deadEnds)
        {
            if(roomFloors.Contains(position) == false)
            {
                var room = RunRandomWalk(randomWalkParameters, position);
                roomFloors.UnionWith(room);
            }
        }
    }

    private List<Vector2Int> FindAllDeadEnds(HashSet<Vector2Int> floorpositions)
    {
        List<Vector2Int> deadEnds = new List<Vector2Int>();
        foreach (var positions in floorpositions)
        {
            int neighboursCount = 0;
            foreach (var direction in Direction2D.cardinalDirectionsList)
            {
                if (floorpositions.Contains(positions + direction))
                    neighboursCount++;
            }
            if (neighboursCount == 1)
                deadEnds.Add(positions);
        }
        return deadEnds;
    }

    private HashSet<Vector2Int> CreateRooms(HashSet<Vector2Int> potentialRoomPositions)
    {
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
        int roomToCreateCount = Mathf.RoundToInt(potentialRoomPositions.Count * roomPercent);
    
        List<Vector2Int> roomsToCreate = potentialRoomPositions.OrderBy(x => Guid.NewGuid()).Take(roomToCreateCount).ToList();

        foreach (var roomPosition in roomsToCreate)
        {
            var roomFloor = RunRandomWalk(randomWalkParameters, roomPosition);
            roomPositions.UnionWith(roomFloor);
        }
        return roomPositions;
    }

    private void CreateCorridors(HashSet<Vector2Int> floorpositions, HashSet<Vector2Int> potentialRoomPositions)
    {
        var currentPositions = startPosition;
        potentialRoomPositions.Add(currentPositions);


        for (int i = 0; i < corridorCount; i++)
        {
            var corridor = ProceduralGenerationAlgorithm.RandomWalkCorridor(currentPositions, corridorLength);
            currentPositions = corridor[corridor.Count - 1];
            potentialRoomPositions.Add(currentPositions);
            floorpositions.UnionWith(corridor);
        }
    }
}
