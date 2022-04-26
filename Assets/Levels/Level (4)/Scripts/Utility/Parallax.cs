using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private Transform[] _layers;
    [SerializeField] private float[] _coeff;

    private int _layersCount;


    void Awake()
    {
        _layersCount = _layers.Length;
    }
    


    // Update is called once per frame
    void Update()
    {

        for (int i = 0; i < _layersCount; i++)
        {
            _layers[i].position = transform.position * _coeff[i];
        }
        
    }
}
