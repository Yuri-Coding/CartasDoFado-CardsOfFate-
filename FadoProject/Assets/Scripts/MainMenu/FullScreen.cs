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

    List<Resolution> resolutions169 = new List<Resolution>();

    void Start(){
        resolutions = Screen.resolutions;
        Resolution current16by9Resolution = Screen.currentResolution;

        resDropdown.ClearOptions();

        List<string> optionsList = new List<string>();

        int currentResolution = 0;

        for(int i = 0; i < resolutions.Length; i++){
            float aspectRatio = (float)resolutions[i].width / resolutions[i].height;

            // Ignora resoluções com proporção de tela diferente de 16:9 (aproximadamente 1.777...)
            if (Mathf.Abs(aspectRatio - (16f / 9f)) > 0.01f) {
                continue; // Pula para a próxima iteração se não for 16:9
            }

            string option = resolutions[i].width + " x " + resolutions[i].height;
            optionsList.Add(option);

            resolutions169.Add(resolutions[i]);

            if(resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height){
                currentResolution = optionsList.Count - 1;
                current16by9Resolution = resolutions[i];
            }
        }

        resDropdown.AddOptions(optionsList);
        resDropdown.value = currentResolution;
        resDropdown.RefreshShownValue();
    }

    public void SetRes(int index){
        Screen.SetResolution(resolutions169[index].width,resolutions169[index].height, Screen.fullScreen);
    }

    public void FullscreenControl()
    {
        Screen.fullScreen = toggleFS.isOn;
    }
}
