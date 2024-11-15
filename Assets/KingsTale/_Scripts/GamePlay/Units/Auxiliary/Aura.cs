using System;
using UnityEngine;

public class Aura : MonoBehaviour
{
    [SerializeField] private AuraType _aura;
    private EffectSystem fxSys;
    
    void Start() => fxSys = FindObjectOfType<EffectSystem>();
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "UnitCollider" && _aura == AuraType.Heal)
        {
            fxSys.IncreaseRegen(2, other.transform.parent.GetComponent<UnitBrain>());
            print("heal");
        }
    }
    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "UnitCollider")
        {
            switch (_aura)
            {
                case AuraType.Flame:
                    fxSys.DisableEffect(Effect.Freeze, other.transform.parent.GetComponent<UnitBrain>());
                    break;
                case AuraType.Frozen:
                    fxSys.DisableEffect(Effect.Pyro, other.transform.parent.GetComponent<UnitBrain>());
                    break;
            }
        }
        else if (other.gameObject.tag == "Building" || other.gameObject.tag == "MenuBuilding")
        {
            switch (_aura)
            {
                case AuraType.Flame:
                    fxSys.DisableEffect(Effect.Freeze, targetBulding: other.GetComponent<Building>());
                    break;
                case AuraType.Frozen:
                    fxSys.DisableEffect(Effect.Pyro, targetBulding: other.GetComponent<Building>());
                    break;
            }
        }
    }
    
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "UnitCollider" && _aura == AuraType.Heal)
        {
            fxSys.IncreaseRegen(2, other.transform.parent.GetComponent<UnitBrain>(), false);
            print("out heal");
        }
    }
}

public enum AuraType
{
    Flame,
    Frozen,
    Heal
}