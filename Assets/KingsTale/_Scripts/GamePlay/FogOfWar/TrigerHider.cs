using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class TrigerHider : MonoBehaviour
{
	private float _radius;
	private Vector3 _lastPos;
	private List<GameObject> _cloudsObj = new();
	private bool IsUnit;
	private void Start()
	{
		_lastPos = transform.position;
		IsUnit = transform.parent.TryGetComponent<UnitBrain>(out UnitBrain unit);
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.tag == "Cloud")
		{
			var obj = other.transform.GetChild(0).gameObject;
			if (obj.activeSelf)
			{
				obj.SetActive(false);
				_cloudsObj.Add(obj);
			}
		}
	}
	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "Cloud")
		{
			var obj = other.transform.GetChild(0).gameObject;
			if (this._cloudsObj.Contains(obj))
			{
				obj.SetActive(true);
				_cloudsObj.Remove(obj);
			}
		}
	}
	

	private void OnDestroy()
	{
		for (int i = 0; i < _cloudsObj.Count; i++)
		{
			_cloudsObj[i].gameObject.SetActive(true);
		}
	}
}