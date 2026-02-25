using UnityEngine;
using UnityEngine.UI;

public class CrosshairUI : MonoBehaviour
{
    public RawImage Crosshair;

    public Player player;



    void OnEnable()
    {
        player.Controller.Constrained += HideCrosshair;
        player.Controller.UnConstrained += ShowCrosshair;
    }
    void OnDisable()
    {
        player.Controller.Constrained -= HideCrosshair;
        player.Controller.UnConstrained -= ShowCrosshair;
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