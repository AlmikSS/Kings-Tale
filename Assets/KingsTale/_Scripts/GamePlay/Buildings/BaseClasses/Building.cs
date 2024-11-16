using System.Collections.Generic;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody), typeof(NavMeshObstacle))]
public abstract class Building : NetworkBehaviour, IDamagable
{
    [HideInInspector] public List<Effect> Effects = new();

    [SerializeField] protected BuildingBaseConfigSO _config;
    [SerializeField] private LayerMask _obstacleMask;
    [SerializeField] private ushort _id;
    [SerializeField] private HealthSlider _healthSlider;

    public bool CanBuild { get; private set; } = true;
    public ushort Id => _id;
 
    private int _magicResist;
    private int _physicalResist;
    private int _currentHealth;

    public override void OnNetworkSpawn()
    {
        _currentHealth = (int)_config.MaxHealth;
        _magicResist = (int)_config.MagicResist;
        _physicalResist = (int)_config.PhysicalResist;
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(GamePlayConstants.BUILDING_TAG))
            CanBuild = false;
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(GamePlayConstants.BUILDING_TAG))
            CanBuild = true;
    }
    
    public IEnumerator ChangeParam(Effect name, int value, float duration, bool increase = true)
    {
        float defaultValue;
        switch (name)
        {
            case Effect.Alchemist:
                Effects.Add(name);
                defaultValue = _magicResist;
                _magicResist = Mathf.Clamp(increase ? _magicResist + value : _magicResist - value, 0, 10000);
			   
                yield return new WaitForSeconds(duration);
                Effects.Remove(name);
                _magicResist = (int)defaultValue;
                break;
        }
	    
    }
    
    public virtual void TakeDamage(int damage, DamageType type = 0, Effect fx = Effect.None)
    {
        var dmg = damage - (type == DamageType.Magical ? _magicResist : _physicalResist);
	    
        if (damage > 0)
        {

            _currentHealth -= dmg;
            _healthSlider.TakeDamage(dmg, type, fx);
        }
        else if(_currentHealth > 0)
            _healthSlider.Defence(type);

        if (_currentHealth <= 0)
            Die();
    }
    
    public virtual void Die()
    {
        Destroy(gameObject);
    }
}

