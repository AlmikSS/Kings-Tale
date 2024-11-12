using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public sealed class MovingGroup
{
    public List<UnitBrain> _activeAgents;
    
    private readonly List<UnitBrain> _complitedAgents;
    public GameObject Line;

    public MovingGroup(IEnumerable<UnitBrain> agents, GameObject line = null)
    {
        this._activeAgents = new List<UnitBrain>(agents);
        this._complitedAgents = new List<UnitBrain>();
        this.Line = line;
    }

    public bool IsComplited()
    {
        return this._activeAgents.Count <= 0;
    }

    public void Update(float stopDist)
    {
        foreach (var agent in this._activeAgents.ToList())
        {
            if (agent.IsComplited())
            {
                this._activeAgents.Remove(agent);
                this._complitedAgents.Add(agent);
            }
        }

        foreach (var agent in this._activeAgents.ToList())
        {
			if(agent == null)
				return;
            var position = agent.transform.position;
            foreach (var otherAgent in this._complitedAgents.ToList())
            {
                
                var otherPosition = otherAgent.transform.position;
                if (Vector3.Distance(position, otherPosition) < stopDist)
                {
                    agent.Complete();
                    break;
                }
            }
        }
    }

    public void RemoveAgents(IEnumerable<UnitBrain> agents)
    {
        foreach (var _agent in agents)
        {
            if (this._activeAgents.Contains(_agent))
            {
                this._activeAgents.Remove(_agent);
            }
        }
    }
}