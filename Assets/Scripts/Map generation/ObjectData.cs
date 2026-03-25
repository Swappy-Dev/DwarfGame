using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ObjectData
{
    public GameObject prefab;
    [Range(0, 1)]
    public float placementProbability = 0.1f; // Tikimybė atsirasti ant plytelės
    public bool placeInRoomsOnly = true;     // Ar generuoti tik kambariuose?
}