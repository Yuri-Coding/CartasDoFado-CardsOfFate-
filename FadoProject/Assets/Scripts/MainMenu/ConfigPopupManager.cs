using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

using FadoProject;
using Unity.VisualScripting;

using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.Localization.Settings;
using System.Linq;

using FMODUnity;

public class ConfigPopupManager : MonoBehaviour
{
    // Start is called before the first frame update
    //anima��o
    public Animation popAnim;

    private FMOD.Studio.VCA VcaController;

    void Start()
    {
        VcaController = FMODUnity.RuntimeManager.GetVCA("vca:/Master");
    }
    public void HidePopup()
    {
        popAnim.Play("fadeOut");
    }

    public void ShowPopup()
    {
        popAnim.Play("fadeIn");
    }

    //troca o locale atual
    public void HandleLocaleChange(int choice)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[choice];
    }

    public void SetVolume(float value)
    {
        VcaController.setVolume(value);
    }

}
