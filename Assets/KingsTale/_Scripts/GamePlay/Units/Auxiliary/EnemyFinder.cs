using System.Collections;
using System.Linq;
using UnityEngine;

public class EnemyFinder : MonoBehaviour
{
    [SerializeField] private Projectile _projectile;
    [SerializeField] private int _count = 3;
    [SerializeField] private bool _damage;
    private GameObject[] _findedObj = new GameObject[0];
    public void OnTriggerEnter(Collider other)
    {
        var parent = other.transform.parent;
        ////////////////ADD SERVER UNIT CHECK
        if (!_damage && parent && parent.gameObject.TryGetComponent(out UnitBrain _) && !_findedObj.Contains(parent.gameObject) && _count > 0)
        {
            _projectile.TargetObject = parent;
            _projectile.Launch();
            _count -= 1;
            _findedObj.Append(parent.gameObject);
            StopAllCoroutines();
            GetComponent<SphereCollider>().enabled = false;
        }
        else if (_damage)
        {
            _projectile.GetComponent<LightningProjectile>().Hit(other);
        }
    }

    public IEnumerator StartDestroy()
    {
        yield return new WaitForSeconds(_projectile.AutoDestroy);
        Destroy(_projectile.gameObject);
    }
}
