using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Monster Spawn Points Data", menuName = "Monster Spawn Points Data", order = 4)]
public class MonsterSpawnPointsData : ScriptableObject
{
    [Serializable]
    public struct MonsterSpawnData
    {
        public Vector3 originPosition;
        public string monsterName;
        public float respawnTime;

        public MonsterSpawnData(Vector3 point, string name, float time)
        {
            originPosition = point;
            monsterName = name;
            respawnTime = time;
        }
    }

    // Unity Editor 공부하고 변경하자
    //[SerializeField]
    //private List<MonsterSpawnData> dataList;

    [SerializeField]
    private MonsterSpawnData[] dataList;
    public MonsterSpawnData[] DataList { get { return dataList; } }
}
