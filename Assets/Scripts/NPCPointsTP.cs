using System;
using System.IO;
using UnityEngine;
using UnityEngine.AI;


public class NPCPointsTP : MonoBehaviour
{
    public enum PathType
    {
        Loop,
        ReverseLoop //Enemy_npc goes back to the point
        //Randomize ; could figure this one out
    }
    
    public Transform[] points; //Array for the points
    public PathType pathType = PathType.Loop; //checks paths
    
    private int direction = 1; //default = 1, Direction is A -> B; -1 ; B -> A
    private int index;
    
    //currentPoint()
    public Vector3 currentPoint() //I'm checking vector3 cause I confused it for transform it works though, thx auto-fill :D
    {
        return points[index].position;
    }
    
    //nextPoint()
    public Vector3 nextPoint()
    {
        if(points.Length == 0) return transform.position;

        index = pointsIndex(); //function loops then reverses it
        Vector3 point = points[index].position;

        return point;
    }
    
    //pointsIndex()
    private int pointsIndex()
    {
        index += direction;
        // if(index >= points.Length) return points.Length - 1; auto-fill wrote this. delete this
        if (pathType == PathType.Loop)
        {
            index %= points.Length; // if like 5/5 = 0 wait hows a divide again? ; UPDATE: it was %, YIPPEE
        } //BUT if
        if (pathType == PathType.ReverseLoop ||
            index < 0) //for loop could work here but maybe too much? ; UPDATE: yeah, nvm
        {
            direction += -1; // 0
            index += direction * 2;// I LOVEE MATH (sobs)
        }
        return index;
    }
    
    //OnDrawGismoz()
    private void OnDrawGizmos()
    {
        if (points == null || points.Length == 0) return;
        Gizmos.color = Color.red;
        // Gizmos.DrawLine(transform.position, points[index].position);
        for (int i = 0; i < points.Length; i++)
        {
            Gizmos.DrawLine(points[i].position, nextPoint());
        }
        if (pathType == PathType.Loop)
        {
            Gizmos.DrawLine(transform.position, nextPoint());
        }

        Gizmos.color = Color.yellow;
    }
}
