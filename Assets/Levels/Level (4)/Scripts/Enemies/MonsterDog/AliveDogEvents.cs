using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AliveDogEvents : MonoBehaviour
{
    private float[] _attackDetails = new float[2];
    [SerializeField]
    private float _collisionDamage;

    private void OnCollisionEnter2D(Collision2D collision)
    {

        _attackDetails[0] = _collisionDamage;
        _attackDetails[1] = transform.position.x;

        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SendMessage("Damage", _attackDetails);
        }
    }
}
