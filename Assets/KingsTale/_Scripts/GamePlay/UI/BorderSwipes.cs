using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class BorderSwipes : NetworkBehaviour
{
    [SerializeField] private int DeltaPosition;
    [SerializeField] private Color DisabledColor;
    [SerializeField] private Color EnabledColor;
    [HideInInspector] public string border;
    
    private string _borderHolder;
    private float _startPosX;
    void Update()
    {
	    //if (!IsOwner) { return; }

		if(UnityEngine.Input.GetMouseButtonDown(0)){
			_startPosX = UnityEngine.Input.mousePosition.x;
            _borderHolder = border;
		}
        else if(UnityEngine.Input.GetMouseButton(0))
        {
	        var deltaX = Mathf.Abs(UnityEngine.Input.mousePosition.x - _startPosX);
	        
			if(deltaX < DeltaPosition && _borderHolder!=""){
                DisabledColor.a = (deltaX/DeltaPosition)*EnabledColor.a;
                GameObject.Find(_borderHolder).GetComponent<Graphic>().color = DisabledColor;
                GameObject.Find(_borderHolder).GetComponent<Image>().fillAmount = Mathf.Clamp(deltaX/DeltaPosition, 0, 1f); 
            }
			
            else if(_borderHolder!=""){
                GameObject.Find(_borderHolder).GetComponent<Graphic>().color = EnabledColor;
                GameObject.Find(_borderHolder).GetComponent<Image>().fillAmount = 1f;
            }
            
		}
		else if(UnityEngine.Input.GetMouseButtonUp(0)){
            DisabledColor.a = 0f;
            if(_borderHolder!=""){
                GameObject.Find(_borderHolder).GetComponent<Graphic>().color = DisabledColor;
                if(Mathf.Abs(UnityEngine.Input.mousePosition.x - _startPosX) >= DeltaPosition){
                    if(_borderHolder == "BorderMap"){
                        _borderHolder = "";
                        GameplayButtons.OpenMap();
                    }
                    else if(_borderHolder == "BorderShop"){
                        _borderHolder = "";
                        GameplayButtons.OpenShop();
                    }

                    else if (_borderHolder == "BorderCloseMap")
                    {
                        _borderHolder = "";
                        GameplayButtons.CloseMap();
                    }
                }
            }
            _borderHolder = "";
            
		}

    }
}
