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
    [Range(0.1f, 1)]
    private float roomPercent;
    [SerializeField]
    private ObjectGenerator objectGenerator;
    [SerializeField]
    private GameObject player;

    [SerializeField]
    private int bossCorridorLength = 8;

    


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

        // 1. Sugeneruojame pagrindinius koridorius
        List<List<Vector2Int>> corridors = CreateCorridors(floorpositions, potentialRoomPositions);

        // Išsaugome tik koridorių plyteles (prieš platinimą)
        HashSet<Vector2Int> corridorPositionsOnly = new HashSet<Vector2Int>(floorpositions);

        // 2. Sugeneruojame Spawn kambarį
        var spawnRoomFloor = RunRandomWalk(randomWalkParameters, startPosition);

        // 3. Sukuriame Boso koridorių ir Boso kambarį (naudojami bossRoomParameters)
        HashSet<Vector2Int> bossRoomFloor = CreateBossRoomWithCorridor(potentialRoomPositions, corridorPositionsOnly, floorpositions);

        // 4. Sugeneruojame likusius paprastus kambarius iš turimų taškų
        HashSet<Vector2Int> roomPositions = CreateRooms(potentialRoomPositions);

        // Sukuriame sąrašą jau turimų kambarių patikrai
        HashSet<Vector2Int> allCreatedRooms = new HashSet<Vector2Int>(roomPositions);
        allCreatedRooms.UnionWith(spawnRoomFloor);
        allCreatedRooms.UnionWith(bossRoomFloor);

        // 5. Surandame aklavietes koridoriuose
        List<Vector2Int> deadEnds = FindAllDeadEnds(floorpositions);

        // Sugeneruojame kambarius aklavietėse, kuriose dar nėra jokio kambario
        foreach (var deadEnd in deadEnds)
        {
            if (!allCreatedRooms.Contains(deadEnd))
            {
                var room = RunRandomWalk(randomWalkParameters, deadEnd);
                roomPositions.UnionWith(room);
                allCreatedRooms.UnionWith(room);
            }
        }

        // Sukuriame kopiją objektų generatoriui (TIK paprasti kambariai, be boso kambario)
        HashSet<Vector2Int> roomsForObjects = new HashSet<Vector2Int>(roomPositions);

        // Sujungiame viską į bendrą grindų sąrašą vizualizacijai
        roomPositions.UnionWith(spawnRoomFloor);
        roomPositions.UnionWith(bossRoomFloor);
        floorpositions.UnionWith(roomPositions);

        // 6. Sutvarkome koridorių platinimą (vizualiai)
        for (int i = 0; i < corridors.Count; i++)
        {
            corridors[i] = IncreaseCorridorSizeByOne(corridors[i]);
            floorpositions.UnionWith(corridors[i]);
        }

        // Nupiešiame viską
        tileMapVisualization.Clear();
        tileMapVisualization.PaintFloorTiles(floorpositions);
        WallGenerator.CreateWalls(floorpositions, tileMapVisualization);

        // 7. Perduodame duomenis objektų generatoriui
        if (objectGenerator != null)
        {
            // Perduodami roomsForObjects, kuriuose nėra boso kambario plytelių
            objectGenerator.PlaceObjects(roomsForObjects, corridorPositionsOnly, spawnRoomFloor);
        }

        // Perkeliame žaidėją
        if (player != null)
        {
            player.transform.position = new Vector3(startPosition.x + 0.5f, startPosition.y + 0.5f, 0);
        }
    }

    private HashSet<Vector2Int> CreateBossRoomWithCorridor(
        HashSet<Vector2Int> potentialRoomPositions,
        HashSet<Vector2Int> corridorPositionsOnly,
        HashSet<Vector2Int> floorpositions)
    {
        HashSet<Vector2Int> bossRoomFloor = new HashSet<Vector2Int>();

        if (potentialRoomPositions.Count == 0)
            return bossRoomFloor;

        var sortedPotentialPositions = potentialRoomPositions
            .OrderByDescending(pos => Vector2Int.Distance(pos, startPosition))
            .ToList();

        Vector2Int bestBossRoomCenter = Vector2Int.zero;
        List<Vector2Int> bestBossCorridor = new List<Vector2Int>();
        bool validSpotFound = false;

        // Spindulys, kuriame neturi būti kitų kambarių/koridorių plytelių
        int clearRadius = 12;

        foreach (Vector2Int candidateStart in sortedPotentialPositions)
        {
            Vector2Int[] testDirections = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
            Vector2 directionToCandidate = ((Vector2)(candidateStart - startPosition)).normalized;
            testDirections = testDirections
                .OrderByDescending(dir => Vector2.Dot(dir, directionToCandidate))
                .ToArray();

            foreach (Vector2Int dir in testDirections)
            {
                List<Vector2Int> testCorridor = new List<Vector2Int>();
                Vector2Int currentPos = candidateStart;
                testCorridor.Add(currentPos);

                for (int i = 0; i < bossCorridorLength; i++)
                {
                    currentPos += dir;
                    testCorridor.Add(currentPos);
                }

                Vector2Int candidateRoomCenter = testCorridor[testCorridor.Count - 1];

                bool isOverlapping = false;
                foreach (var floorTile in corridorPositionsOnly)
                {
                    if (floorTile == candidateStart) continue;

                    if (Vector2Int.Distance(candidateRoomCenter, floorTile) < clearRadius)
                    {
                        isOverlapping = true;
                        break;
                    }
                }

                if (!isOverlapping)
                {
                    bestBossRoomCenter = candidateRoomCenter;
                    bestBossCorridor = testCorridor;
                    validSpotFound = true;
                    potentialRoomPositions.Remove(candidateStart);
                    break;
                }
            }

            if (validSpotFound) break;
        }

        if (!validSpotFound)
        {
            Vector2Int fallbackStart = sortedPotentialPositions.First();
            potentialRoomPositions.Remove(fallbackStart);

            Vector2 directionToBoss = ((Vector2)(fallbackStart - startPosition)).normalized;
            Vector2Int fallbackDir = Mathf.Abs(directionToBoss.x) > Mathf.Abs(directionToBoss.y)
                ? (directionToBoss.x > 0 ? Vector2Int.right : Vector2Int.left)
                : (directionToBoss.y > 0 ? Vector2Int.up : Vector2Int.down);

            Vector2Int currentPos = fallbackStart;
            bestBossCorridor.Add(currentPos);
            for (int i = 0; i < bossCorridorLength; i++)
            {
                currentPos += fallbackDir;
                bestBossCorridor.Add(currentPos);
            }
            bestBossRoomCenter = bestBossCorridor[bestBossCorridor.Count - 1];
        }

        corridorPositionsOnly.UnionWith(bestBossCorridor);
        floorpositions.UnionWith(bestBossCorridor);

        // Naudojame boso parametrus, o jei jie neįvesti - paprastus parametrus
        SimpleRandomWalkSO parametersToUse = bossRoomParameters;
        bossRoomFloor = RunRandomWalk(parametersToUse, bestBossRoomCenter);

        potentialRoomPositions.RemoveWhere(pos => Vector2Int.Distance(pos, bestBossRoomCenter) < clearRadius);

        return bossRoomFloor;
    }

    private HashSet<Vector2Int> CreateRooms(HashSet<Vector2Int> potentialRoomPositions)
    {
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();

        var filteredPositions = potentialRoomPositions
            .Where(pos => Vector2Int.Distance(pos, startPosition) > 10)
            .ToList();

        int roomToCreateCount = Mathf.RoundToInt(filteredPositions.Count * roomPercent);
        List<Vector2Int> roomsToCreate = filteredPositions.OrderBy(x => Guid.NewGuid()).Take(roomToCreateCount).ToList();

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

    private List<Vector2Int> IncreaseCorridorSizeByOne(List<Vector2Int> corridor)
    {
        List<Vector2Int> newCorridor = new List<Vector2Int>();
        Vector2Int prewievDirection = Vector2Int.zero;
        for (int i = 1; i < corridor.Count; i++)
        {
            Vector2Int directionFromCell = corridor[i] - corridor[i - 1];
            if (prewievDirection != Vector2Int.zero &&
                directionFromCell != prewievDirection)
            {
                for (int x = -1; x < 2; x++)
                {
                    for (int y = -1; y < 2; y++)
                    {
                        newCorridor.Add(corridor[i - 1] + new Vector2Int(x, y));
                    }
                }
                prewievDirection = directionFromCell;
            }
            else
            {
                Vector2Int newCorridorTileOffset = GetDirection90From(directionFromCell);
                newCorridor.Add(corridor[i - 1]);
                newCorridor.Add(corridor[i - 1] + newCorridorTileOffset);
            }
        }
        return newCorridor;
    }

    private Vector2Int GetDirection90From(Vector2Int direction)
    {
        if (direction == Vector2Int.up) return Vector2Int.right;
        if (direction == Vector2Int.right) return Vector2Int.down;
        if (direction == Vector2Int.down) return Vector2Int.left;
        if (direction == Vector2Int.left) return Vector2Int.up;
        return Vector2Int.zero;
    }

    private void CreateRoomsAtDeadEnd(List<Vector2Int> deadEnds, HashSet<Vector2Int> roomFloors)
    {
        foreach (var position in deadEnds)
        {
            if (roomFloors.Contains(position) == false)
            {
                var room = RunRandomWalk(randomWalkParameters, position);
                roomFloors.UnionWith(room);
            }
        }
    }

    private List<Vector2Int> FindAllDeadEnds(HashSet<Vector2Int> floorpositions)
    {
        List<Vector2Int> deadEnds = new List<Vector2Int>();
        foreach (var position in floorpositions)
        {
            int neighboursCount = 0;
            foreach (var direction in Direction2D.cardinalDirectionsList)
            {
                if (floorpositions.Contains(position + direction))
                    neighboursCount++;
            }
            if (neighboursCount == 1)
                deadEnds.Add(position);
        }
        return deadEnds;
    }

    public List<Vector2Int> IncreaseCorridorBrush3by3(List<Vector2Int> corridor)
    {
        List<Vector2Int> newCorridor = new List<Vector2Int>();
        for (int i = 1; i < corridor.Count; i++)
        {
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    newCorridor.Add(corridor[i - 1] + new Vector2Int(x, y));
                }
            }
        }
        return newCorridor;
    }
}