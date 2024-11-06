using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuBtns : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioMixer _mixer;
    [SerializeField] private Slider _soundSlider;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _SFXSlider;

    [Header("Fonts")]
    [SerializeField] private TMP_FontAsset[] _fonts;
    [SerializeField] private TMP_Dropdown _languageDrop;

    [Header("Graphics")] 
    [SerializeField] private TMP_Dropdown[] _graphicsDrops;
    [SerializeField] private Slider _renderScale;
    [SerializeField] private GameObject _postProcessing;

    [Header("Game")]
    [SerializeField] private TMP_Dropdown _invertMouse;
    [SerializeField] private TMP_Dropdown _fullscreenDrop;
    
    private bool _activePP;
    private int _customFPS = 2;
    private PlayerInput _controller;
    
    private void Start()
    {
        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("Graphics"));
        if (PlayerPrefs.GetInt("InGame") == 0)
        {
            PlayerPrefs.SetFloat("GeneralSound", 50f);
            PlayerPrefs.SetFloat("Music", 50f);
            PlayerPrefs.SetFloat("SFX", 50f);
            PlayerPrefs.SetInt("Resolution", Screen.resolutions.Length - 1);
        }
        _mixer.SetFloat("GeneralSound", Mathf.Log10(PlayerPrefs.GetFloat("GeneralSound") / 100f) * 20f);
        _mixer.SetFloat("Music", Mathf.Log10(PlayerPrefs.GetFloat("Music") / 100f) * 20f);
        _mixer.SetFloat("SFX", Mathf.Log10(PlayerPrefs.GetFloat("SFX") / 100f) * 20f);

        ChangeLocale(PlayerPrefs.GetInt("Language"));
        
        if (SceneManager.GetActiveScene().buildIndex != 0) return;
        
        _soundSlider.value = PlayerPrefs.GetFloat("GeneralSound");
        _musicSlider.value = PlayerPrefs.GetFloat("Music");
        _SFXSlider.value = PlayerPrefs.GetFloat("SFX");

        _languageDrop.value = PlayerPrefs.GetInt("Language");
        ChangeGraphics(PlayerPrefs.GetInt("Graphics"));
        _graphicsDrops[0].value = PlayerPrefs.GetInt("Graphics");
        _invertMouse.value = PlayerPrefs.GetInt("InvertMouse");
        _fullscreenDrop.value = PlayerPrefs.GetInt("Fullscreen");
        
        _graphicsDrops[1].options.Clear();
        foreach (var res in Screen.resolutions)
        {
            _graphicsDrops[1].options.Add(new TMP_Dropdown.OptionData(text: $"{res.width}x{res.height}")); 
        }
        _graphicsDrops[1].value = PlayerPrefs.GetInt("Resolution");
        
        Application.targetFrameRate = PlayerPrefs.GetInt("FPS"); 
        QualitySettings.vSyncCount = 0;
        UpdateGraphicDowns((UniversalRenderPipelineAsset)QualitySettings.renderPipeline);
        DontDestroyOnLoad(_postProcessing);
        
        PlayerPrefs.SetInt("InGame", 1);
        
        if(TryGetComponent<PlayerInput>(out _))
            _controller = GetComponent<PlayerInput>();
    }

    public void OnEnable()
    {
        if(_controller)
            _controller.actions["Escape"].performed += Escape;
    }

    public void OnDisable()
    {
        if(_controller)
            _controller.actions["Escape"].performed -= Escape;
    }

    private void Escape(InputAction.CallbackContext _)
    {
        print("esc");
        if (GameObject.FindGameObjectWithTag("Close"))
        {
            print("find");
            GameObject.FindGameObjectWithTag("Close").GetComponent<Button>().onClick.Invoke();
        }

    }
    private void UpdateGraphicDowns(UniversalRenderPipelineAsset asset)
    {
        
        //render scale
        _renderScale.value = asset.renderScale;
        
        switch (QualitySettings.globalTextureMipmapLimit)
        {
            //texture quality
            case 3:
                _graphicsDrops[2].SetValueWithoutNotify(0);
                break;
            case 2:
                _graphicsDrops[2].SetValueWithoutNotify(1);
                break;
            case 1:
                _graphicsDrops[2].SetValueWithoutNotify(2);
                break;
            case 0:
                _graphicsDrops[2].SetValueWithoutNotify(3);
                break;
        }

        //pp
        _graphicsDrops[3].SetValueWithoutNotify(_postProcessing.activeSelf ? 0 : 1);
        //hdr
        _graphicsDrops[4].SetValueWithoutNotify(asset.supportsHDR == false ? 1 : 0);

        switch (Application.targetFrameRate)
        {
            case 24:
                _graphicsDrops[5].SetValueWithoutNotify(0);
                break;
            case 30:
                _graphicsDrops[5].SetValueWithoutNotify(1);
                break;
            case 60:
                _graphicsDrops[5].SetValueWithoutNotify(2);
                break;
            case 120:
                _graphicsDrops[5].SetValueWithoutNotify(3);
                break;
            case 144:
                _graphicsDrops[5].SetValueWithoutNotify(4);
                break;
            case 240:
                _graphicsDrops[5].SetValueWithoutNotify(5);
                break;
        }
        
        
        PlayerPrefs.SetInt("FPS", Application.targetFrameRate);
        PlayerPrefs.Save();
        
    }

    public void ChangeGraphics(int index)
    {
        
        QualitySettings.SetQualityLevel(index);
        PlayerPrefs.SetInt("Graphics", index);
        PlayerPrefs.Save();
        
        _postProcessing.SetActive((index > 1 && index != 5) || (index == 5 && _activePP));
        
        _graphicsDrops[5].value = index switch
        {
            0 => 1,
            1 => 1,
            2 => 2,
            3 => 2,
            4 => 3,
            5 => _customFPS,
            _ => _graphicsDrops[5].value
        };
        
        UpdateGraphicDowns((UniversalRenderPipelineAsset)QualitySettings.renderPipeline);
        
    }
    public void ChangeDropDown() => _graphicsDrops[0].value = PlayerPrefs.GetInt("Graphics");
    public void ChangeInvertion(int value) => PlayerPrefs.SetInt("InvertMouse", value);

    public void ChangeFullscreen(int value)
    {
        PlayerPrefs.SetInt("Fullscreen", value);
        Screen.SetResolution(Screen.width, Screen.height, value == 0);
    }
    public void ChangeResolution(int value)
    {
        PlayerPrefs.SetInt("Resolution", value);
        var res = Screen.resolutions[value]; 
        Screen.SetResolution(res.width, res.height, PlayerPrefs.GetInt("Fullscreen") == 0);
    }

    public void ChangeCustom()
    {
        print("_changeLocal");
        QualitySettings.SetQualityLevel(5);
        PlayerPrefs.SetInt("Graphics", 5);
        PlayerPrefs.Save();
        
        var asset = (UniversalRenderPipelineAsset)QualitySettings.renderPipeline;
        //render scale
        asset.renderScale = _renderScale.value;
        
        QualitySettings.globalTextureMipmapLimit = _graphicsDrops[2].value switch
        {
            //texture quality
            0 => 3,
            1 => 2,
            2 => 1,
            3 => 0,
            _ => QualitySettings.globalTextureMipmapLimit
        };

        //pp
        _activePP = _graphicsDrops[3].value == 0;
        _postProcessing.SetActive(_activePP);
        
        //hdr
        asset.supportsHDR = _graphicsDrops[4].value == 0;
        _graphicsDrops[0].value = 5;
        
        
        Application.targetFrameRate = _graphicsDrops[5].value switch
        {
            0 => 24,
            1 => 30,
            2 => 60,
            3 => 120,
            4 => 144,
            5 => 240,
            _ => Application.targetFrameRate
        };
        _customFPS = _graphicsDrops[5].value;
    }

    public void GoToScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void OpenUrl(string url)
    {
        Application.OpenURL(url);
    }

    public void ChangeGenSnd(float value)
    {
        var _sound = Mathf.Log10(value / 100f) * 20f;
        _mixer.SetFloat("GeneralSound", _sound);
        PlayerPrefs.SetFloat("GeneralSound", value);
        PlayerPrefs.Save();
    }

    public void ChangeMusic(float value)
    {
        var _sound = Mathf.Log10(value / 100f) * 20f;
        _mixer.SetFloat("Music", _sound);
        PlayerPrefs.SetFloat("Music", value);
        PlayerPrefs.Save();
    }

    public void ChangeSFX(float value)
    {
        var _sound = Mathf.Log10(value / 100f) * 20f;
        _mixer.SetFloat("SFX", _sound);
        PlayerPrefs.SetFloat("SFX", value);
        PlayerPrefs.Save();
    }


    public void ChangeLocale(int localeID)
    {
        StartCoroutine(SetLocale(localeID));
        PlayerPrefs.SetInt("Language", localeID);
    }
    public void CloseApp() => Application.Quit();
    private IEnumerator SetLocale(int _localeID)
    {
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localeID];
        UpdateFonts(_localeID);
    }

    private void UpdateFonts(int _localeID)
    {
        foreach (var text in Resources.FindObjectsOfTypeAll<TMP_Text>())
            if (text.gameObject.CompareTag("Header"))
            {
                if (_localeID == 4)
                    text.font = _fonts[2];
                else if (_localeID == 5)
                    text.font = _fonts[4];
                else
                    text.font = _fonts[0];
            }
            else if (text.gameObject.CompareTag("Simple"))
            {
                if (_localeID == 4)
                    text.font = _fonts[3];
                else if (_localeID == 5)
                    text.font = _fonts[5];
                else
                    text.font = _fonts[1];
            }
    }
}