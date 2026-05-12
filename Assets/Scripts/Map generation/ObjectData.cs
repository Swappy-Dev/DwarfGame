using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ObjectData
{
    public GameObject prefab;
    [Range(0, 1)]
    public float placementProbability = 0.1f;
    public bool placeInRoomsOnly = true;
    public bool isEnemy = false; // NAUJA: Ar tai priešas?
}