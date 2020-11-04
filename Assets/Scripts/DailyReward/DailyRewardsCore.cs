using System;
using UnityEngine;

public abstract class DailyRewardsCore : SingletonMB<DailyRewardsCore>
{
    public DateTime now;    // The actual date. Either returned by the using the world clock or the player device clock

    public delegate void OnInitialize();
    public OnInitialize onInitialize;

    public bool isInitialized = false;

    // Initializes the current DateTime. If the player is using the World Clock initializes it
    public void InitializeDate()
    {
	    now = DateTime.Now;
	    isInitialized = true;
    }

    public void RefreshTime ()
    {
        now = DateTime.Now;
    }

    public virtual void TickTime()
    {
        if (!isInitialized)
			return;

        now = now.AddSeconds(Time.unscaledDeltaTime);
    }

    public string GetFormattedTime (TimeSpan span)
    {
        return string.Format("{0:D2}:{1:D2}:{2:D2}", span.Hours, span.Minutes, span.Seconds);
    }

    protected virtual void Awake(){}

    protected virtual void OnApplicationPause(bool pauseStatus)
    {
        if(!pauseStatus)
        {
            RefreshTime();
        }                
    }
}