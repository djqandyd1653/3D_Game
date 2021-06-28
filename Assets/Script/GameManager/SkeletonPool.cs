using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonPool : MonoBehaviour
{
    public GameObject skeleton;
    private Queue<GameObject> skeletonQueue;

    void Start()
    {
        skeletonQueue = new Queue<GameObject>();
        CreateSkeleton(20);
    }

    private void CreateSkeleton(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var skeletonObject = Instantiate(skeleton, transform.position, Quaternion.identity, transform);
            skeletonQueue.Enqueue(skeletonObject);
            skeletonObject.SetActive(false);
        }
    }

    public void GetSkeleton(Vector3 position)
    {
        if(skeletonQueue.Count == 0)
        {
            CreateSkeleton(1);
        }

        var tempSkeleton = skeletonQueue.Dequeue();
        tempSkeleton.transform.position = position;
        tempSkeleton.transform.parent = null;
        tempSkeleton.SetActive(true);
    }

    public void ReturnSkeleton(GameObject _skeleton)
    {
        _skeleton.transform.position = transform.position;
        _skeleton.transform.SetParent(transform);
        skeletonQueue.Enqueue(_skeleton);
        _skeleton.SetActive(false);
    }
}
