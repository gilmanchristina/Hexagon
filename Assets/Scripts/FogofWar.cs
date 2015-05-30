using UnityEngine;
using System.Collections;
using System;

public class FogofWar : MonoBehaviour {

    public int sight;
    public double dist;
    Vector2 start = Vector2.zero;
    Vector2 end = Vector2.zero;
    
    public void isVisible()
    {
        GameObject[] allys = GameObject.FindGameObjectsWithTag("Ally");
        GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");
        if (allys != null || enemys != null)
        {
            foreach (GameObject enemy in enemys)
            {
                start.x = enemy.GetComponent<Unit>().tileX;
                start.y = enemy.GetComponent<Unit>().tileY;
                foreach (GameObject ally in allys)
                {
                    end.x = ally.GetComponent<Unit>().tileX;
                    end.y = ally.GetComponent<Unit>().tileY;
                    sight = 4; // ally.GetComponent<Unit>().sightDistance;
                    dist = Distance(start, end);
                    //Debug.Log(start.x + " " + start.y);
                    //Debug.Log(end.x + " " + end.y);
                    //Debug.Log(dist);
                    if (dist > sight)
                    {
                        enemy.transform.GetChild(0).gameObject.layer = 10;
                    }
                    else
                    {
                        enemy.transform.GetChild(0).gameObject.layer = 8;
                    }
                }
            }
        }
    }

    public double Distance(Vector2 start, Vector2 end) {
            dist = Math.Sqrt((start.x - end.x) * (start.x - end.x) + (start.y - end.y) * (start.y - end.y));
        return dist;
    }
}