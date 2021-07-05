using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SpawnPointSettingWindow : EditorWindow
{
    Texture2D headSectionTexture;
    Texture2D infoSectionTexture;
    Texture2D buttonSectionTexture;

    Color headerSectionColor = new Color(151f / 255f, 202f / 255f ,1);
    Color infoSectionColor = new Color(105f / 255f, 143f / 255f, 178f / 255f);
    Color buttonSectionColor = new Color(135f / 255f, 184f / 255, 299f / 255f);

    Rect headerSection;
    Rect infoSection;
    Rect buttonSection;

    GUISkin skin;

    MonsterData[] monsterDatas;
    List<string> monsterNameList = new List<string>();
    Vector2 scrollPos;
    int currMonsterNum = 0;

    private MonsterData monsterData;

    [MenuItem("Window/Spawn Point Setting Window")]
    static void OpenWindow()
    {
        SpawnPointSettingWindow window = (SpawnPointSettingWindow)GetWindow(typeof(SpawnPointSettingWindow));
        window.minSize = new Vector2(350, 500);
        window.Show();
    }

    private void OnEnable()
    {
        InitTexture();
        InitData();
        skin = Resources.Load<GUISkin>("GUIStyles/SpawnPointSkin");
    }

    private void InitTexture()
    {
        headSectionTexture = new Texture2D(1,1);
        headSectionTexture.SetPixel(0, 0, headerSectionColor);
        headSectionTexture.Apply();

        infoSectionTexture = new Texture2D(1,1);
        infoSectionTexture.SetPixel(0, 0, infoSectionColor);
        infoSectionTexture.Apply();

        buttonSectionTexture = new Texture2D(1,1);
        buttonSectionTexture.SetPixel(0, 0, buttonSectionColor);
        buttonSectionTexture.Apply();
    }

    private void InitData()
    {
        monsterDatas = Resources.LoadAll<MonsterData>("Datas/Monsters");

        foreach (var data in monsterDatas)
        {
            monsterNameList.Add(data.MonsterName);
        }
    }

    private void OnGUI()
    {
        DrawLayout();
        DrawHeader();
        DrawInfo();
        DrawButton();
    }

    private void DrawLayout()
    {
        headerSection.x = 0;
        headerSection.y = 0;
        headerSection.width = position.width;
        headerSection.height = position.height / 5;

        infoSection.x = 0;
        infoSection.y = headerSection.height;
        infoSection.width = position.width;
        infoSection.height = (position.height / 5) * 4;

        buttonSection.x = 0;
        buttonSection.y = infoSection.height;
        buttonSection.width = position.width;
        buttonSection.height = position.height / 5;

        GUI.DrawTexture(headerSection, headSectionTexture);
        GUI.DrawTexture(infoSection, infoSectionTexture);
        GUI.DrawTexture(buttonSection, buttonSectionTexture);
    }

    private void DrawHeader()
    {
        GUILayout.BeginArea(headerSection);

        GUILayout.Label("Spawn Point Setting Window", skin.GetStyle("Header1"));

        GUILayout.EndArea();
    }

    private void DrawInfo()
    {
        GUILayout.BeginArea(infoSection);

        GUILayout.BeginHorizontal();
        GUILayout.Label("name", skin.GetStyle("Info"));
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true);
        currMonsterNum = EditorGUILayout.Popup(currMonsterNum, monsterNameList.ToArray());
        EditorGUILayout.EndScrollView();
        GUILayout.EndHorizontal();

        GUILayout.Label("Position", skin.GetStyle("Info"));

        

        GUILayout.EndArea();
    }

    private void DrawButton()
    {
        GUILayout.BeginArea(buttonSection);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Set Position", GUILayout.Height(70)))
        {
            Debug.Log("Input Set Position Button");
        }

        if(GUILayout.Button("Save All", GUILayout.Height(70)))
        {
            Debug.Log("Input Save All Button");
        }
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }
}
