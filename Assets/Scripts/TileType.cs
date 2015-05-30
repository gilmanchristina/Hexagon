using UnityEngine;
using System.Collections;

[System.Serializable]
public class TileType{

    public string name;
    public Material hex_Material;

    public float movementCost = 1;
    public bool is_Walkable;
    public bool is_Visable;
    public bool is_seen;
}