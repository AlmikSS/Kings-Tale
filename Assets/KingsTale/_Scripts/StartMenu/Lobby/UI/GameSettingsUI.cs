using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class GameSettingsUI : MonoBehaviour
{
    [SerializeField] private RenderPipelineAsset[] QualityLevels;
    [SerializeField] private TMP_Dropdown Dropdown;
    [SerializeField] private TMP_InputField NickText;
    void Start()
    {
        NickText.text = PlayerPrefs.GetString("PlayerNick");
        Dropdown.value = QualitySettings.GetQualityLevel();
    }
    public void SetNick(string txt){
        PlayerPrefs.SetString("PlayerNick", txt);
    }
    public void SetGraphics(int value){
        QualitySettings.SetQualityLevel(value);
        QualitySettings.renderPipeline = QualityLevels[value];
    }
}
