#region About Script Definitions
    //CustomSplineRender for NavMeshLink
    //Refences:
    //https://docs.unity3d.com/Packages/com.unity.ai.navigation@2.0/manual/NavMeshLink.html
#endregion
using UnityEngine; 

public class Spline : MonoBehaviour
{
    [SerializeField] private Transform _start, _middle, _end;
    [SerializeField] private bool showGizmos = true;

    private Vector3 CalculatePoints(float value01, Vector3 startPos, Vector3 endPos, Vector3 midPos)
    {
        value01 = Mathf.Clamp01(value01);
        Vector3 startMiddle = Vector3.Lerp(startPos, midPos, value01);
        Vector3 middleEnd = Vector3.Lerp(midPos, endPos, value01);
        return Vector3.Lerp(startMiddle, middleEnd, value01);
    }
    public Vector3 CalculatePosition(float interpolationAmount01)
        => CalculatePoints(interpolationAmount01, _start.position, _end.position, _middle.position);
    
    //calculates the fluent movement when jumping, referenced sunny valley studio here
    #region CustomCalculations
        public Vector3 CalculatePositonStart(float interpolationAmount01, Vector3 startPos) => CalculatePoints(interpolationAmount01, startPos, _end.position, _middle.position);
        
        public Vector3 CalculatePositionEnd(float interpolationAmount01, Vector3 endPos) => CalculatePoints(interpolationAmount01, _start.position, endPos,  _middle.position);
    #endregion

    public void SetPoints(Vector3 startPos, Vector3 midPos, Vector3 endPos)
    {
        if (_start != null && _middle != null && _end != null)
        {
            _start.position = startPos;
            _middle.position = midPos;
            _end.position = endPos;
        }
    }

    private void OnDrawGizmos()
    {
        if (showGizmos && _start != null
            && _middle != null
            && _end != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(_start.position, 0.1f);
            Gizmos.DrawSphere(_middle.position, 0.1f);
            Gizmos.DrawSphere(_end.position, 0.1f);
            Gizmos.color = Color.red;
            int granularity = 5;
            for (int i = 0; i < granularity; i++)
            {
                Vector3 startPoint = i == 0 ? _start.position : 
                    CalculatePosition(i/ (float)granularity);
                Vector3 endPoint = i == granularity - 1
                    ? _end.position : 
                    CalculatePosition((i + 1) / (float)granularity);  
                Gizmos.DrawLine(startPoint, endPoint);

            }
        }
            
    }

}
