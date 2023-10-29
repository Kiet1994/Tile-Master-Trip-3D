using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;

public class LevelSOEditorWindow : EditorWindow
{
    private LevelSO levelSO;
    private Vector2 scrollPosition = Vector2.zero;

    private int currentTab = 0;
    private string[] tabNames = { "Maps" , "Tile Configs" };

    [MenuItem("Custom Tools/Game Setting")]
    public static void ShowWindow()
    {
        GetWindow<LevelSOEditorWindow>("Game Setting");
    }

    private void OnEnable()
    {
        // Load or create your LevelSO asset here
        levelSO = AssetDatabase.LoadAssetAtPath<LevelSO>("Assets/GameAssets/Scripts/Levels.asset");


    }

    private void OnGUI()
    {
        GUILayout.Label("LevelSO Editor");

        // Display and edit your LevelSO fields here

        levelSO.tileSO = EditorGUILayout.ObjectField("TileSO", levelSO.tileSO, typeof(TileSO), false) as TileSO;
        //  AddLevel
        if (GUILayout.Button("Add Level"))
        {
            AddLevel();
        }

        //* Hiển thị các tab
        currentTab = GUILayout.Toolbar(currentTab, tabNames);
        switch (currentTab)
        {
            case 0:
                ShowTabMaps();
                break;
            case 1:
                ShowTabTileConfig();
                break;
            default:
                Debug.Log(default);
                break;
        }
        //*

        //* Hiển thị thông tin bên trái cửa sổ
        //EditorGUILayout.BeginHorizontal();
        //// Cột bên trái (hiển thị LevelSO)
        //EditorGUILayout.BeginVertical(GUILayout.Width(position.width * 0.5f));
        //*

        //* Vẽ một nút trên cửa sổ tùy chỉnh
        //if (GUI.Button(new Rect(100, 100, 50, 50), "Click Me"))
        //{
        //    // Xử lý khi nút được nhấn
        //    Debug.Log("Button clicked!");
        //}
        //*

        //* Tạo levels nếu không tìm thấy file theo đường dẫn
        if (levelSO == null)
        {
            GUILayout.Label("LevelSO not found. Create or load it first.");
            if (GUILayout.Button("Create LevelSO"))
            {
                levelSO = CreateLevelSO();
            }
            return;
        }
        //*



        
    }

    private LevelSO CreateLevelSO()
    {
        LevelSO newLevelSO = ScriptableObject.CreateInstance<LevelSO>();
        newLevelSO.levels = new Level[0]; // Initialize the levels array
        AssetDatabase.CreateAsset(newLevelSO, "Assets/GameAssets/Scripts/Levels.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return newLevelSO;
    }

    private string[] GetTileNames(List<Tile> tiles)
    {
        string[] tileNames = new string[tiles.Count];
        for (int i = 0; i < tiles.Count; i++) //
        {
            tileNames[i] = "Tile " + i;
        }
        return tileNames;
    }

    private void AddLevel()
    {
        if (levelSO != null)
        {
            Level newTile = new Level();
            ArrayUtility.Add(ref levelSO.levels, newTile);
        }
    }

    private void ShowTabTileConfig()
    {
        GUILayout.BeginArea(new Rect(10, 60, 350, 650));
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        // hiện tileSO
        if (levelSO.tileSO.tiles == null)
        {
            levelSO.tileSO.tiles = new List<Tile>();
        }
        for (int i = 0; i < levelSO.tileSO.tiles.Count; i++)
        {
            EditorGUILayout.LabelField("Tile " + i.ToString());
            levelSO.tileSO.tiles[i].id = EditorGUILayout.IntSlider("ID", levelSO.tileSO.tiles[i].id, 1, 200);
            levelSO.tileSO.tiles[i].sprite = (Sprite)EditorGUILayout.ObjectField("Sprite", levelSO.tileSO.tiles[i].sprite, typeof(Sprite), false);

            //if (GUILayout.Button("Remove Tile"))
            //{
            //   // RemoveTile(i);
            //}

            //EditorGUILayout.Space();
        }

        //if (GUILayout.Button("Add Tile"))
        //{
        //   // AddTile();
        //}
        GUILayout.EndArea();
        EditorGUILayout.EndScrollView();
    }
    private void ShowTabMaps()
    {
        foreach (Level level in levelSO.levels)
        {
            level.selectedTileIndex = GUILayout.Toolbar(levelSO.levels.Length, GetTileNames(levelSO.levels));
            EditLevels();
        }
    }
    private string[] GetTileNames(Level[] Map)
    {
        string[] tileNames = new string[Map.Length];
        for (int i = 0; i < Map.Length; i++) //
        {
            tileNames[i] = "Map " + i;
        }
        return tileNames;
    }
    private void EditLevels()
    {
        // Iterate through levels and display their properties
        foreach (Level level in levelSO.levels)
        {
            EditorGUILayout.LabelField("Level Name: " + level.name);
            level.name = EditorGUILayout.TextField("Name", level.name);
            level.displayName = EditorGUILayout.TextField("Display Name", level.displayName);
            level.level = EditorGUILayout.IntField("Level", level.level);
            level.playTime = EditorGUILayout.TextField("Play Time", level.playTime);
        }

        // 
        if (GUILayout.Button("Save LevelSO"))
        {
            // Save the changes to the LevelSO asset
            EditorUtility.SetDirty(levelSO);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        AddTile();
    }

    public void AddTile()
    {
        // Bắt đầu một hàng ngang
        EditorGUILayout.BeginHorizontal();

        // Hiển thị Popup
        int selectedOption = EditorGUILayout.Popup("Select an Option", 0, new string[] { "Option 1", "Option 2", "Option 3" });

        // Hiển thị Button "Add"
        if (GUILayout.Button("Add"))
        {
            // Xử lý khi bấm nút "Add"
        }

        // Hiển thị Button "Remove"
        if (GUILayout.Button("Remove"))
        {
            // Xử lý khi bấm nút "Remove"
        }

        // Hiển thị Label và TextField
        EditorGUILayout.LabelField("Quantity");
        string quantityText = EditorGUILayout.TextField("Quantity", "10");

        // Kết thúc hàng ngang
        EditorGUILayout.EndHorizontal();

        // Tiếp tục hiển thị các thành phần khác tùy theo yêu cầu của bạn
    }
}
