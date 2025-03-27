using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class FullScreen : MonoBehaviour
{
    public Toggle toggleFS;

    public TMP_Dropdown resDropdown;

    Resolution[] resolutions;

    void Start(){
        resolutions = Screen.resolutions;

        resDropdown.ClearOptions();

        List<string> optionsList = new List<string>();    

        int currentResolution = 0;

        for(int i = 0; i < resolutions.Length; i++){
            string option = resolutions[i].width + " x " + resolutions[i].height;
            optionsList.Add(option);

            if(resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height){
                currentResolution = i;
            }
        }

        resDropdown.AddOptions(optionsList);
        resDropdown.value = currentResolution;
        resDropdown.RefreshShownValue();
    }

    public void SetRes(int index){
        Screen.SetResolution(resolutions[index].width,resolutions[index].height, Screen.fullScreen);
    }

    public void FullscreenControl()
    {
        Screen.fullScreen = toggleFS.isOn;
    }
}
