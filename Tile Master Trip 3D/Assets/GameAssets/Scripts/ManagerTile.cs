using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerTile : MonoBehaviour
{
    [SerializeField] private GameObject borders;
    [HideInInspector] public Tile tile;
    private int id;
    public int ID
    {
        get => id; 
    }
    private void Start()
    {
        id = tile.id;
        gameObject.GetComponentInChildren<SpriteRenderer>().sprite = tile.sprite;
    }
    public void Selected()
    {
        borders.SetActive(true);
        transform.localScale = Vector3.one * 1.1f;
    }

    public void Unselected()
    {
        borders.SetActive(false);
        transform.localScale = Vector3.one;
    }
}
