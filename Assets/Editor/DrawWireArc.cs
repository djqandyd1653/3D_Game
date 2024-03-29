﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Skeleton))]
public class DrawWireArc : Editor
{
    private void OnSceneGUI()
    {
        Handles.color = Color.blue;
        Skeleton monster = (Skeleton)target;
        Handles.DrawWireArc(monster.transform.position,
        monster.transform.up, -monster.transform.right, 360, monster.monsterData.SearchRange);
    }
}
