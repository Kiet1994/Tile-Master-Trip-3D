using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstanceTiles : MonoBehaviour
{
    [SerializeField] GameObject tilePrefap;
    [SerializeField] Vector3 rangeRandom;
    [SerializeField] TileSO tileSO;
    [SerializeField] int tileCount = 10;
    private void Start()
    {
        for (int i = 1; i <= tileCount; i++)
        {
            var typeTile = Random.Range(0 , tileSO.tiles.Count);
            for (int j = 1; j <= 3; j++)
            {
            var tile = Instantiate(tilePrefap, new Vector3(Random.Range(-rangeRandom.x, rangeRandom.x), rangeRandom.y, Random.Range(-rangeRandom.z, rangeRandom.z)),Quaternion.identity);
            var managerTile = tile.GetComponent<ManagerTile>();
            managerTile.tile = tileSO.tiles[typeTile];
            }
        }
    }
}
