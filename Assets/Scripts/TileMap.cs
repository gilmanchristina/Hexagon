using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TileMap : MonoBehaviour {
    
    #region fields
    public TileType[] tileTypes;
    public ObjectType[] objectTypes;
    public Hex _hex = new Hex();
    public Board_Editor be = new Board_Editor();
    public FogofWar fogofwar;
    public GameObject selectedUnit;

    public int[,] tiles = null;
    public int[,] objects;
    public int tile;
    public int obj;
    Node[,] graph;

    public int sizeX;
    public int sizeY;
    public RaycastHit hit;

    public Vector3[,] node;

    [HideInInspector]
    public Vector3 gridposition;
    [HideInInspector]
    public Vector3 gridsize = Vector3.zero;

    [HideInInspector]
    public float z = 0;
    [HideInInspector]
    public float size = 0.4330127f;
    #endregion

	void Start () {
        selectedUnit.GetComponent<Unit>().tileX = (int)selectedUnit.transform.position.x;
        selectedUnit.GetComponent<Unit>().tileX = (int)selectedUnit.transform.position.y;
        selectedUnit.GetComponent<Unit>().map = this;

        genMapData();
        GenPathfindingGraph();
        genMap();
	}
    public void Update(){
        
        if (Input.GetMouseButtonDown(0)){
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast (ray, out hit, Mathf.Infinity)){
                if (hit.collider.gameObject.name == "Unit"){
                    hit.collider.GetComponent<Unit>().selectedUnit = true;
                    Debug.Log("Unit Selected = " + selectedUnit);
                }
            }
            //if (hit.collider.gameObject.name != "Unit")
            //{
            //    hit.collider.GetComponent<Unit>().selectedUnit = false;
            //}
        }  
    }

    public void FixedUpdate() {
        fogofwar.isVisible();
    }

    //public void OnGUI()
    //{
    //    if(GUI.Button(new Rect(100,100,100,25), "Check")){
    //        fogofwar.isVisible();
    //    }

    //}

    public void genMapData(){
        tiles = new int[sizeX, sizeY];
        node = createNode(sizeX, sizeY);
        gridposition.z = 0;
    }

    public void genMap(){
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                tiles[x, y] = 0;
            }
        }
        tiles[4, 4] = 2; tiles[5, 4] = 2; tiles[6, 4] = 2; tiles[7, 4] = 2; tiles[8, 4] = 2; tiles[4, 5] = 2; tiles[4, 6] = 2; tiles[8, 5] = 2; tiles[8, 6] = 2;

        for (int a = 8; a < 11; a++)
        {
            for (int b = 8; b < 11; b++)
            {
                tiles[a, b] = 1;
            }
        }


        for (int i = 0; i < sizeX; i++){
            z = i;
            if (IsEven(i))
                genHexeven(z, i);
            else
                genHexodd(z, i);
        }
    }

    public void genHexeven(float y, int i){
        for (int j = 0; j < sizeY; j++){
            gridposition.x = j;
            gridposition.z = z * 0.866f;
            TileType tt = tileTypes[tiles[i,j]];
            _hex.hexMesh(gridposition, tt.hex_Material);

            //if (objects[i, j] != 0)
            //{
            //    ObjectType ot = objectTypes[objects[i, j]];
            //    Instantiate(ot.obj, gridposition, Quaternion.identity);
            //}
            //else { }

            ClickableTile ct = _hex.tileHex.GetComponent<ClickableTile>();
            ct.tileX = i;
            ct.tileY = j;
            ct.map = this;
            node[i, j] = gridposition;
        }
    }

    public void genHexodd(float y, int i){
        for (int l = 0; l < sizeY; l++){
            gridposition.x = l + 0.5f;
            gridposition.z = z * 0.866f;
            TileType tt = tileTypes[tiles[i, l]];
            _hex.hexMesh(gridposition, tt.hex_Material);

            //if (objects[i, l] != 0)
            //{
            //    ObjectType ot = objectTypes[objects[i, l]];
            //    Instantiate(ot.obj, gridposition, Quaternion.identity);
            //}
            //else { }

            ClickableTile ct = _hex.tileHex.GetComponent<ClickableTile>();
            ct.tileX = i;
            ct.tileY = l;
            ct.map = this;
            node[i, l] = gridposition;
        }
    }

    public static bool IsEven(int value){
        return value % 2 == 0;
    }

    public bool UnitCanEnterTile(int x, int y){
        return tileTypes[tiles[x,y]].is_Walkable;
    }
    
    public void GeneratePathTo(int x, int y){
        selectedUnit.GetComponent<Unit>().currentPath = null;

        if (UnitCanEnterTile(x, y) == false){
            return;
        }

        Dictionary<Node, float> dist = new Dictionary<Node, float>();
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();

        List<Node> unvisited = new List<Node>();    //Setup the Q list
        Node source = graph[selectedUnit.GetComponent<Unit>().tileX, selectedUnit.GetComponent<Unit>().tileY];
        Node target = graph[x, y];

        dist[source] = 0;
        prev[source] = null;

        foreach (Node v in graph){   //Initalize everything to have Infinity distance
            if (v != source){
                dist[v] = Mathf.Infinity;
                prev[v] = null;
            }

            unvisited.Add(v);
        }
        while (unvisited.Count > 0){
            Node u = null;

            foreach (Node possblieU in unvisited){
                if (u == null || dist[possblieU] < dist[u]){
                    u = possblieU;
                }
            }
            if (u == target){
                break;
            }
            unvisited.Remove(u);

            foreach(Node v in u.neighbours){
                //float alt = dist[u] + u.DistanceTo(v);
                float alt = dist[u] + CostToEnterTile(v.x,v.y);
                if (alt < dist[v]){
                    dist[v] = alt;
                    prev[v] = u;
                }
            }
        }
        if (dist[target] == null){
            //No route from source to target
            return;
        }
        List<Node> currentPath = new List<Node>();
        Node curr = target;

        while (curr != null){
            currentPath.Add(curr);
            curr = prev[curr];
        }
        currentPath.Reverse();

        selectedUnit.GetComponent<Unit>().currentPath = currentPath;
    }

    public float CostToEnterTile(int x, int y){
        TileType tt = tileTypes[tiles[x, y]];

        if (UnitCanEnterTile(x, y) == false)
        {
            return Mathf.Infinity;
        }
        return tt.movementCost;
    }

    public void GenPathfindingGraph(){
        graph = new Node[sizeX,sizeY];

        for (int x = 0; x < sizeX; x++){        //creation of Node graph
            for (int y = 0; y < sizeY; y++){
                graph[x, y] = new Node();
                graph[x, y].x = x;
                graph[x, y].y = y;
            }
        }
        for (int x = 0; x < sizeX; x++){        //calculation of neighbours
            for (int y = 0; y < sizeY; y++){
                if (IsEven(x)){   //for a even row
                    if (x == 0 && y == 0){
                        graph[x, y].neighbours.Add(graph[x + 1, y]);      //Upper Right
                        graph[x, y].neighbours.Add(graph[x, y + 1]);      //Right
                    }
                    else if (x == 0 && y == sizeY - 1){
                        graph[x, y].neighbours.Add(graph[x + 1, y - 1]);  //Upper Left
                        graph[x, y].neighbours.Add(graph[x, y - 1]);      //Left
                    }
                    else if (x == 0 && y != 0 && y != sizeY - 1){
                        graph[x, y].neighbours.Add(graph[x + 1, y]);      //Upper Right
                        graph[x, y].neighbours.Add(graph[x, y + 1]);      //Right
                        graph[x, y].neighbours.Add(graph[x + 1, y - 1]);  //Upper Left
                        graph[x, y].neighbours.Add(graph[x, y - 1]);      //Left
                    }
                    else if (y == 0 && x != 0){
                        graph[x, y].neighbours.Add(graph[x + 1, y]);      //Upper Right
                        graph[x, y].neighbours.Add(graph[x, y + 1]);      //Right
                        graph[x, y].neighbours.Add(graph[x - 1, y]);      //Lower Right
                    }
                    else if (y == sizeY - 1 && x != sizeX - 1){
                        graph[x, y].neighbours.Add(graph[x + 1, y - 1]);  //Upper Left
                        graph[x, y].neighbours.Add(graph[x, y - 1]);      //Left
                        graph[x, y].neighbours.Add(graph[x - 1, y - 1]);  //Lower Left
                    }
                    else{
                        graph[x, y].neighbours.Add(graph[x + 1, y]);      //Upper Right
                        graph[x, y].neighbours.Add(graph[x, y + 1]);      //Right
                        graph[x, y].neighbours.Add(graph[x - 1, y]);      //Lower Right
                        graph[x, y].neighbours.Add(graph[x + 1, y - 1]);  //Upper Left
                        graph[x, y].neighbours.Add(graph[x, y - 1]);      //Left
                        graph[x, y].neighbours.Add(graph[x - 1, y - 1]);  //Lower Left
                    }
                }
                else{                                                     //Odd Row
                    if (y == 0 && x != sizeX - 1){
                        graph[x, y].neighbours.Add(graph[x + 1, y + 1]);  //Upper Right
                        graph[x, y].neighbours.Add(graph[x, y + 1]);      //Right
                        graph[x, y].neighbours.Add(graph[x - 1, y + 1]);  //Lower Right
                        graph[x, y].neighbours.Add(graph[x + 1, y]);      //Upper Left
                        graph[x, y].neighbours.Add(graph[x - 1, y]);      //Lower Left
                    }
                    else if (y == sizeY - 1 && x != sizeX - 1){
                        graph[x, y].neighbours.Add(graph[x + 1, y]);      //Upper Left
                        graph[x, y].neighbours.Add(graph[x, y - 1]);      //Left
                        graph[x, y].neighbours.Add(graph[x - 1, y]);      //Lower Left
                    }
                    else if (x == sizeX - 1 && y == sizeY - 1){
                        graph[x, y].neighbours.Add(graph[x, y - 1]);      //Left
                        graph[x, y].neighbours.Add(graph[x - 1, y]);      //Lower Left
                    }
                    else if (x == sizeX - 1 && y == 0){
                        graph[x, y].neighbours.Add(graph[x, y + 1]);      //Right
                        graph[x, y].neighbours.Add(graph[x - 1, y + 1]);  //Lower Right
                    }
                    else if (x == sizeX - 1 && y != sizeY - 1 && y != 0){
                        graph[x, y].neighbours.Add(graph[x, y + 1]);      //Right
                        graph[x, y].neighbours.Add(graph[x - 1, y + 1]);      //Lower Right
                        graph[x, y].neighbours.Add(graph[x, y - 1]);      //Left
                        graph[x, y].neighbours.Add(graph[x - 1, y]);  //Lower Left
                    }
                    else if (y == sizeY - 1 && x == sizeX - 1){
                        graph[x, y].neighbours.Add(graph[x - 1, y + 1]);  //Lower Right
                        graph[x, y].neighbours.Add(graph[x, y - 1]);      //Left
                        graph[x, y].neighbours.Add(graph[x - 1, y]);  //Lower Left
                    }
                    else{
                        graph[x, y].neighbours.Add(graph[x + 1, y + 1]);  //Upper Right
                        graph[x, y].neighbours.Add(graph[x, y + 1]);      //Right
                        graph[x, y].neighbours.Add(graph[x - 1, y + 1]);  //Lower Right
                        graph[x, y].neighbours.Add(graph[x + 1, y]);      //Upper Left
                        graph[x, y].neighbours.Add(graph[x, y - 1]);      //Left
                        graph[x, y].neighbours.Add(graph[x - 1, y]);      //Lower Left
                    }
                }
            }
        }
    }

    public Vector3 TileCoordToWorldCoord(int x, int y){
        return node[x,y];
    }

    public static Vector3[,] createNode(int sizeX, int sizeY)
    {
        return new Vector3[sizeX, sizeY];
    }

    public void clear()
    {
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
}