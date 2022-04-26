using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class SoulParticle : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private float _soulSpeed;

    private ParticleSystem _soulParticles;
    private Rigidbody2D _emitterRigidbody;

    private Vector2 _targetVector;
    private Vector2 _emitterTargetVector;

    private ParticleSystem.EmissionModule _emission;


    private float _soulDistance;

    private void Awake()
    {
        _soulParticles = GetComponent<ParticleSystem>();

       
        _emitterRigidbody = GetComponent<Rigidbody2D>();

        _emission = _soulParticles.emission;
    }

    private void Update()
    {
        _emitterTargetVector = (_playerTransform.position - transform.position) * _soulSpeed;


        _soulDistance = _emitterTargetVector.magnitude;


        _emitterRigidbody.velocity = _emitterTargetVector;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _emission.enabled = false;

            StartCoroutine(DestroyEmitter());
        }
    }

    private IEnumerator DestroyEmitter()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
}
