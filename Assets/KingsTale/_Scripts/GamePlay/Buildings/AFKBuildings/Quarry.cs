using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Quarry : Building
{
    [SerializeField] private float _culDown;
    [SerializeField] private ResourcesStruct _resourcesToAdd;
    
    private GoldenMine _currentGoldenMine;

    private IEnumerator LiveCycleRoutine()
    {
        while (_currentGoldenMine.Cycles > 0)
        {
            yield return new WaitForSeconds(_culDown);

            var request = new ServerAddResourcesRequestStruct
            {
                PlayerId = OwnerClientId,
                ResourcesToAdd = _resourcesToAdd
            };

            InputManager.Instance.HandleAddResourcesRequestRpc(request);
            _currentGoldenMine.Mine();
        }
    }

    [Rpc(SendTo.Owner)]
    public override void PlaceBuildingRpc()
    {
        if (!IsOwner) { return; }
        
        _isPlaced.Value = true;
        var cols = Physics.OverlapSphere(transform.position, 0.3f);

        foreach (var col in cols)
        {
            if (col.gameObject.TryGetComponent(out GoldenMine mine))
            {
                transform.position = mine.transform.position;
                _currentGoldenMine = mine;
                break;
            }
        }
    }

    [Rpc(SendTo.Owner)]
    public override void BuildRpc()
    {
        if (!IsOwner) { return; }

        _isBuilt.Value = true;
        GetComponentInChildren<MeshRenderer>().sharedMaterial.color = Color.white;

        StartCoroutine(LiveCycleRoutine());
    }
}