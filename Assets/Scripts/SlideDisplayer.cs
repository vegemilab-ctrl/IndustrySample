using UnityEngine;
using UnityEngine.UI;

public class SlideDisplayer : MonoBehaviour
{
    public Text text;

    private void Start()
    {
        text = GetComponent<Text>();
    }

    public void OnChangedValue(float value)
    {
        text.text = value.ToString("F0");
    }

}