using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AliveAnimEvents : MonoBehaviour
{

    [SerializeField]
    private float attack1Radius, attack1Damage;
    [SerializeField]
    private Transform attack1HitBoxPos;
    [SerializeField]
    private float _attackOffsetSpeed;
    [SerializeField]
    private LayerMask whatIsDamageable;

    private Rigidbody2D _rigidbody;
    private Transform _aliveTransfrorm;

    private float[] attackDetails = new float[2];

 




   
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _aliveTransfrorm = GetComponent<Transform>();
    

    }

    private void CheckAttackHitBox()
    {
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attack1HitBoxPos.position, attack1Radius, whatIsDamageable);

        attackDetails[0] = attack1Damage;
        attackDetails[1] = _aliveTransfrorm.position.x;

        foreach (Collider2D collider in detectedObjects)
        {
            try
            {
                if (collider.CompareTag("Player"))
                {
                    collider.transform.SendMessage("Damage", attackDetails);

                }
            }
            catch (System.Exception)
            {

                throw;
            }





        }
    }

    private void AttackOffset()
    {
        Vector3 targetVelocity;

        if (_aliveTransfrorm.localScale.x > 0)
        {
            targetVelocity = new Vector2(1, 0);
        }
        else
        {
            targetVelocity = new Vector2(-1, 0);
        }

        _rigidbody.velocity = targetVelocity * _attackOffsetSpeed;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attack1HitBoxPos.position, attack1Radius);
    }
}
