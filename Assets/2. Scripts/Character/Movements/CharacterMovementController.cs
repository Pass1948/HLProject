using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CharacterMovementController : MonoBehaviour 
{
    [SerializeField] private Tilemap tilemap;

    private Collider _collider;
    private Vector3Int _position;


    private void Awake()
    {
        _collider = GetComponent<Collider>();

        transform.position = tilemap.GetCellCenterWorld(_position);
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
