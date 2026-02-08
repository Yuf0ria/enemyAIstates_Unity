using System.IO;
using UnityEngine;
using UnityEngine.AI;


public class NPCPointsTP : MonoBehaviour
{
    public enum PathType
    {
        Loop,
        ReverseLoop //Enemy_npc goes back to the point
        //Randomize
    }
    
    public Transform[] points; //Array for the points
    public PathType pathType = PathType.Loop; //checks paths
    
    private int direction = 1; //default = 1, Direction is A -> B; -1 ; B -> A
    private int index;

    public Vector3 currentPoint()
    {
        return points[index].position;
    }
    
    public Vector3 nextPoint()
    {
        if(points.Length == 0) return transform.position;

        index = pointsIndex(); //function loops then reverses it
        Vector3 point = points[index].position;

        return point;
    }
    
    priavte int pointsIndex()
    {
        index += direction;
        // if(index >= points.Length) return points.Length - 1; not initialized
        if (pathType == PathType.Loop)
        {
            index %= points.Length; // if like 5/5 = 0 wait hows a divide again? ; UPDATE: it was %, YIPPEE
        }
        if (pathType == PathType.ReverseLoop ||
            index < 0) //for loop could work here but maybe too much? ; UPDATE: yeah, nvm
        {
            direction += -1; // 0
            index += direction * 2;// I LOVEE MATH (sobs)
        }
        return index;
    }
}
