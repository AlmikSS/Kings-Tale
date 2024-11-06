using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wiggle : MonoBehaviour {
    // [SerializeField] private float _minScale = 0.1f;
    public float RecomendedScale { private get; set; } = 0.1f;
    // [SerializeField] private float _speed = 5f;
    // public float PrevScale { get; private set; } = 1.0f;
    // private Vector3[] _baseVertices, vertices;
    // private Mesh mesh;
    // private void Start()
    // {
    //     mesh = GetComponent<MeshFilter>().mesh;
    //
    //     if (_baseVertices == null)
    //         _baseVertices = mesh.vertices;
    //
    //     vertices = new Vector3[_baseVertices.Length];
    //
    // }
    // public void FixedUpdate()
    // {
    //     if (Mathf.Abs(_recomendedScale - _scale) < 0.005f)
    //         return;
    //     for (var i = 0; i < vertices.Length; i++)
    //     {
    //         _scale = Mathf.Lerp(_scale, _recomendedScale, Time.fixedDeltaTime * _speed);
    //         transform.localScale = new Vector3(_scale,_scale,_scale);
    //         // vertices[i] = _baseVertices[i] * _scale;
    //     }
    //     
    //     // mesh.vertices = vertices;
    //     // mesh.RecalculateBounds();
    //
    // }

    // public void ChangeSize(float size)
    // {
    //     PrevScale = _recomendedScale;
    //     _recomendedScale = size;
    // }

    public void ChangeToBig()
    {
        transform.localScale = new Vector3(RecomendedScale,RecomendedScale,RecomendedScale);
    }
}
