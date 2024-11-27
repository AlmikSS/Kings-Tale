using UnityEngine;

public class CanvasCameraSetter : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;

    private void Awake()
    {
        _canvas.worldCamera = GameObject.FindGameObjectWithTag("MainCam").GetComponent<Camera>();
    }
}