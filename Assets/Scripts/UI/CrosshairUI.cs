using UnityEngine;
using UnityEngine.UI;

public class CrosshairUI : MonoBehaviour
{
    public RawImage Crosshair;

    public Player player;



    void OnEnable()
    {
        player.Controller.CameraConstrained += HideCrosshair;
        player.Controller.CameraUnconstrained += ShowCrosshair;
    }
    void OnDisable()
    {
        player.Controller.CameraConstrained -= HideCrosshair;
        player.Controller.CameraUnconstrained -= ShowCrosshair;
    }


    void ShowCrosshair()
    {
        Crosshair.enabled = true;
    }
    void HideCrosshair()
    {
        Crosshair.enabled = false;
    }


}