using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{

    // Test Start

    Queue<GameObject> tempQue;

    // Tset End

    [SerializeField]
    private GameObject[] monsterObjects;

    private Dictionary<string, Queue<GameObject>> monsterPoolManager;

    void Start()
    {

        // Test Start

        tempQue = new Queue<GameObject>();

        // Tset End

        monsterPoolManager = new Dictionary<string, Queue<GameObject>>();

        foreach(var monsterObject in monsterObjects)
        {
            monsterPoolManager.Add(monsterObject.name, new Queue<GameObject>());
            CreateMonster(monsterPoolManager[monsterObject.name], monsterObject, 5);
        }
    }

    void CreateMonster(Queue<GameObject> queue, GameObject _monster, int count = 1)
    {
        for(int i = 0; i < count; i++)
        {
            var monster = Instantiate(_monster, transform.position, Quaternion.identity, transform);
            queue.Enqueue(monster);
            monster.SetActive(false);
        }
    }

    void GetMonster(GameObject _monster, Vector3 spawnPosition)
    {
        //if(monsterPoolManager[_monster.name].Count == 0)
        if (monsterPoolManager["Goblin"].Count == 0)           // (임시)
        {
            var tempMonster = Instantiate(_monster, spawnPosition, Quaternion.identity);
            tempMonster.SetActive(true);

            // Test Start

            tempQue.Enqueue(tempMonster);

            // Tset End

            return;
        }

        var monster = monsterPoolManager[_monster.name].Dequeue();
        monster.transform.SetParent(null);
        monster.transform.position = spawnPosition;
        monster.SetActive(true);

        // Test Start

        tempQue.Enqueue(monster);

        // Tset End
    }

    void RemoveMonster(GameObject _monster)
    {
        //monsterPoolManager[_monster.name].Enqueue(_monster);
        monsterPoolManager["Goblin"].Enqueue(_monster); // (임시) Test
        _monster.transform.SetParent(transform);
        _monster.transform.position = transform.position;
        _monster.SetActive(false);
    }

    // Test Start

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.O))
        {
            if(tempQue.Count != 0)
            {
                RemoveMonster(tempQue.Dequeue());
            }
        }

        if(Input.GetKeyDown(KeyCode.P))
        {
            GetMonster(monsterObjects[0], new Vector3(Random.Range(-2, 3) * 5, 0, 25));
        }
    }

    // Tset End
}
