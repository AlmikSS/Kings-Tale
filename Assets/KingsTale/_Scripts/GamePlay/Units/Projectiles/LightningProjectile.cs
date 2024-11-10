using UnityEngine;

public class LightningProjectile : Projectile
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private SphereCollider _enemyCollider;
    private Vector3 _targetPos;
    
    public void FixedUpdate()
    {
        
        transform.position = Vector3.MoveTowards(transform.position, _targetPos, _speed * Time.fixedDeltaTime);
        //if(transform.position == _targetPos) Destroy(gameObject);
    }

    public override void Launch()
    {
        _targetPos = new Vector3(TargetObject.position.x, transform.position.y, TargetObject.position.z);
        transform.LookAt(TargetObject);
        transform.rotation = Quaternion.Euler(-90f, transform.eulerAngles.y, transform.eulerAngles.z);
    }


    public void Hit(Collider other)
    {
        if (Brain && other.gameObject.TryGetComponent(out Building building))
        {
            Brain.SetAttackObject(other.gameObject);
            _effect.TargetBulding = building;
            Brain.ApplyDamage(1, DamageType.Magical, Effect.Electro);

            Destroy(gameObject);
        }
        else if (Brain && other.transform.parent && other.transform.parent.gameObject.TryGetComponent(out EnemyUnit unit))
        {
            var lastObj = Brain._objectToAttack;
            Brain.SetAttackObject(other.transform.parent.gameObject);
            _effect.Target = unit;
            _fxSys.ApplyEffect(_fxSys.MicroStan, _effect);
            
            Brain.ApplyDamage(other.gameObject.name != "UnitWeakPlace" ? 1 : 2, DamageType.Magical, Effect.Electro);
            if(lastObj)
                Brain.SetAttackObject(lastObj);
            _enemyCollider.enabled = true;
            StartCoroutine(transform.GetChild(1).GetComponent<EnemyFinder>().StartDestroy());
        }
        else if(other.gameObject.tag == "Ground"){
            Destroy(gameObject);
        }
    }
    public override void OnTriggerEnter(Collider other){}
}
