using System.Threading.Tasks;
using UnityEngine;

public class AbilityCircle : MonoBehaviour
{
    public Ability Script;
    private void OnTriggerEnter(Collider other)
    {
        if (Script != null && other.gameObject.tag is "EnemyUnit" or "Unit")
        {
            Script.Use(other.transform.parent.GetComponent<AttackUnit>());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (Script != null && other.gameObject.tag is "EnemyUnit" or "Unit")
        {
            Script.DeUse(other.transform.parent.GetComponent<AttackUnit>());
        }
    }
    
    public async void Deactivate()
    {
        await Task.Delay(1000);
        Script.Cooldown();
        gameObject.SetActive(false);
    }
}
