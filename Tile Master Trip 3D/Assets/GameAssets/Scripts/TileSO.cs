using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TileSO : ScriptableObject
{
    public Tile[] tiles;
}
[System.Serializable]
public class Tile
{
    [Range(1, 200)]
    public int id;
    public Sprite sprite;
}