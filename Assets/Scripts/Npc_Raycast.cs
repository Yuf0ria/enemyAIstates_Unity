using UnityEngine;

public class Npc_Raycast : MonoBehaviour
{
    //serializefields
    float distances = 10;
    [SerializeField] LayerMask playerHit; //playerOnSight
    [SerializeField] Color color;

    void Update()
    {
        Interact();
    }

    public void Interact()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, distances, playerHit))
        {
            Debug.Log($"Object name {hitInfo.transform.name}");
            Debug.DrawLine(ray.origin, hitInfo.point, Color.red);
            //follow
        }
        else
        {
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * distances, Color.blue);
            //wait or stop
        }
    }
}
