using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinding
{
    public int GridWidth;
    public int GridHeight;
    static Vector2 startPosition;
    static private Vector2 endPosition;
    List<Vector2> waypoints = new List<Vector2>();
    static Vector2 currentFieldComparator;

    public AStarPathfinding(Vector2 startPosition, Vector2 endPosition, int gridWidth, int gridHeight)
    {
        AStarPathfinding.startPosition = startPosition;
        AStarPathfinding.endPosition = endPosition;
        GridWidth = gridWidth;
        GridHeight = gridHeight;
    }

    public void start()
    {
        Debug.Log("start()");
        PrintGrid();
        generateNextWaypoint(startPosition);
    }

    public List<Vector2> getWaypoints()
    {
        generateNextWaypoint(startPosition);
        return waypoints;
    }

    List<Vector2> generateNextWaypoint(Vector2 currentField)
    {
        currentFieldComparator = currentField;
        Vector2 nextField = findNextField(currentField);
        if (nextField != endPosition)
        {
            waypoints.Add(nextField);
            generateNextWaypoint(nextField);
        }
        return waypoints;
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

    Vector2 findNextField(Vector2 currentField)
    {
        Vector2 nextField;
        List<Vector2> possibleFields = new List<Vector2>();
        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                if (((currentField.x + x) >= 0) && ((currentField.y + y) >= 0) && ((currentField.x + x) <= GridWidth) && ((currentField.y + y) <= GridHeight))
                {
                    possibleFields.Add(new Vector2(x + currentField.x, y + currentField.y));
                }
            }
        }

        possibleFields.Sort(new PossibleFieldsComparator());
        nextField = possibleFields[0];
        
        return nextField;
    }


    class PossibleFieldsComparator : IComparer<Vector2>
    {
        public int Compare(Vector2 x, Vector2 y)
        {
            float distx = (AStarPathfinding.endPosition - x).magnitude;
            float disty = (AStarPathfinding.endPosition - y).magnitude;

            if (distx < disty)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
    }
}