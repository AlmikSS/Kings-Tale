using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameplayButtons : MonoBehaviour
{
    public ulong PlayerId { get; set; }
    
    [SerializeField] private GameObject _map, _shop, _hud, _groups, _mainCam, _mapCam, _res, _info, _infoBtns;
    [SerializeField] private ScrollRect _shopScroll;
    [SerializeField] private Image[] _imagesToTransparent;
    [SerializeField] private CardsUI _cards;
        
    private static GameObject map, shop, hud, groups, mainCam, mapCam, res, info, infoBtns;
    private static Image[] imagesToTransparent;
    private static PlayerManager plManager;
    
    private PlayerInput _controller;

    public void Awake()
    {
        map = _map;
        shop = _shop;
        hud = _hud;
        groups = _groups;
        mainCam = _mainCam;
        mapCam = _mapCam;
        res = _res;
        imagesToTransparent = _imagesToTransparent;
        infoBtns = _infoBtns;
    }
    private void OnEnable()
    {
        _controller = GetComponent<PlayerInput>();
        _controller.actions["Shop"].started += OpenShop;
        _controller.actions["Map"].started += OpenMap;
        _controller.actions["Escape"].started += Escape;
        _controller.actions["Numpad"].performed += Numpad;
    }

    private void OnDisable()
    {
        _controller.actions["Shop"].started -= OpenShop;
        _controller.actions["Map"].started -= OpenMap;
        _controller.actions["Escape"].started -= Escape;
        _controller.actions["Numpad"].performed -= Numpad;
    }
    
    private void OpenShop(InputAction.CallbackContext _) => GameplayButtons.OpenShop();
    private void OpenMap(InputAction.CallbackContext _) => GameplayButtons.OpenMap();
    private void Escape(InputAction.CallbackContext _) => GameplayButtons.Escape();

    private void Numpad(InputAction.CallbackContext ctx)
    {
        var card = _cards.Cards[(int)ctx.ReadValue<float>() - 1];
        _cards.ActivateCard(card);
        card.GetComponent<UnitsGroupButton>().SelectGroup();
    }


    public static void OpenMap()
    {
        if (shop.activeSelf || !groups.activeSelf) return;
        if (!map.activeSelf)
        {
            mainCam.SetActive(false);
            mapCam.SetActive(true);
            map.SetActive(true);
            hud.SetActive(false);
            infoBtns.SetActive(false);
        }
        else
            CloseMap();
    }

    public static void OpenShop()
    {
        if (map.activeSelf || (!groups.activeSelf && !shop.activeSelf)) return;
        if (!shop.activeSelf)
        {
            shop.SetActive(true);
            hud.SetActive(false);
            groups.SetActive(false);
            res.SetActive(false);
            infoBtns.SetActive(false);
            mainCam.GetComponent<CameraMovement>().enabled = false;
        }
        else
            CloseShop();
    }

    public static void CloseMap()
    {
        mainCam.SetActive(true);
        mapCam.SetActive(false);
        map.SetActive(false);
        hud.SetActive(true);
        groups.SetActive(true);
    }
    public static void CloseShop()
    {
        shop.SetActive(false);
        hud.SetActive(true);
        groups.SetActive(true);
        res.SetActive(true);
        plManager.enabled = true;
        mainCam.GetComponent<CameraMovement>().enabled = true;
    }
    public static void CloseInfo()
    {
        // plManager.ClosedBuildingMenu();
    }
    public static void Escape()
    {
        if (shop.activeSelf)
            CloseShop();
        else if(map.activeSelf)
            CloseMap();
        else
            CloseInfo();
    }

    public static void TransparentUI()
    {
        foreach (var image in imagesToTransparent)
        {
            var tempColor = image.color;
            tempColor.a = 0.35f;
            image.color = tempColor;
        }
    }
    public static void NotTransparentUI()
    {
        foreach (var image in imagesToTransparent)
        {
            var tempColor = image.color;
            tempColor.a = 1f;
            image.color = tempColor;
        }
    }
    public void SetScrollValue(float value)
    {
        _shopScroll.horizontalNormalizedPosition = value;
    }
    
    public void GoToScene(int index) => SceneManager.LoadScene(index);
}
