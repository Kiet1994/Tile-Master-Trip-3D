using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LevelSO : ScriptableObject
{

	public TileSO tileSO;
    public Level[] levels;
    public List<Tile> tileChoices;
}


[System.Serializable]
public class Level
{
    public string name;
    public string displayName;
    public int level;
    public string playTime;
    public int selectedTileIndex; // Chỉ mục của Tile được chọn
    public List<Tile> tiles = new List<Tile>();
}

//[System.Serializable]
//public class TilePool
//{
//    public int id;
//    public TileSO tileSO;
//    public Tile[] tiles;
//    public int quantity;
//}