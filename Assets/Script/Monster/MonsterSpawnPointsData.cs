using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Monster Spawn Points Data", menuName = "Monster Spawn Points Data", order = 4)]
public class MonsterSpawnPointsData : ScriptableObject
{
    [SerializeField]
    private Vector3[] spawnPoints;
}
