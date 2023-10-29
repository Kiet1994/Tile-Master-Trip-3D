using UnityEditor;
using UnityEngine;

public class GameSettingWindow : EditorWindow
{
    private int currentTab = 0;
    private string[] tabNames = { "Tile Configs", "Maps" };

    private TileSOManager tileSOManager;
    private LevelSO mapSO;
    private Vector2 mapSOScrollPosition = Vector2.zero;
    private string mapSOFilePath = "Assets/GameAssets/Scripts/Maps.asset";

    [MenuItem("Window/Game Setting")]
    public static void ShowWindow()
    {
        GameSettingWindow window = GetWindow<GameSettingWindow>(false, "Game Setting");
        window.tileSOManager = new TileSOManager();
        window.tileSOManager.LoadTileSO();
        window.LoadMapSO();
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        currentTab = GUILayout.Toolbar(currentTab, tabNames);
        GUILayout.EndHorizontal();

        if (currentTab == 0) // Game Setting tab
        {
            tileSOManager.Draw();
        }
        else if (currentTab == 1) // Maps tab
        {
            mapSOScrollPosition = EditorGUILayout.BeginScrollView(mapSOScrollPosition);

            mapSO = (LevelSO)EditorGUILayout.ObjectField("MapSO", mapSO, typeof(LevelSO), false);

            if (mapSO != null)
            {
                // Your MapSO content here
            }

            EditorGUILayout.EndScrollView();
        }
    }

    private void LoadMapSO()
    {
        mapSO = AssetDatabase.LoadAssetAtPath<LevelSO>(mapSOFilePath);
        if (mapSO == null)
        {
            Debug.LogError("MapSO not found at path: " + mapSOFilePath);
        }
    }


}


public class TileSOManager
{
    private TileSO tileSO;
    private Vector2 scrollPosition = Vector2.zero;
    private string mapSOFilePath = "Assets/GameAssets/Scripts/Tiles.asset";
    public void LoadTileSO()
    {
        tileSO = AssetDatabase.LoadAssetAtPath<TileSO>(mapSOFilePath);
        if (tileSO == null)
        {
            Debug.LogError("MapSO not found at path: " + mapSOFilePath);
        }
    }
    public void Draw()
    {
        // Tạo một ScrollView với thanh cuộn bên phải
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        if (tileSO != null)
        {
            EditorGUILayout.Space();

            if (tileSO.tiles == null)
            {
               // tileSO.tiles = new Tile[0];
            }

            //for (int i = 0; i < tileSO.tiles.Length; i++)
            //{
            //    EditorGUILayout.LabelField("Tile " + i.ToString());
            //    tileSO.tiles[i].id = EditorGUILayout.IntSlider("ID", tileSO.tiles[i].id, 1, 200);
            //    tileSO.tiles[i].sprite = (Sprite)EditorGUILayout.ObjectField("Sprite", tileSO.tiles[i].sprite, typeof(Sprite), false);

            //    if (GUILayout.Button("Remove Tile"))
            //    {
            //        RemoveTile(i);
            //    }

            //    EditorGUILayout.Space();
            //}

            if (GUILayout.Button("Add Tile"))
            {
                AddTile();
            }
        }
        EditorGUILayout.EndScrollView(); //// Kết thúc ScrollView
    }

    private void AddTile()
    {
        if (tileSO != null)
        {
            Tile newTile = new Tile();
            //ArrayUtility.Add(ref tileSO.tiles, newTile);
        }
    }

    private void RemoveTile(int index)
    {
        //if (tileSO != null && index >= 0 && index < tileSO.tiles.Length)
        //{
        //    //ArrayUtility.RemoveAt(ref tileSO.tiles, index);
        //}
    }
}
