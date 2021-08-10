using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

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

    GameObject[] monsterObjects;
    MonsterSpawnPointsData monsterSpawnPointsData;

    List<string> monsterNameList = new List<string>();
    List<GameObject> summonedMonsterList = new List<GameObject>();

    int buttonSize = 50;
    int monsterNum = 0;

    private MonsterData monsterData;

    [MenuItem("Window/Spawn Point Setting Window")]
    static void OpenWindow()
    {
        SpawnPointSettingWindow window = (SpawnPointSettingWindow)GetWindow(typeof(SpawnPointSettingWindow));
        window.minSize = new Vector2(300, 200);
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
        monsterSpawnPointsData = Resources.Load<MonsterSpawnPointsData>("Datas/Scriptable Data/SpawnPosition/New Monster Spawn Points Data");
        monsterObjects = Resources.LoadAll<GameObject>("Prefabs/Monster");

        foreach (var data in monsterObjects)
        {
            monsterNameList.Add(data.name);
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
        buttonSection.y = position.height - buttonSize;
        buttonSection.width = position.width;
        buttonSection.height = position.height;

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
        monsterNum = EditorGUILayout.Popup(monsterNum, monsterNameList.ToArray(), GUILayout.Height(30));
        GUILayout.EndHorizontal();

        GUILayout.EndArea();
    }

    private void DrawButton()
    {
        GUILayout.BeginArea(buttonSection);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Spawn\nMonster", GUILayout.Height(buttonSize)))
        {
            Debug.Log("Input Set Position Button");
            SpawnTempMonster();
        }

        if(GUILayout.Button("Remove\nMonster", GUILayout.Height(buttonSize)))
        {
            RemoveMonster();
        }

        if(GUILayout.Button("Save All", GUILayout.Height(buttonSize)))
        {
            foreach (var data in summonedMonsterList.ToArray())
            {
                string replaceName = data.name.Replace("(Clone)", "");
                monsterSpawnPointsData.AddSpawnData(data.transform.position, replaceName);
                summonedMonsterList.Remove(data);
                DestroyImmediate(data);
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    private void SpawnTempMonster()
    {
        Vector3 sceneViewCenterPoint = new Vector3(SceneView.lastActiveSceneView.camera.scaledPixelWidth / 2,
                                                   SceneView.lastActiveSceneView.camera.scaledPixelHeight / 2,
                                                   0);

        Ray ray = SceneView.lastActiveSceneView.camera.ScreenPointToRay(sceneViewCenterPoint);

        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 hitPoint = hit.point;
            var tempMonster = Instantiate(monsterObjects[monsterNum], hitPoint, Quaternion.identity);
            summonedMonsterList.Add(tempMonster);
            Selection.activeGameObject = tempMonster;
        }
    }

    private void RemoveMonster()
    {
        if(Selection.gameObjects.Length == 0)
        {
            Debug.LogError("No object selected'");
            return;
        }

        bool tempObject = false;

        foreach (var selectedObject in Selection.gameObjects)
        {
            tempObject = summonedMonsterList.Exists(x => GameObject.ReferenceEquals(x, selectedObject));

            if (!tempObject)
            {
                RemovalWarningWindow.OpenWindow(ref summonedMonsterList);
                return;
            }
        }

        foreach (var selectedObject in Selection.gameObjects)
        {
            if (summonedMonsterList.Remove(selectedObject))
            {
                DestroyImmediate(selectedObject);
            }
        }
    }
}

public class RemovalWarningWindow : EditorWindow
{
    static RemovalWarningWindow window;
    static List<GameObject> summonedMonsterList;
    static GameObject[] selectedObjectArray;
    Rect infoSection;
    Rect buttonSection;
    int buttonSize = 50;
    Vector2 scrollPosition = Vector2.zero;

    GUISkin skin;

    public static void OpenWindow(ref List<GameObject> _summonedMonsterList)
    {
        selectedObjectArray = Selection.gameObjects;
        summonedMonsterList = _summonedMonsterList;
        window = (RemovalWarningWindow)GetWindow(typeof(RemovalWarningWindow));
        window.minSize = new Vector2(300, 150);
        window.Show();
    }

    private void OnEnable()
    {
        skin = Resources.Load<GUISkin>("GUIStyles/SpawnPointSkin");
    }

    private void OnGUI()
    {
        DrawLayout();
        DrawList();
        DrawButton();
    }

    private void DrawLayout()
    {
        infoSection.x = 0;
        infoSection.y = 0;
        infoSection.width = position.width;
        infoSection.height = position.height - buttonSize;

        buttonSection.x = 0;
        buttonSection.y = position.height - buttonSize;
        buttonSection.width = position.width;
        buttonSection.height = position.height;
    }

    private void DrawList()
    {
        GUILayout.BeginArea(infoSection);

        GUILayout.Label("Warning! : An object not in the list exists. \nAre you sure you want to remove it?", skin.GetStyle("Remove Warning"));

        GUILayout.Label("------------------\nSelected Object List\n------------------", skin.GetStyle("List"));

        scrollPosition = GUILayout.BeginScrollView(scrollPosition, true, true);
        foreach (var selectedObject in selectedObjectArray)
        {
            bool tempObject = summonedMonsterList.Exists(x => GameObject.ReferenceEquals(x, selectedObject));

            if (tempObject)
            {
                GUILayout.Label(selectedObject.name, skin.GetStyle("Remove Possible"));
                continue;
            }

            GUILayout.Label(selectedObject.name, skin.GetStyle("Remove Impossible"));
        }
        GUILayout.EndScrollView();

        GUILayout.EndArea();
    }

    private void DrawButton()
    {
        GUILayout.BeginArea(buttonSection);

        GUILayout.BeginHorizontal();

        if(GUILayout.Button("Remove", GUILayout.Height(buttonSize)))
        {
            foreach (var selectedObject in selectedObjectArray)
            {
                if (summonedMonsterList.Remove(selectedObject))
                {
                    DestroyImmediate(selectedObject);
                }
            }

            window.Close();
        }

        if (GUILayout.Button("Cancel", GUILayout.Height(buttonSize)))
        {
            window.Close();
        }

        GUILayout.EndHorizontal();

        GUILayout.EndArea();
    }
}
