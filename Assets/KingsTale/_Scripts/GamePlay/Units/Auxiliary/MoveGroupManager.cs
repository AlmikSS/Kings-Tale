using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MoveGroupManager : MonoBehaviour
{
    [SerializeField] private float _stopDisatance = 1f;
    
    public List<MovingGroup> _activeGroups = new List<MovingGroup>();

    public void AddGroup(IEnumerable<UnitBrain> agents, GameObject line = null)
    {
        foreach (var activeGroup in this._activeGroups)
        {
            activeGroup.RemoveAgents(agents);
        }

        this._activeGroups.Add(new MovingGroup(agents, line));
    }

    public List<UnitBrain> CheckAgent(UnitBrain brain)
    {
        foreach (var group in _activeGroups)
        {
            if (group._activeAgents.Contains(brain))
            {
                var list = group._activeAgents;
                _activeGroups.Remove(group);
                return list;
            }
        }
        return null;
    }
    private void FixedUpdate()
    {
        foreach (var group in this._activeGroups.ToList())
        {
            group.Update(_stopDisatance);

            if (group.IsComplited())
            {
                if (group.Line != null)
                    Destroy(group.Line);
                this._activeGroups.Remove(group);
            }
        }
    }
}
