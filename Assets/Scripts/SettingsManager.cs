using UnityEngine;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    public TMP_Dropdown flightModeDropdown;
    public TMP_Dropdown imageCaptureModeDropdown;

    void Start()
    {
        LoadSettings();
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("FlightMode", flightModeDropdown.value);
        PlayerPrefs.SetInt("ImageCaptureMode", imageCaptureModeDropdown.value);
        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        flightModeDropdown.value = PlayerPrefs.GetInt("FlightMode", 0);
        imageCaptureModeDropdown.value = PlayerPrefs.GetInt("ImageCaptureMode", 0);

        flightModeDropdown.RefreshShownValue();
        imageCaptureModeDropdown.RefreshShownValue();
    }
}