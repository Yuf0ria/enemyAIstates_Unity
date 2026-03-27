// ============================================================
//  VFXManager.cs  –  Simple flash effects, no prefabs needed
//  Attach to any persistent GameObject in your scene
// ============================================================
using System.Collections;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    // Spawns a tiny sphere that shrinks and disappears
    public void FlashAt(Vector3 pos, Color color)
    {
        StartCoroutine(DoFlash(pos, color));
    }

    IEnumerator DoFlash(Vector3 pos, Color color)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Destroy(go.GetComponent<Collider>());
        go.transform.position   = pos + Vector3.up * 0.5f;
        go.transform.localScale = Vector3.one * 0.4f;
        go.GetComponent<Renderer>().material.color = color;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 4f;
            go.transform.localScale = Vector3.one * Mathf.Lerp(0.4f, 0f, t);
            yield return null;
        }
        Destroy(go);
    }
}
