// ============================================================
//  HealthBar.cs  –  World-space bar that faces the camera
//  Attach to a child GameObject on the unit prefab
// ============================================================
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("Assign in Prefab")]
    public Image fillImage;   // the colored fill
    public Image bgImage;     // dark background

    private float maxHp;
    private Camera mainCam;

    void Awake() => mainCam = Camera.main;

    void LateUpdate()
    {
        // Always face camera
        if (mainCam != null)
            transform.LookAt(transform.position + mainCam.transform.forward);
    }

    public void Initialize(float max, string label)
    {
        maxHp = max;
        UpdateHealth(max);
    }

    public void UpdateHealth(float current)
    {
        if (fillImage == null) return;
        float pct = Mathf.Clamp01(current / maxHp);
        fillImage.fillAmount = pct;
        // Green → Yellow → Red
        fillImage.color = Color.Lerp(Color.red, Color.green, pct);
    }
}
