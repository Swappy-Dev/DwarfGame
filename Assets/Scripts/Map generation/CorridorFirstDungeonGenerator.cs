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
    [SerializeField] 
    private ObjectGenerator objectGenerator;


    private void Start()
    {
        tileMapVisualization.Clear();
        GenerateDungeon();
    }

    public new void GenerateDungeon()
    {
        RunProceduralGeneration();
    }

    protected override void RunProceduralGeneration()
    {
        CorridorFirstGeneration();
    }

    private void CorridorFirstGeneration()
    {
        HashSet<Vector2Int> floorpositions = new HashSet<Vector2Int>();
        HashSet<Vector2Int> potentialRoomPositions = new HashSet<Vector2Int>();

        // 1. Sukuriami koridoriai
        List<List<Vector2Int>> corridors = CreateCorridors(floorpositions, potentialRoomPositions);
        HashSet<Vector2Int> corridorPositionsOnly = new HashSet<Vector2Int>(floorpositions);

        // 2. Sukuriami kambariai
        HashSet<Vector2Int> roomPositions = CreateRooms(potentialRoomPositions);
        List<Vector2Int> deadEnds = FindAllDeadEnds(floorpositions);
        CreateRoomsAtDeadEnd(deadEnds, roomPositions);

        // 3. Sujungiami visi grindų taškai vizualizacijai
        floorpositions.UnionWith(roomPositions);

        // Koridorių platinimas...
        for (int i = 0; i < corridors.Count; i++)
        {
            corridors[i] = IncreaseCorridorSizeByOne(corridors[i]);
            floorpositions.UnionWith(corridors[i]);
        }

        tileMapVisualization.PaintFloorTiles(floorpositions);
        WallGenerator.CreateWalls(floorpositions, tileMapVisualization);

        // --- NAUJA DALIS: Objektų generavimas ---
        if (objectGenerator != null)
        {
            objectGenerator.PlaceObjects(roomPositions, corridorPositionsOnly);
        }
    }

    private List<Vector2Int> IncreaseCorridorSizeByOne(List<Vector2Int> corridor)
    {
        List<Vector2Int> newCorridor = new List<Vector2Int>();
        Vector2Int prewievDirection = Vector2Int.zero;
        for(int i = 1; i < corridor.Count; i++)
        {
            Vector2Int directionFromCell = corridor[i] - corridor[i - 1];
            if(prewievDirection != Vector2Int.zero &&
                directionFromCell != prewievDirection)
            {
                for(int x = -1; x < 2; x++)
                {
                    for(int y = -1; y < 2; y++)
                    {
                        newCorridor.Add(corridor[i-1] + new Vector2Int(x, y));
                    }
                }
                prewievDirection = directionFromCell;
            }
            else
            {
                Vector2Int newCorridorTileOffset
                    = GetDirection90From(directionFromCell);
                newCorridor.Add(corridor[i - 1]);
                newCorridor.Add(corridor[i - 1] + newCorridorTileOffset);
            }
        }
        return newCorridor;
    }

    private Vector2Int GetDirection90From(Vector2Int direction)
    {
        if (direction == Vector2Int.up)
            return Vector2Int.right;
        if (direction == Vector2Int.right)
            return Vector2Int.down;
        if (direction == Vector2Int.down)
            return Vector2Int.left;
        if(direction == Vector2Int.left)
            return Vector2Int.up;
        return Vector2Int.zero;
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

    private List<List<Vector2Int>> CreateCorridors(HashSet<Vector2Int> floorpositions, HashSet<Vector2Int> potentialRoomPositions)
    {
        var currentPositions = startPosition;
        potentialRoomPositions.Add(currentPositions);
        List<List<Vector2Int>> corridors = new List<List<Vector2Int>>();

        for (int i = 0; i < corridorCount; i++)
        {
            var corridor = ProceduralGenerationAlgorithm.RandomWalkCorridor(currentPositions, corridorLength);
            corridors.Add(corridor);
            currentPositions = corridor[corridor.Count - 1];
            potentialRoomPositions.Add(currentPositions);
            floorpositions.UnionWith(corridor);
        }
        return corridors;
    }
    public List<Vector2Int> IncreaseCorridorBrush3by3(List<Vector2Int> corridor)
    {
        List<Vector2Int> newCorridor = new List<Vector2Int>();
        for(int i = 1; i < corridor.Count; i++)
        {
            for(int x = -1; x < 2; x++)
            {
                for(int y = -1; y < 2; y++)
                {
                    newCorridor.Add(corridor[i - 1] + new Vector2Int(x, y));
                }
            }
        }
        return newCorridor;
    }
}


