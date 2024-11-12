using UnityEngine;

public class ArrowProjectile : Projectile
{
    [Range(20.0f, 75.0f)] public float LaunchAngle;
    
    private Rigidbody rigid;
    private Quaternion initialRotation;
    
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        initialRotation = Quaternion.Euler(90,0,0);
    }
    
    void Update ()
    {
        // update the rotation of the projectile during trajectory motion
        transform.rotation = Quaternion.LookRotation(rigid.linearVelocity) * initialRotation;

    }

    // launches the object towards the TargetObject with a given LaunchAngle
    public override void Launch()
    {
        // think of it as top-down view of vectors: 
        // we don't care about the y-component(height) of the initial and target position.
        Vector3 projectileXZPos = new Vector3(transform.position.x, 0.0f, transform.position.z);
        Vector3 targetXZPos = new Vector3(TargetObject.position.x, 0.0f, TargetObject.position.z);
    
        // rotate the object to face the target
        transform.LookAt(targetXZPos);

        // shorthands for the formula
        float R = Vector3.Distance(projectileXZPos, targetXZPos);
        float G = Physics.gravity.y;
        float tanAlpha = Mathf.Tan(LaunchAngle * Mathf.Deg2Rad);
        float H = TargetObject.position.y - transform.position.y;

        // calculate the local space components of the velocity 
        // required to land the projectile on the target object 
        float Vz = Mathf.Sqrt(G * R * R / (2.0f * (H - R * tanAlpha)) );
        float Vy = tanAlpha * Vz;

        // create the velocity vector in local space and get it in global space
        Vector3 localVelocity = new Vector3(0f, Vy, Vz);
        Vector3 globalVelocity = transform.TransformDirection(localVelocity);

        // launch the object by setting its initial velocity and flipping its state
        rigid.linearVelocity = globalVelocity;
    }
    
    public override void OnTriggerEnter(Collider other)
    {
        var type = DamageType.Physical;
        var fx = _effect.Name;
        if (other.gameObject.TryGetComponent(out Building building)){
            Brain.SetAttackObject(other.gameObject);
            _effect.TargetBulding = building;
            if (_effect.Name == Effect.Alchemist)
            {
                _fxSys.ApplyEffect(_fxSys.DecreaseMagic, _effect);
                type = DamageType.Magical;
            }
            Brain.ApplyDamage(1, type, fx);
            
            Destroy(gameObject);
        }
        else if (other.transform.parent && other.transform.parent.gameObject.TryGetComponent(out EnemyUnit unit)){
            Brain.SetAttackObject(other.transform.parent.gameObject);
            _effect.Target = unit;
            if (_effect.Name == Effect.Alchemist)
            {
                _fxSys.ApplyEffect(_fxSys.DecreaseMagic, _effect);
                type = DamageType.Magical;
            }

            Brain.ApplyDamage(other.gameObject.name != "UnitWeakPlace" ? 1 : 2, type, fx);

            Destroy(gameObject);
        }
        else if(other.gameObject.tag == "Ground"){
            Destroy(gameObject);
        }
    }

}
