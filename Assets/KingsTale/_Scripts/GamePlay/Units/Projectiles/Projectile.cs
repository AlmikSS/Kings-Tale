using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    [SerializeField] private AnimationCurve _yPositionCurve;
    [SerializeField] private ushort _id;
    [SerializeField] private float _speed;
    
    public ushort Id => _id;
    
    private NetworkObject _target;
    private uint _damage;

    public void Launch(NetworkObject target, uint damage)
    {
        _target = target;
        _damage = damage;
        StartCoroutine(FlyRoutine());
    }

    private IEnumerator FlyRoutine()
    {
        var startPosition = transform.position;
        var targetPosition = _target.transform.position;
        
        var distance = Vector3.Distance(startPosition, targetPosition);
        var flightDuration = distance / _speed;
        
        var elapsedTime = 0f;

        while (elapsedTime < flightDuration)
        {
            elapsedTime += Time.deltaTime;
            
            var previousPos = transform.position;
            var progress = Mathf.Clamp01(elapsedTime / flightDuration);
            var currentPosition = Vector3.Lerp(startPosition, targetPosition, progress);
            
            currentPosition.y = _yPositionCurve.Evaluate(progress);
            transform.position = currentPosition;
            
            var direction = (currentPosition - previousPos).normalized;

            if (direction != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(direction);
            
            yield return null;
        }
        
        Invoke(nameof(Despawn), 0.3f);
    }

    private void Despawn()
    {
        var request = new ServerDespawnRequestStruct
        {
            Id = NetworkObjectId
        };
        
        InputManager.Instance.HandleDespawnRequestRpc(request);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out NetworkObject networkObject))
        {
            if (networkObject.NetworkObjectId == _target.NetworkObjectId)
            {
                var request = new ServerTakeDamageRequestStruct
                {
                    PlayerId = OwnerClientId,
                    Damage = _damage,
                    Id = _target.NetworkObjectId
                };
                
                InputManager.Instance.HandleTakeDamageRequestRpc(request);
            }
        }
    }
}