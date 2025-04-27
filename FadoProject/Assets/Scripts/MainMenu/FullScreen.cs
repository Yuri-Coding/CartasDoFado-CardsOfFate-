using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class FullScreen : MonoBehaviour
{
    public Toggle toggleFS;
    public TMP_Dropdown resDropdown;

    Resolution[] resolutions;
    List<Resolution> resolutions169 = new List<Resolution>();

    void Start()
    {
        resolutions = Screen.resolutions;
        resolutions169.Clear(); 

        resDropdown.ClearOptions();
        List<string> optionsList = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            float aspectRatio = (float)resolutions[i].width / resolutions[i].height;
            if (Mathf.Abs(aspectRatio - (16f / 9f)) > 0.01f) continue;

            resolutions169.Add(resolutions[i]);
            optionsList.Add(resolutions[i].width + " x " + resolutions[i].height);

            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = resolutions169.Count - 1;
            }
        }

        resDropdown.AddOptions(optionsList);

        resDropdown.value = currentResolutionIndex;
        resDropdown.RefreshShownValue();

        toggleFS.isOn = Screen.fullScreen;
    }

    public void SetRes(int index)
    {
        if (index < 0 || index >= resolutions169.Count) return; // Safety check
        Resolution res = resolutions169[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        PlayerPrefs.SetInt("resolutionIndex", index);
    }

    public void FullscreenControl()
    {
        Screen.fullScreen = toggleFS.isOn;
        PlayerPrefs.SetInt("fullscreen", toggleFS.isOn ? 1 : 0);
    }
}