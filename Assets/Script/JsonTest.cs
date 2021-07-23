using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class JsonTest : MonoBehaviour
{
    public MonsterSpawner spawner;

    [Serializable]
    public class PlayerJsonData
    {
        [SerializeField]
        public float hp;

        [SerializeField]
        public float attackPower;

        [SerializeField]
        public CustomVector3 position;

        [SerializeField]
        public CustomVector3 rotation;
    }

    [Serializable]
    public class MonsterJsonData
    {
        [SerializeField]
        public string name;

        [SerializeField]
        public float hp;

        [SerializeField]
        public CustomVector3 position;

        [SerializeField]
        public CustomVector3 rotation;

        [SerializeField]
        public bool isActive;

        [SerializeField]
        public Transform parent;
    }

    // vector3 커스텀
    [Serializable]
    public struct CustomVector3
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }

        public Quaternion ToQuaternion()
        {
            return new Quaternion(x, y, z, w);
        }

        public CustomVector3(Vector3 vector3)
        {
            x = vector3.x;
            y = vector3.y;
            z = vector3.z;
            w = 0;
        }

        public CustomVector3(Quaternion quaternion)
        {
            x = quaternion.x;
            y = quaternion.y;
            z = quaternion.z;
            w = quaternion.w;
        }
    }

    // 리스트는 따로 빼서 직렬화 해줘야한다.
    // List 커스텀
    [Serializable]
    public class CustomList<T>
    {
        [SerializeField]
        List<T> list;

        public List<T> ToList()
        {
            return list;
        }

        public CustomList(List<T> _list)
        {
            list = _list;
        }
    }


    // dictionary 커스텀
    [Serializable]
    public class CustomDictionary<TKey, TValue>
    {
        [SerializeField]
        public List<TKey> keys = new List<TKey>();
        [SerializeField]
        public List<TValue> values = new List<TValue>();

        Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

        public Dictionary<TKey, TValue> ToDictoinary()
        {
            return dictionary;
        }

        public CustomDictionary(Dictionary<TKey, TValue> dic)
        {
            keys = new List<TKey>(dic.Keys);
            values = new List<TValue>(dic.Values);

            for (int i = 0; i < dic.Count; i++)
            {
                dictionary.Add(keys[i], values[i]);
            }
        }
    }

    // 플레이어 데이터 저장
    public void SavePlayerData()
    {
        var playerObject = GameObject.FindGameObjectWithTag("Player");
        var player = playerObject.GetComponent<Player>();
        PlayerJsonData playerData = new PlayerJsonData();

        playerData.hp = player.Hp;
        playerData.attackPower = player.AttackPower;

        playerData.position = new CustomVector3(playerObject.transform.position);

        playerData.rotation = new CustomVector3(playerObject.transform.rotation);

        string jsonData = JsonUtility.ToJson(playerData, true);
        string path = Path.Combine(Application.dataPath, "SavePlayerData.json");
        File.WriteAllText(path, jsonData);
    }

    // 플레이어 데이터 불러오기
    public void LoadPlayerData()
    {
        string path = Path.Combine(Application.dataPath, "SavePlayerData.json");
        string jsonData = File.ReadAllText(path);
        PlayerJsonData playerData = JsonUtility.FromJson<PlayerJsonData>(jsonData);

        var playerObject = GameObject.FindGameObjectWithTag("Player");
        var player = playerObject.GetComponent<Player>();

        playerObject.transform.position = playerData.position.ToVector3();

        playerObject.transform.rotation = playerData.rotation.ToQuaternion();
    }

    // 몬스터 데이터 저장하기
    public void SaveMonsterData()
    {
        GameObject[] monsterObjects = GameObject.FindGameObjectsWithTag("Monster");

        List<MonsterJsonData> monsters = new List<MonsterJsonData>();

        for (int i = 0; i < monsterObjects.Length; i++)
        {
            MonsterJsonData monster = new MonsterJsonData();

            MonsterData monsterData = monsterObjects[i].GetComponent<Monster>().monsterData;

            monster.name = monsterData.MonsterName;
            monster.hp = monsterData.Hp;
            monster.isActive = monsterObjects[i].activeSelf;
            monster.parent = monsterObjects[i].transform.parent;
            monster.position = new CustomVector3(monsterObjects[i].transform.position);

            monster.rotation = new CustomVector3(monsterObjects[i].transform.rotation);

            monsters.Add(monster);
        }

        CustomList<MonsterJsonData> customList = new CustomList<MonsterJsonData>(monsters);
        string monsterJson = JsonUtility.ToJson(customList, true);
        string path = Path.Combine(Application.dataPath, "MonsterData.json");
        File.WriteAllText(path, monsterJson);
    }

    // 몬스터 데이터 불러오기
    public void LoadMonsterData()
    {
        GameObject[] monsterObjects = GameObject.FindGameObjectsWithTag("Monster");

        for(int i = 0; i < monsterObjects.Length; i++)
        {
            var monster = monsterObjects[i].GetComponent<Monster>();
            GameEvent.Instance.OnEventMonsterDead(monsterObjects[i], monster.OriginPos, monster.monsterData.MonsterName, monster.monsterData.RespawnTime);
        }

        string path = Path.Combine(Application.dataPath, "MonsterData.json");
        string jsonData = File.ReadAllText(path);
        CustomList<MonsterJsonData> monsterJsonDataList = JsonUtility.FromJson<CustomList<MonsterJsonData>>(jsonData);

        List<MonsterJsonData> monsters = monsterJsonDataList.ToList();
        int count = monsters.Count;

        for(int i = 0; i < count; i++)
        {
            var monster = spawner.MonsterPoolManager[monsters[i].name].Dequeue();
            monster.transform.position = monsters[i].position.ToVector3();
            monster.transform.rotation = monsters[i].rotation.ToQuaternion();
            monster.transform.SetParent(monsters[i].parent);
            monster.gameObject.SetActive(monsters[i].isActive);
        }

        //GameObject[] monsterObjects = GameObject.FindGameObjectsWithTag("Monster");

        //List<MonsterJsonData> monsters = monsterJsonDataList.ToList();

        //for (int i = 0; i < monsterObjects.Length; i++)
        //{
        //    MonsterJsonData monster = new MonsterJsonData();

        //    MonsterData monsterData = monsterObjects[i].GetComponent<Monster>().monsterData;

        //    monster.hp = monsterData.Hp;
        //    monster.isActive = monsterObjects[i].activeSelf;
        //    monster.parent = monsterObjects[i].transform.parent;
        //    monster.position = new CustomVector3(monsterObjects[i].transform.position.x,
        //                                         monsterObjects[i].transform.position.y,
        //                                         monsterObjects[i].transform.position.z);

        //    monster.rotation = new CustomVector3(monsterObjects[i].transform.rotation.x,
        //                                         monsterObjects[i].transform.rotation.y,
        //                                         monsterObjects[i].transform.rotation.z,
        //                                         monsterObjects[i].transform.rotation.w);

        //    monsters.Add(monster);
        //}
    }
}
