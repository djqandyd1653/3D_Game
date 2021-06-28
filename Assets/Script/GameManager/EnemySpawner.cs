using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class EnemySpawner : MonoBehaviour
{
    public GameObject SkeletonPool;
    private Vector3[] spawnPositions;
    private Dictionary<Vector3, GameObject> spawnData;

    // Test var
    int i = 0;

    private void Start()
    {
        spawnPositions = new Vector3[4];

        for(int i = 0; i < 4; i++)
        {
            spawnPositions[i] = new Vector3(-10 + 10 * i, 0, 25);
        }
        
        spawnData = new Dictionary<Vector3, GameObject>();
    }

    private void Update()
    {
        
        if(Input.GetKeyDown(KeyCode.O))
        {
            SkeletonPool.GetComponent<SkeletonPool>().GetSkeleton(spawnPositions[i++]);

            if (i >= 4)
                i = 0;
        }
    }


    //Dictionary<Vector3, GameObject> monsterPool;
    //public SpawnPosData[] spawnPosData;

    //[ContextMenu("To Json")]
    //void SaveToJson()
    //{
    //    string path = Path.Combine(Application.dataPath, "SpawnPosData.json");
    //    string data = JsonUtility.ToJson(spawnPosData, true);
    //    File.WriteAllText(path, data);
    //}

    //[ContextMenu("From Json")]
    //void LoadFromJson()
    //{
    //    string path = Path.Combine(Application.dataPath, "SpawnPosData.json");
    //    string data = File.ReadAllText(path);
    //    spawnPosData = JsonUtility.FromJson<SpawnPosData[]>(data);
    //}
}

//[System.Serializable]
//public class SpawnPosData
//{
//    public Vector3[] spawnPos;
//    public string monsterName;
//}