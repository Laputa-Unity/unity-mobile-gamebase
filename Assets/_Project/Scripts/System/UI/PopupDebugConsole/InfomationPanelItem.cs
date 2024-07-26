using System.Globalization;
using TMPro;
using UnityEngine;

public class InfomationPanelItem : ConsolePanelItem
{
    [SerializeField] private TextMeshProUGUI identificationText;
    [SerializeField] private TextMeshProUGUI versionText;
    [SerializeField] private TextMeshProUGUI languageText;
    [SerializeField] private TextMeshProUGUI platformText;
    
    [SerializeField] private TextMeshProUGUI operatingSystemText;
    [SerializeField] private TextMeshProUGUI deviceModelText;
    [SerializeField] private TextMeshProUGUI deviceTypeText;
    [SerializeField] private TextMeshProUGUI deviceNameText;
    
    [SerializeField] private TextMeshProUGUI ramText;
    
    [SerializeField] private TextMeshProUGUI cpuTypeText;
    [SerializeField] private TextMeshProUGUI cpuCountText;
    
    [SerializeField] private TextMeshProUGUI gpuNameText;
    [SerializeField] private TextMeshProUGUI gpuTypeText;
    [SerializeField] private TextMeshProUGUI gpuVendorText;
    [SerializeField] private TextMeshProUGUI gpuMemorySizeText;

    [SerializeField] private TextMeshProUGUI refreshRateText;
    [SerializeField] private TextMeshProUGUI resolutionText;
    [SerializeField] private TextMeshProUGUI aspectRatioText;
    [SerializeField] private TextMeshProUGUI dpiDisplayText;
    protected override ConsoleTabType consoleTabType => ConsoleTabType.Information;

    public override void Setup(ConsoleTabType tabType)
    {
        base.Setup(tabType);
        SetupInformation();
    }

    void SetupInformation()
    {
        // Application Information
        identificationText.text = Application.identifier;
        versionText.text = Application.version;
        languageText.text = CultureInfo.CurrentCulture.DisplayName;
        platformText.text = Application.platform.ToString();

        // Device Information
        operatingSystemText.text = SystemInfo.operatingSystem;
        deviceModelText.text = SystemInfo.deviceModel;
        deviceTypeText.text = SystemInfo.deviceType.ToString();
        deviceNameText.text = SystemInfo.deviceName;
        
        // CPU Information
        cpuTypeText.text = SystemInfo.processorType;
        cpuCountText.text = SystemInfo.processorCount.ToString();
        
        // RAM Information
        ramText.text =  $"{SystemInfo.systemMemorySize} MB";
        
        // GPU Information
        gpuNameText.text = SystemInfo.graphicsDeviceName;
        gpuTypeText.text = SystemInfo.graphicsDeviceType.ToString();
        gpuVendorText.text = SystemInfo.graphicsDeviceVendor;
        gpuMemorySizeText.text =  $"{SystemInfo.graphicsMemorySize} MB";

        // Display Information
        Resolution resolution = Screen.currentResolution;
        refreshRateText.text = $"{resolution.refreshRate}Hz";
        resolutionText.text = $"{resolution.width} x {resolution.height}";
        aspectRatioText.text = ((float)resolution.width / (float)resolution.height).ToString("0.00");
        dpiDisplayText.text =  $"{Screen.dpi} DPI";
    }
}
