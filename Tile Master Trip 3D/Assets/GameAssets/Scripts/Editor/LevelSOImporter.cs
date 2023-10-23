
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelSO))]
public class LevelSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        LevelSO levelSO = (LevelSO)target;

        if(levelSO.tileSO != null && levelSO.tileChoices.Count < levelSO.tileSO.tiles.Count)
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
    }

    // Hàm để lấy danh sách tên của các Tile
    private string[] GetTileNames(List<Tile> tiles)
    {
        string[] tileNames = new string[tiles.Count];
        for (int i = 0; i < tiles.Count; i++)
        {
            tileNames[i] =i.ToString(); // Thay thế name bằng thuộc tính bạn muốn hiển thị
        }
        return tileNames;
    }
}

