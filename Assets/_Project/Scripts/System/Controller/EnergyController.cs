using System;
using System.Globalization;
using UnityEngine;

public class EnergyController : SingletonDontDestroy<EnergyController>
{
    [SerializeField] private EnergyConfig energyConfig;
    
    // Tracking online timer
    private float _timer;
    // Cache the max and refill time purely for shorter access
    private int MaxEnergy => energyConfig != null ? energyConfig.maxEnergy : 30;
    private int RefillTimeInSeconds => energyConfig != null ? energyConfig.refillTimeInSeconds : 300;

    protected override void Awake()
    {
        base.Awake();
        _timer = RefillTimeInSeconds;
    }

    private void Start()
    {
        CheckOfflineEnergy();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            CheckOfflineEnergy();
        }
    }

    private void CheckOfflineEnergy()
    {
        if (energyConfig == null || Data.PlayerData == null) return;

        int currentEnergy = Data.PlayerData.CurrentEnergy;
        if (currentEnergy >= MaxEnergy)
        {
            UpdateRefillPointToNow();
            _timer = RefillTimeInSeconds;
            return;
        }

        if (DateTime.TryParseExact(Data.PlayerData.RefillEnergyPoint, Utility.DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out DateTime lastPoint))
        {
            TimeSpan diff = DateTime.UtcNow - lastPoint;
            if (diff.TotalSeconds > 0)
            {
                int energyToAdd = (int)(diff.TotalSeconds / RefillTimeInSeconds);
                
                if (energyToAdd > 0)
                {
                    int newEnergy = Mathf.Min(currentEnergy + energyToAdd, MaxEnergy);
                    Data.PlayerData.CurrentEnergy = newEnergy;
                    
                    if (newEnergy >= MaxEnergy)
                    {
                         UpdateRefillPointToNow();
                         _timer = RefillTimeInSeconds;
                    }
                    else
                    {
                        // Calculate remainder and keep progress
                        int remainingSeconds = (int)(diff.TotalSeconds % RefillTimeInSeconds);
                        Data.PlayerData.RefillEnergyPoint = DateTime.UtcNow.AddSeconds(-remainingSeconds).ToString(Utility.DateTimeFormat, CultureInfo.InvariantCulture);
                        _timer = RefillTimeInSeconds - remainingSeconds;
                    }
                }
                else
                {
                    // No full energy added, just update timer visually based on passed time
                    _timer = RefillTimeInSeconds - (float)diff.TotalSeconds;
                }
            }
        }
        else
        {
            // Fallback if parsing fails
            UpdateRefillPointToNow();
            _timer = RefillTimeInSeconds;
        }
    }

    private void Update()
    {
        if (energyConfig == null || Data.PlayerData == null) return;

        if (Data.PlayerData.CurrentEnergy >= MaxEnergy)
        {
            if (_timer != RefillTimeInSeconds)
            {
                _timer = RefillTimeInSeconds;
                UpdateRefillPointToNow();
            }
            return;
        }

        _timer -= Time.deltaTime;
        
        // Frequently update the RefillEnergyPoint to the exact current time (minus whatever progress we made in the current timer)
        // so that if the app is abruptly closed, the PlayerDataController saves the correct timestamp.
        float elapsedInCurrentInterval = RefillTimeInSeconds - _timer;
        Data.PlayerData.RefillEnergyPoint = DateTime.UtcNow.AddSeconds(-elapsedInCurrentInterval).ToString(Utility.DateTimeFormat, CultureInfo.InvariantCulture);

        if (_timer <= 0)
        {
            Data.PlayerData.CurrentEnergy++;
            _timer = RefillTimeInSeconds;
            UpdateRefillPointToNow();
        }
    }

    private void UpdateRefillPointToNow()
    {
        if (Data.PlayerData != null)
        {
            Data.PlayerData.RefillEnergyPoint = DateTime.UtcNow.ToString(Utility.DateTimeFormat, CultureInfo.InvariantCulture);
        }
    }

    public string GetEnergyString()
    {
        if (Data.PlayerData == null) return "0/0";
        return $"{Data.PlayerData.CurrentEnergy}/{MaxEnergy}";
    }
}
