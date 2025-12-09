using System;
using UnityEngine;

public class UpgradeController : MonoBehaviour
{
    PlayerConfig playerConfig;

    private void Start()
    {
        playerConfig = GameManager.Instance.GetPlayerConfig();
    }
    public void PayDoggy(float doggyCost)
    {
         playerConfig.wood -= doggyCost;
    }

    public void PayRicochetPrice(float ricochetPrice)
    {
        playerConfig.wood -= ricochetPrice;
    }
}
