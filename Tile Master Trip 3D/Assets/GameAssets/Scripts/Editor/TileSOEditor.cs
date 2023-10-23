using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

public class TileSOEditor : EditorWindow
{
    private TileSO tileSO;
    private Vector2 scrollPosition = Vector2.zero;

    private string tileSOFilePath = "Assets/GameAssets/Scripts/Tiles.asset";

    [MenuItem("Window/TileSO Editor")]
    public static void ShowWindow()
    {
        TileSOEditor window = GetWindow<TileSOEditor>(false, "TileSO Editor");
        window.LoadTileSO(); // Tải TileSO khi mở cửa sổ
    }

    private void OnGUI()
    {
        // Tạo một ScrollView với thanh cuộn bên phải
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        // Hiển thị EditorWindow cho TileSO
        tileSO = (TileSO)EditorGUILayout.ObjectField("TileSO", tileSO, typeof(TileSO), false);

        if (tileSO.tiles == null)
        {
            tileSO.tiles = new List<Tile>();
        }

        for (int i = 0; i < tileSO.tiles.Count; i++)
        {
            EditorGUILayout.LabelField("Tile " + i.ToString());
            tileSO.tiles[i].id = EditorGUILayout.IntSlider("ID", tileSO.tiles[i].id, 1, 200);
            tileSO.tiles[i].sprite = (Sprite)EditorGUILayout.ObjectField("Sprite", tileSO.tiles[i].sprite, typeof(Sprite), false);

            if (GUILayout.Button("Remove Tile"))
            {
                RemoveTile(i);
            }

            EditorGUILayout.Space();
        }

        if (GUILayout.Button("Add Tile"))
        {
            AddTile();
        }   

    EditorGUILayout.EndScrollView(); //// Kết thúc ScrollView
    }
    private void LoadTileSO()
    {
        tileSO = AssetDatabase.LoadAssetAtPath<TileSO>(tileSOFilePath);
        if (tileSO == null)
        {
            Debug.LogError("TileSO not found at path: " + tileSOFilePath);
        }
    }

	private void AddTile()
	{
		//if (tileSO != null)
		//{
		//	Tile newTile = new Tile();
		//	ArrayUtility.Add(ref tileSO.tiles, newTile);
		//}
	}

	private void RemoveTile(int index)
	{
		//if (tileSO != null && index >= 0 && index < tileSO.tiles.Length)
		//{
		//	ArrayUtility.RemoveAt(ref tileSO.tiles, index);
		//}
	}
}
