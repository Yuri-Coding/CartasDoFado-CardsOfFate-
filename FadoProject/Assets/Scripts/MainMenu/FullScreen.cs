using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FullScreen : MonoBehaviour
{
    public Toggle toggleFS;
    public void FullscreenControl()
    {
        Screen.fullScreen = toggleFS.isOn;
    }
}
