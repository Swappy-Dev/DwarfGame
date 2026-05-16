using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
    [SerializeField] private List<ObjectData> objectsToPlace;
    private List<GameObject> spawnedObjects = new List<GameObject>();

    [SerializeField] private ObjectData bossData;

    // --- NAUJA: Pridedame nuorodą į pergalės panelę generatoriuje ---
    [Header("UI Kontrolė")]
    [SerializeField] private GameObject victoryPanel;

    public void PlaceBoss(Vector2Int position)
    {
        if (bossData != null && bossData.prefab != null)
        {
            Vector3 worldPos = new Vector3(position.x + 0.5f, position.y + 0.5f, 0);
            GameObject boss = Instantiate(bossData.prefab, worldPos, Quaternion.identity, transform);
            spawnedObjects.Add(boss);

            // --- NAUJA LOGIKA: Perduodame VictoryPanel tiesiai sukurtam bosui ---
            BOSS_AI bossAI = boss.GetComponent<BOSS_AI>();
            if (bossAI != null)
            {
                bossAI.victoryPanel = victoryPanel;
            }
            else
            {
                Debug.LogWarning("Bosas buvo sukurtas, bet ant jo nerastas BOSS_AI skriptas!");
            }
        }
    }

    public void PlaceObjects(HashSet<Vector2Int> roomPositions, HashSet<Vector2Int> corridorPositions, HashSet<Vector2Int> spawnRoomPositions)
    {
        ClearObjects();

        // 1. Generuojame objektus kambariuose
        foreach (var pos in roomPositions)
        {
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