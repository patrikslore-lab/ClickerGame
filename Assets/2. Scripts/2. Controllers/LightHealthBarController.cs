using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class LightHealthBarController : MonoBehaviour
{
    PlayerConfig playerConfig;
    private float maxLightHealth;
    private float currentLightHealth;
    private Image lightHealthBarImage;

    private float lightDamage;
    private float lightAdd;

    void Start()
    {
        playerConfig = GameManager.Instance.GetPlayerConfig();
        maxLightHealth = playerConfig.lightHealthMax;
        currentLightHealth = playerConfig.lightHealthCurrent;
        lightHealthBarImage = GetComponent<Image>();
        lightHealthBarImage.type = Image.Type.Filled;
        lightHealthBarImage.fillMethod = Image.FillMethod.Horizontal;
        lightHealthBarImage.fillOrigin = (int)Image.OriginHorizontal.Left;
    }

    void Update()
    {
        float fillAmount = playerConfig.lightHealthCurrent / playerConfig.lightHealthMax;
        lightHealthBarImage.fillAmount = fillAmount;
    }
}

