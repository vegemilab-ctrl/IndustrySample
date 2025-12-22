using UnityEngine;

public class SignalCommander : MonoBehaviour
{
    public TowerLampController controller;

    private bool _isOnSTF;
    private bool _isOnSTR;

    public void OnChangedSTF(bool isOn)
    {
        _isOnSTF = isOn;

        controller.IsOnGreen = _isOnSTF;
        controller.IsOnYellow = _isOnSTR;
        controller.IsOnRed = !_isOnSTF && !_isOnSTR;
    }

    public void OnChangedSTR(bool isOn)
    {
        _isOnSTR = isOn;

        controller.IsOnGreen = _isOnSTF;
        controller.IsOnYellow = _isOnSTR;
        controller.IsOnRed = !_isOnSTF && !_isOnSTR;
    }
}