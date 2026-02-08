using UnityEngine;

public class NpcPointsTp : MonoBehaviour
{
    #region enum
        public enum PathType
        {
            Loop,
            ReverseLoop //Enemy_npc goes back to the point
            //Randomize ; could figure this one out
        }
    #endregion
    #region public variables
        public Transform[] points; //Array for the points
        public PathType pathType = PathType.Loop; //checks paths
    #endregion
    #region private variables

        private int _direction = 1; //default = 1, Direction is A -> B; -1 ; B -> A
        private int _index;

    #endregion
    
    
    
    //currentPoint()
    public Vector3 CurrentPoint() //I'm checking vector3 cause I confused it for transform it works though, thx auto-fill :D
    {
        return points[_index].position;
    }
    
    //nextPoint()
    public Vector3 NextPoint()
    {
        if(points.Length == 0) return transform.position;

        _index = IndexingPoints(); //function loops then reverses it
        Vector3 point = points[_index].position;

        return point;
    }
    
    //IndexingPoints()
    private int IndexingPoints()
    {
        _index += _direction;
        // if(index >= points.Length) return points.Length - 1; auto-fill wrote this. delete this
        if (pathType == PathType.Loop)
        {
            _index %= points.Length; // if like 5/5 = 0 wait hows a divide again? ; UPDATE: it was %, YIPPEE
        } //BUT if
        if (pathType == PathType.ReverseLoop ||
            _index < 0) //for loop could work here but maybe too much? ; UPDATE: yeah, nvm
        {
            _direction += -1; // 0
            _index += _direction * 2;// I LOVEE MATH (sobs)
        }
        return _index;
    }
    
    //OnDrawGismoz()
    private void OnDrawGizmos()
    {
        if (points == null || points.Length == 0) return;
        Gizmos.color = Color.red;
        // Gizmos.DrawLine(transform.position, points[index].position);
        for (int i = 0; i < points.Length -1; i++)
        {
            Gizmos.DrawLine(points[i].position, points[i + 1].position);
        }
        if (pathType == PathType.Loop)
        {
            Gizmos.DrawLine(points[points.Length - 1].position, points[0].position);
        }
        Gizmos.color = Color.yellow;
        foreach (Transform point in points)
        {
            Gizmos.DrawSphere(point.position, 0.1f);
        }
    }
}
