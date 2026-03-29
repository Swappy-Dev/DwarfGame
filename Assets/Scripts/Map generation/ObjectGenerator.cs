using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
    [SerializeField] private List<ObjectData> objectsToPlace;
    private List<GameObject> spawnedObjects = new List<GameObject>();

    public void PlaceObjects(HashSet<Vector2Int> roomPositions, HashSet<Vector2Int> corridorPositions, HashSet<Vector2Int> spawnRoomPositions)
    {
        ClearObjects();

        // 1. Generuojame objektus kambariuose
        foreach (var pos in roomPositions)
        {
            // Tikriname, ar ši plytelė priklauso Spawn kambariui
            bool isInsideSpawn = spawnRoomPositions.Contains(pos);
            TryPlaceObject(pos, true, isInsideSpawn);
        }

        // 2. Generuojame objektus koridoriuose
        foreach (var pos in corridorPositions)
        {
            TryPlaceObject(pos, false, false);
        }
    }

    private void TryPlaceObject(Vector2Int position, bool isRoom, bool isInsideSpawn)
    {
        foreach (var objData in objectsToPlace)
        {
            if (objData.placeInRoomsOnly && !isRoom) continue;

            // NAUJA LOGIKA: Jei tai priešas ir mes esame Spawn kambaryje - praleidžiame
            if (objData.isEnemy && isInsideSpawn) continue;

            if (UnityEngine.Random.value < objData.placementProbability)
            {
                Vector3 worldPos = new Vector3(position.x + 0.5f, position.y + 0.5f, 0);
                GameObject spawned = Instantiate(objData.prefab, worldPos, Quaternion.identity, transform);
                spawnedObjects.Add(spawned);
                break;
            }
        }
    }

    public void ClearObjects()
    {
        foreach (var obj in spawnedObjects)
        {
            if (Application.isPlaying) Destroy(obj);
            else DestroyImmediate(obj);
        }
        spawnedObjects.Clear();
    }
}