using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;


public class Board_Editor : MonoBehaviour
{

    #region fields
    public TileType_Editor[] tileTypes;
    public ObjectType_Editor[] objectTypes;
    public Hex_Editor _hex;

    public int[,] tiles = null;
    public int[,] objects;
    public int tile;
    public int obj;
    bool placeObj = false;
    bool placetile = false;

    public int sizeX;
    public int sizeY;

    public Vector3[,] node;

    [HideInInspector]
    public Vector3 gridposition;

    [HideInInspector]
    public float z = 0;
    
    Rect[] button = new Rect[9]{
        new Rect(Screen.width * 0.005f, Screen.height * 0.005f, 100, 25),
        new Rect(Screen.width * 0.005f, (Screen.height * 0.005f) + 30, 100, 25),
        new Rect(Screen.width * 0.005f, (Screen.height * 0.005f) + 60, 100, 25),
        new Rect(Screen.width * 0.005f, (Screen.height * 0.005f) + 90, 100, 25),
        new Rect(Screen.width * 0.005f, (Screen.height * 0.005f) + 120, 100, 25),
        new Rect(Screen.width * 0.005f, (Screen.height * 0.005f) + 150, 100, 25),
        new Rect(Screen.width * 0.005f, (Screen.height * 0.005f) + 180, 100, 25),
        new Rect(Screen.width * 0.005f, (Screen.height * 0.005f) + 210, 100, 25),
        new Rect(Screen.width * 0.005f, (Screen.height * 0.005f) + 240, 100, 25)
    };
    #endregion

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 9))
            {
                if (hit.collider.gameObject.name == "Hex")
                {
                    if (placetile == true)
                    {
                        tiles[hit.collider.gameObject.GetComponent<ClickableTile_Editor>().tileX,
                              hit.collider.gameObject.GetComponent<ClickableTile_Editor>().tileY] = tile;
                        TileType_Editor tt = tileTypes[tiles[hit.collider.gameObject.GetComponent<ClickableTile_Editor>().tileX,
                                                             hit.collider.gameObject.GetComponent<ClickableTile_Editor>().tileY]];
                        hit.collider.gameObject.GetComponent<MeshRenderer>().material = tt.hex_Material;
                    }

                    else if (placeObj == true)
                    {
                        objects[hit.collider.gameObject.GetComponent<ClickableTile_Editor>().tileX,
                                hit.collider.gameObject.GetComponent<ClickableTile_Editor>().tileY] = obj;
                        ObjectType_Editor ot = objectTypes[objects[hit.collider.gameObject.GetComponent<ClickableTile_Editor>().tileX,
                                                                 hit.collider.gameObject.GetComponent<ClickableTile_Editor>().tileY]];
                        Instantiate(ot.obj, TileCoordToWorldCoord(hit.collider.gameObject.GetComponent<ClickableTile_Editor>().tileX,
                                                                  hit.collider.gameObject.GetComponent<ClickableTile_Editor>().tileY), Quaternion.identity);
                    }
                }
            }
        }
    }

    public void OnGUI(){

        // TILE BUTTONS
        if(GUI.Button(button[0], "Grass")){
            tile = 0;
            placetile = true;
            placeObj = false;
        }
        if (GUI.Button(button[1], "Dirt")){
            tile = 1;
            placetile = true;
            placeObj = false;
        }
        if (GUI.Button(button[2], "Sand")){
            tile = 2;
            placetile = true;
            placeObj = false;
        }
        if (GUI.Button(button[3], "Mountain")){
            tile = 3;
            placetile = true;
            placeObj = false;
        }
        //OBJECT BUTTONS
        if (GUI.Button(new Rect(Screen.width * 0.005f + 105, (Screen.height * 0.005f), 100, 25), "Remove"))
        {
            obj = 0;
            placetile = false;
            placeObj = true;
        }
        if (GUI.Button(new Rect(Screen.width * 0.005f + 105, (Screen.height * 0.005f) + 30, 100, 25), "Small Rock"))
        {
            obj = 1;
            placetile = false;
            placeObj = true;
        }
        if (GUI.Button(new Rect(Screen.width * 0.005f + 105, (Screen.height * 0.005f) + 60, 100, 25), "Large Rock"))
        {
            obj = 2;
            placetile = false;
            placeObj = true;
        }
        if (GUI.Button(new Rect(Screen.width * 0.005f + 105, (Screen.height * 0.005f) + 90, 100, 25), "Tree"))
        {
            obj = 3;
            placetile = false;
            placeObj = true;
        }
        //FILE OPERATION BUTTONS
        if (GUI.Button(new Rect(Screen.width * 0.9f, (Screen.height * 0.005f + 30), 100, 25), "Load"))
        {
            clear();
            tiles = Load("C:\\Users\\Jakob\\Documents\\Unity\\Hexagon\\Assets\\Saves\\savetile1.csv", sizeX, sizeY);
            objects = Load("C:\\Users\\Jakob\\Documents\\Unity\\Hexagon\\Assets\\Saves\\saveobj1.csv", sizeX, sizeY);
            genMap();
        }
        if (GUI.Button(new Rect(Screen.width * 0.9f, (Screen.height * 0.005f + 60), 100, 25), "Save"))
        {
            Save(tiles, "C:\\Users\\Jakob\\Documents\\Unity\\Hexagon\\Assets\\Saves\\savetile1.csv", sizeX, sizeY);
            Save(objects, "C:\\Users\\Jakob\\Documents\\Unity\\Hexagon\\Assets\\Saves\\saveobj1.csv", sizeX, sizeY);
        }
        if (GUI.Button(new Rect(Screen.width * 0.9f, (Screen.height * 0.005f), 100, 25), "New"))
        {
            clear();
            New(sizeX, sizeY);
            genMap();
        }
    }

    void Start()
    {
        genMapData();
    }

    public void genMapData()
    {
        tiles = new int[sizeX, sizeY];
        objects = new int[sizeX, sizeY];
        node = createNode(sizeX, sizeY);
        gridposition.z = 0;
    }

    public void genMap()
    {
        for (int i = 0; i < sizeX; i++)
        {
            z = i;
            if (IsEven(i))
                genHexeven(z, i);
            else
                genHexodd(z, i);
        }
    }

    public void genHexeven(float y, int i)
    {
        for (int j = 0; j < sizeY; j++)
        {
            gridposition.x = j;
            gridposition.z = z * 0.866f;
            TileType_Editor tt = tileTypes[tiles[i, j]];
            _hex.hexMesh(gridposition, tt.hex_Material);

            if (objects[i, j] != 0)
            {
                ObjectType_Editor ot = objectTypes[objects[i, j]];
                Instantiate(ot.obj, gridposition, Quaternion.identity);
            }
            else { }

            ClickableTile_Editor ct = _hex.tileHex.GetComponent<ClickableTile_Editor>();
            ct.tileX = i;
            ct.tileY = j;
            ct.map = this;
            node[i, j] = gridposition;
        }
    }

    public void genHexodd(float y, int i)
    {
        for (int l = 0; l < sizeY; l++)
        {
            gridposition.x = l + 0.5f;
            gridposition.z = z * 0.866f;
            TileType_Editor tt = tileTypes[tiles[i, l]];
            _hex.hexMesh(gridposition, tt.hex_Material);

            if (objects[i, l] != 0){
               ObjectType_Editor ot = objectTypes[objects[i, l]];
            Instantiate(ot.obj, gridposition, Quaternion.identity); 
            }
            else { }

            ClickableTile_Editor ct = _hex.tileHex.GetComponent<ClickableTile_Editor>();
            ct.tileX = i;
            ct.tileY = l;
            ct.map = this;
            node[i, l] = gridposition;
        }
    }

    public static bool IsEven(int value)
    {
        return value % 2 == 0;
    }

    public static Vector3[,] createNode(int sizeX, int sizeY)
    {
        return new Vector3[sizeX, sizeY];
    }

    public void clear(){
        GameObject[] hexs = GameObject.FindGameObjectsWithTag("Hex");
        foreach (GameObject hex in hexs)
        {
            Destroy(hex);
        }
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Objects");
        if (objs != null)
        {
            foreach (GameObject obj in objs)
            {
                Destroy(obj);
            }
        }
    }

    public void New(int sizeX, int sizeY){
        int[,] newMap = new int[sizeX,sizeY];
        for (int x = 0; x < sizeX; x++){
            for (int y = 0; y < sizeY; y++){
                objects[x, y] = 0;
                tiles[x, y] = 0;
            }
        }
        //return newMap;
    }

    public static void Save(int[,] temp, string path, int sizeX, int sizeY)
    {
        using (StreamWriter file = new StreamWriter(path))
        {
            for (int x = 0; x < sizeX; x++){
                for (int y = 0; y < sizeY; y++){
                    file.Write(temp[x, y] + ",");
                }
                //file.Write("\n");
            }
            file.Close();
        }
    }

    public int[,] Load(string path, int sizeX, int sizeY)
    {
    List<int> parsedData = new List<int>();

        using (StreamReader readFile = new StreamReader(path)){
            string line;
            string[] row;
            int length;
            int[,] loaded = new int[sizeX, sizeY];
            
            while ((line = readFile.ReadLine()) != null){
                row = line.Split(',');
                length = row.Length;
                int[] temp = new int[length];
                int y = 0;
                int x = 0;
                for (int l = 0; l < length - 1; l++){
                    if (y == sizeY){
                        y = 0;
                        x++;
                    }
                    temp[l] = Convert.ToInt16(row[l]);
                    loaded[x, y] = temp[l];
                    y++;
                }
            }return loaded;
        }   
    }

    public Vector3 TileCoordToWorldCoord(int x, int y)
    {
        return node[x, y];
    }
}