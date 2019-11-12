using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinding : MonoBehaviour
{
    public int GridWidth;
    public int GridHeight;
    private Vector2 startPosition;
    private Vector2 endPosition;
    public int waypointAmount = 4;
    List<Vector2> waypoints = new List<Vector2>();
    float[] distances = new float[8];

    readonly Vector2[] direction = { new Vector2(-1, 1), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0), 
        new Vector2(1, -1), new Vector2(0, -1), new Vector2(-1, -1), new Vector2(-1, 0), };

    public AStarPathfinding(Vector2 startPosition, Vector2 endPosition)
    {
        this.startPosition = startPosition;
        this.endPosition = endPosition;
    }

    public void start()
    {
        PrintGrid();
        generateDistances(startPosition);
        generateNextWaypoint(startPosition);
    }

    public List<Vector2> getWaypoints()
    {
        return waypoints;
    }

    void generateNextWaypoint(Vector2 start)
    {
        Vector2 newStartPos;


    }

    void generateDistances(Vector2 start)
    {
        foreach (int i in distances)
        {
            distances[i] = (start - endPosition + direction[i]).magnitude;
        }
    }

    void PrintGrid()
    {
        int i, j;
        Vector2 loop;
        for (i = 0; i <= GridHeight - 1; i++)
        {
            for (j = 0; j <= GridWidth - 1; j++)
            {
                loop = new Vector2(i, j);
                if (startPosition.Equals(loop))
                {
                    Debug.Log('S');
                }
                else if (endPosition == (loop))
                {
                    Debug.Log('E');
                }
                else
                {
                    Debug.Log('-');
                }
                if (j != GridWidth - 1)
                {
                    Debug.Log('|');
                }
            }
            Debug.Log('\n');
        }
    }

    float Compare8(Vector2 start)
    {
        generateDistances(start);
        return Mathf.Min(Mathf.Min(Mathf.Min(distances[0], distances[1]), Mathf.Min(distances[2], distances[3])),
                        Mathf.Min(Mathf.Min(distances[4], distances[5]), Mathf.Min(distances[6], distances[7])));
    }

    Vector2 GetNewCoors(int field, Vector2 start)
    {
        float X = 0;
        float Y = 0;
        for(int i = 0; i <= 8; i++)
        {
            if(field == i)
            {
                X = start.x + direction[i].x;
                Y = start.y + direction[i].y;
            }
        }
        Vector2 coors = new Vector2(X, Y);
        return coors;
    }

}