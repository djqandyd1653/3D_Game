using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Monster))]
public class DrawWireArc : Editor
{
    private void OnSceneGUI()
    {
        Handles.color = Color.red;
        Monster monster = (Monster)target;
        Handles.DrawWireArc(monster.transform.position,
        monster.transform.up, -monster.transform.right, 360, monster.searchRange);
    }
}
