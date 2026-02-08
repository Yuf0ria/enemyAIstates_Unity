using UnityEngine;

public class OverlapRadius : MonoBehaviour
{
    float radius = 4,
            interval = 0f;

    private void Update() {
        interval += Time.deltaTime;
        if (interval >= 2f )
        {
            RadObject(transform.position, radius);
            interval = 0f;
        }
    }
    void RadObject(Vector3 center, float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("object"))
            {
                Debug.Log(hitCollider.gameObject.name);
            }
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
