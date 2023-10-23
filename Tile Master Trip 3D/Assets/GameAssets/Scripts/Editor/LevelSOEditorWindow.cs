using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class LevelSOEditorWindow : EditorWindow
{
    private LevelSO levelSO;
    private Vector2 scrollPosition;

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
        if (levelSO == null)
        {
            GUILayout.Label("LevelSO not found. Create or load it first.");
            if (GUILayout.Button("Create LevelSO"))
            {
                levelSO = CreateLevelSO();
            }
            return;
        }

        GUILayout.Label("LevelSO Editor");

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // Display and edit your LevelSO fields here
        levelSO.tileSO = EditorGUILayout.ObjectField("TileSO", levelSO.tileSO, typeof(TileSO), false) as TileSO;

        // Iterate through levels and display their properties
        foreach (Level level in levelSO.levels)
        {
            EditorGUILayout.LabelField("Level Name: " + level.name);
            level.name = EditorGUILayout.TextField("Name", level.name);
            level.displayName = EditorGUILayout.TextField("Display Name", level.displayName);
            level.level = EditorGUILayout.IntField("Level", level.level);
            level.playTime = EditorGUILayout.TextField("Play Time", level.playTime);

            level.selectedTileIndex = EditorGUILayout.Popup("Select a Tile", level.selectedTileIndex, GetTileNames(levelSO.tileChoices));

            
        }
        if (levelSO.tileSO != null && levelSO.tileChoices.Count < levelSO.tileSO.tiles.Count)
        {
            for (int i = 0; i < levelSO.tileSO.tiles.Count; i++)
            {
                var selectedTile = levelSO.tileSO.tiles[i];
                levelSO.tileChoices.Add(selectedTile);
            }
        }


        if (GUILayout.Button("Add Selected Tile"))
        {
            // Người dùng đã nhấn nút "Add Selected Tile", thêm Tile vào danh sách
            foreach (Level level in levelSO.levels)
            {
                if (level.selectedTileIndex >= 0 && level.selectedTileIndex < levelSO.tileChoices.Count)
                {
                    Tile selectedTile = levelSO.tileChoices[level.selectedTileIndex];
                    level.tiles.Add(selectedTile);
                }
            }
        }

        // Hiển thị danh sách Tile và Popup
        foreach (Level level in levelSO.levels)
        {
            level.selectedTileIndex = EditorGUILayout.Popup("Select a Tile", level.selectedTileIndex, GetTileNames(levelSO.tileChoices));
        }
        EditorGUILayout.EndScrollView();
        if (levelSO.tileSO.tiles == null)
        {
            levelSO.tileSO.tiles = new List<Tile>();
        }

        for (int i = 0; i < levelSO.tileSO.tiles.Count; i++)
        {
            EditorGUILayout.LabelField("Tile " + i.ToString());
            levelSO.tileSO.tiles[i].id = EditorGUILayout.IntSlider("ID", levelSO.tileSO.tiles[i].id, 1, 200);
            levelSO.tileSO.tiles[i].sprite = (Sprite)EditorGUILayout.ObjectField("Sprite", levelSO.tileSO.tiles[i].sprite, typeof(Sprite), false);

            if (GUILayout.Button("Remove Tile"))
            {
               // RemoveTile(i);
            }

            EditorGUILayout.Space();
        }

        if (GUILayout.Button("Add Tile"))
        {
           // AddTile();
        }
        if (GUILayout.Button("Save LevelSO"))
        {
            // Save the changes to the LevelSO asset
            EditorUtility.SetDirty(levelSO);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
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
        for (int i = 0; i < tiles.Count; i++)
        {
            tileNames[i] = "Tile " + tiles[i];
        }
        return tileNames;
    }
}
