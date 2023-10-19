using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.UI.Image;

public class PlayerControler : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    private GameObject currentHitObject;
    private GameObject lastHitObject;
    private Camera _mainCamera;
    private Vector3 _mouseInput = Vector3.zero;
    [SerializeField] private GameObject tray;
    public Vector3[] positionTiles;
    public GameObject[] tiles;


    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void Start()
    {
        //Is Adjacent
        var transfrom = tray.GetComponentsInChildren<Transform>()
        .Where(t => t != tray.transform) // Loại bỏ transform của đối tượng gốc
        .ToArray();
        positionTiles = new Vector3[transfrom.Length];
        tiles = new GameObject[transfrom.Length];

        for (int i = 0; i < transfrom.Length; i++)
        {
            positionTiles[i] = transfrom[i].position;
        }
    }

    private void Update()
    {
        ReceiveInput();
              
    }

    private void ReceiveInput()
    {
        _mouseInput.x = Input.mousePosition.x;
        _mouseInput.y = Input.mousePosition.y;
        _mouseInput.z = _mainCamera.transform.position.y;

        if (Input.GetMouseButton(0)) 
        {
            CreateRaycast();
        }

        if (Input.GetMouseButtonUp(0))
        {
            MoveTile();
            DestroyTiles();
        }    
    }

    private void MoveTile()
    {
        if (!lastHitObject) return;
        //di chuyển vào khay
        if (tiles[0] == null) // khay rỗng 
        {
            SetTile(lastHitObject, 0);
        }
        else // khay có tile
        {
            for (int i = (tiles.Length - 1); i >= 0; i--)
            {
                if (tiles[i] != null) // tại vị trí có tile 
                {
                    if (CompareID(lastHitObject, tiles[i])) //thực hiện so sánh ID nếu lớn hơn or =
                    {
                        SetTile(lastHitObject, i + 1);
                        break;
                    }
                    else // nếu nhỏ hơn di chuyển vị trí tile lên 1 ô và xét tile[i - 1] ở lần tiếp theo
                    {
                        SetTile(tiles[i], i + 1);
                        if (i == 0) SetTile(lastHitObject, 0);
                    }
                }
            }
        }
        OriginalState();
    }

    private void DestroyTiles()
    {
        int posEmply = 0;
        for (int i = (tiles.Length - 1); i >= 2; i--)
        {
            if (tiles[i] == null) continue;       
            if ((tiles[i].GetComponent<ManagerTile>().ID == tiles[i - 1].GetComponent<ManagerTile>().ID) &&
                (tiles[i].GetComponent<ManagerTile>().ID == tiles[i - 2].GetComponent<ManagerTile>().ID))
            { // thay bằng pooling
                Destroy(tiles[i]); 
                Destroy(tiles[i - 1]);
                Destroy(tiles[i - 2]);
                posEmply = i + 1;
                break;
            }               
        }
        if (posEmply != 0) Fill(posEmply); 
    }

    private void Fill(int posEmply)
    {
        for (int i = posEmply; i < tiles.Length; i++)
        {
            if (tiles[i] == null) continue;
            SetTile(tiles[i], i - 3);
            tiles[i] = null;
        }
    }

    private void CreateRaycast()
    {
        Vector3 mouseWorldCoordinates = _mainCamera.ScreenToWorldPoint(_mouseInput);
        Vector3 direction = new Vector3(mouseWorldCoordinates.x, 0, mouseWorldCoordinates.z);
        Vector3 directionRay = new Vector3(mouseWorldCoordinates.x, -_mouseInput.z, mouseWorldCoordinates.z);
        RaycastHit hit;
        //
        Debug.DrawRay(_mainCamera.transform.position, directionRay, Color.red);
        //transform.position = Vector3.MoveTowards(transform.position, direction, Time.deltaTime * speed);
        //if (direction != transform.position)
        //{
        //    Vector3 targetDirection = direction - transform.position;
        //    targetDirection.y = 0;
        //    transform.right = targetDirection;
        //}
        //    
        //int layerMask = 1 << LayerMask.NameToLayer("Piece");
        if (Physics.Raycast(_mainCamera.transform.position, directionRay, out hit, 100, 8))
        {
            Debug.Log(hit.collider.gameObject.name);
            lastHitObject = hit.collider.gameObject;
            if (currentHitObject != lastHitObject)
            {
                currentHitObject?.GetComponentInParent<ManagerTile>().Unselected();
                currentHitObject = lastHitObject;
                lastHitObject.GetComponentInParent<ManagerTile>().Selected();
            }
        }
        else
        {
            if (lastHitObject != null)
            {
                OriginalState();
            }
        }
    }

    private bool CompareID(GameObject lastHitObject, GameObject tile)
    {
        return (lastHitObject.GetComponent<ManagerTile>().ID >= tile.GetComponent<ManagerTile>().ID);
    }

    private void SetTile(in GameObject tile,int pos)
    {
        tiles[pos] = tile;
        tiles[pos].GetComponent<Rigidbody>().isKinematic = true;
        tiles[pos].transform.rotation = Quaternion.identity; //corotine
        tiles[pos].transform.position = positionTiles[pos];
    }

    private void OriginalState()
    {
        lastHitObject.GetComponentInParent<ManagerTile>().Unselected();
        currentHitObject = null;
        lastHitObject = null;
    }
}
