using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
    [SerializeField] private List<ObjectData> objectsToPlace;
    private List<GameObject> spawnedObjects = new List<GameObject>();

    public void PlaceObjects(HashSet<Vector2Int> roomPositions, HashSet<Vector2Int> corridorPositions)
    {
        ClearObjects();

        // Generuojame objektus kambariuose
        foreach (var pos in roomPositions)
        {
            TryPlaceObject(pos, true);
        }

        // Generuojame objektus koridoriuose (jei reikia)
        foreach (var pos in corridorPositions)
        {
            TryPlaceObject(pos, false);
        }
    }

    private void TryPlaceObject(Vector2Int position, bool isRoom)
    {
        foreach (var objData in objectsToPlace)
        {
            // Tikriname sąlygas: ar tinka vieta ir ar pasisekė "ridenti kauliuką"
            if (objData.placeInRoomsOnly && !isRoom) continue;

            if (UnityEngine.Random.value < objData.placementProbability)
            {
                Vector3 worldPos = new Vector3(position.x + 0.5f, position.y + 0.5f, 0);
                GameObject spawned = Instantiate(objData.prefab, worldPos, Quaternion.identity, transform);
                spawnedObjects.Add(spawned);
                break; // Vienoje plytelėje tik vienas objektas
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