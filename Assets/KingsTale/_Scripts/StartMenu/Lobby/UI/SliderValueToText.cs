using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderValueToText : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private TMP_Text _text;

    public void ChangeText(bool isFloat = false)
    {
        _text.text = !isFloat ? _slider.value.ToString("N0") : _slider.value.ToString("N1");
    }
}