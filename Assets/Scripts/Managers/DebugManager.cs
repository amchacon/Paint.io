using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : SingletonMB<DebugManager>
{
    public enum Features
    {
        NONE,
        SHOP,
        DAILY_REWARD,
        COLLISION,
        ALL
    }

    public bool AllFeatures;
    public bool ShopFeature;
    public bool DailyRewardFeature;
    public bool CollisionFeature;

    public void EnableFeature(Features feature)
    {
        switch (feature)
        {
            case Features.NONE:
                AllFeatures = false;
                break;
            case Features.ALL:
                AllFeatures = true;
                break;
            case Features.SHOP:
                ShopFeature = true;
                break;
            case Features.DAILY_REWARD:
                DailyRewardFeature = true;
                break;
            case Features.COLLISION:
                CollisionFeature = true;
                break;
        }
    }
}
