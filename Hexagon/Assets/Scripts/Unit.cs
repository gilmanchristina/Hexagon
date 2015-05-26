using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Unit : MonoBehaviour {

    public int tileX;
    public int tileY;
    public TileMap map;
    public bool selectedUnit;

    public List<Node> currentPath = null;

    int moveSpeed = 2;
    float remainingMovement = 2;

    void Update(){
            if (currentPath != null){
                int currNode = 0;

                while (currNode < currentPath.Count - 1){
                    Vector3 start = map.TileCoordToWorldCoord(currentPath[currNode].x, currentPath[currNode].y) + new Vector3(0, 0.1f, 0);
                    Vector3 end = map.TileCoordToWorldCoord(currentPath[currNode + 1].x, currentPath[currNode + 1].y) + new Vector3(0, 0.1f, 0);

                    Debug.DrawLine(start, end, Color.red);

                    currNode++;
                }
            }
            if (Vector3.Distance(transform.position, map.TileCoordToWorldCoord(tileX, tileY)) < 0.1f)
                AdvancePathing();

            // Smoothly animate towards the correct map tile.
            transform.position = Vector3.Lerp(transform.position, map.TileCoordToWorldCoord(tileX, tileY), 5f * Time.deltaTime);
    }

    void AdvancePathing(){
        if (currentPath == null)
            return;

        if (remainingMovement <= 0)
            return;

        transform.position = map.TileCoordToWorldCoord(tileX, tileY);

        remainingMovement -= map.CostToEnterTile(currentPath[1].x, currentPath[1].y);

        tileX = currentPath[1].x;
        tileY = currentPath[1].y;

        currentPath.RemoveAt(0);

        if (currentPath.Count == 1){
            currentPath = null;
        }
    }

    public void NextTurn(){
        // Make sure to wrap-up any outstanding movement left over.
        while (currentPath != null && remainingMovement > 0)
        {
            AdvancePathing();
        }

        // Reset our available movement points.
        remainingMovement = moveSpeed;
    }
}
