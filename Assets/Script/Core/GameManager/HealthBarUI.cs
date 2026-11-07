using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;

    public void UpdateHealth(int currentHealth, int maxHealth)
    {
        if (fillImage == null)
            return;

        float healthPercent = maxHealth > 0
            ? (float)currentHealth / maxHealth
            : 0f;

        fillImage.fillAmount = Mathf.Clamp01(healthPercent);
    }
}