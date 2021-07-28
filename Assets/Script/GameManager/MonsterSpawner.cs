using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MonsterSpawner : MonoBehaviour
{
    public struct SpawnMonsterData
    {
        private string name;
        private float respawnTime;

        public string Name { get { return name; } }
        public float RespawnTime { get { return respawnTime; } }

        public SpawnMonsterData(string _name, float _respawnTime)
        {
            name = _name;
            respawnTime = _respawnTime;
        }
    }

    // 초기 몬스터 배치 정보
    [SerializeField]
    private MonsterSpawnPointsData firstSpawnDatas;

    // 몬스터의 오브젝트 정보
    [SerializeField]
    private GameObject[] monsterObjects;

    // 몬스터 리스트 (key: 이름, value: 오브젝트)
    private Dictionary<string, GameObject> monsterList;

    // 몬스터 오브젝트 폴
    private Dictionary<string, Queue<GameObject>> monsterPoolManager;
    public Dictionary<string, Queue<GameObject>> MonsterPoolManager { get { return monsterPoolManager; } }

    // 몬스터 위치, 이름, 리스폰시간 데이터 리스트
    private Dictionary<Vector3, SpawnMonsterData> spawnPointsData;
    public Dictionary<Vector3, SpawnMonsterData> SpawnPointsData { get { return spawnPointsData; } }

    // 플레이어 위치정보
    private Transform player;


    void Start()
    {
        Init();
    }

    public void Init()
    {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }


        if (monsterList == null)
        {
            monsterList = new Dictionary<string, GameObject>();

            // 몬스터 리스트 작성
            foreach (var monster in monsterObjects)
            {
                monsterList.Add(monster.name, monster);
            }
        }

        if(monsterPoolManager == null)
        {
            monsterPoolManager = new Dictionary<string, Queue<GameObject>>();
        }

        if(spawnPointsData == null)
        {
            spawnPointsData = new Dictionary<Vector3, SpawnMonsterData>();
        }

        foreach(var queue in monsterPoolManager.Values)
        {
            int count = queue.Count;

            for(int i = 0; i < count; i++)
            {
                var obj = queue.Dequeue();
                Destroy(obj);
            }
        }

        monsterPoolManager.Clear();
        spawnPointsData.Clear();

        // 스폰 데이터 생성
        foreach (var data in firstSpawnDatas.DataList)
        {
            spawnPointsData.Add(data.originPosition, new SpawnMonsterData(data.monsterName, 0));
        }

        // 오브젝트 풀 생성
        foreach (var monster in monsterObjects)
        {
            monsterPoolManager.Add(monster.name, new Queue<GameObject>());
        }

        // 몬스터 생성
        foreach (var monsterSpawnInfo in firstSpawnDatas.DataList)
        {
            string name = monsterSpawnInfo.monsterName;
            CreateMonster(monsterPoolManager[name], monsterList[name]);
        }

        // 몬스터 사망시 이벤트 등록
        GameEvent.Instance.EventMonsterDead += RemoveMonster;
    }

    // 오브젝트 풀에 몬스터 생성
    public void CreateMonster(Queue<GameObject> queue, GameObject monster, int count = 1)
    {
        for(int i = 0; i < count; i++)
        {
            var tempMonster = Instantiate(monster, transform.position, Quaternion.identity, transform);
            //tempMonster.GetComponent<Monster>().OriginPos = 
            queue.Enqueue(tempMonster);
            tempMonster.SetActive(false);
        }
    }

    // 오브젝트 풀에서 몬스터 가져오기
    public void GetMonster(string monsterName, Vector3 spawnPosition)
    {
        if (monsterPoolManager[monsterName].Count == 0)
        {
            var tempMonster = Instantiate(monsterList[monsterName], spawnPosition, Quaternion.identity);
            tempMonster.SetActive(true);
            return;
        }

        var monster = monsterPoolManager[monsterName].Dequeue();
        monster.transform.SetParent(null);
        monster.transform.position = spawnPosition;
        monster.SetActive(true);
    }


    // 몬스터를 오브젝트 풀로 돌려보내기
    public void RemoveMonster(GameObject monster, Vector3 originPoint, string monsterName, float respawnTime)
    {
        // 몬스터 리스폰 리스트에 등록
        spawnPointsData.Add(originPoint, new SpawnMonsterData(monsterName, respawnTime));

        // 몬스터 오브젝트 폴에 등록
        monsterPoolManager[monsterName].Enqueue(monster);
        monster.transform.SetParent(transform);
        monster.transform.position = transform.position;
        monster.SetActive(false);
    }

    // 몬스터 리스폰 리스트에서 제거
    void DisRegistrateSpawnPointDatas()
    {
        if (spawnPointsData.Count == 0)
            return;

        var points = from point in spawnPointsData.Keys.ToList()
                where (Vector3.SqrMagnitude(player.position - point) < 100 && spawnPointsData[point].RespawnTime <= 0)
                select point;

        foreach(var point in points)
        {
            GetMonster(spawnPointsData[point].Name, point);
            spawnPointsData.Remove(point);
        }        
    }

    // 리스폰 시간 감소
    private void CalculateRespawnTime()
    {
        if (spawnPointsData.Count == 0)
            return;

        var keys = from key in spawnPointsData.Keys.ToList()
                   where spawnPointsData[key].RespawnTime > 0
                   select key;

        if (keys != null)
        {
            foreach (var key in keys)
            {
                spawnPointsData[key] = new SpawnMonsterData
                    (spawnPointsData[key].Name, spawnPointsData[key].RespawnTime - Time.smoothDeltaTime);
            }
        }
    }


    private void Update()
    {
        CalculateRespawnTime();
        DisRegistrateSpawnPointDatas();
    }

    public void Test()
    {
        foreach(var data in spawnPointsData)
        {
            Debug.Log(data.Key);
        }
    }
}
