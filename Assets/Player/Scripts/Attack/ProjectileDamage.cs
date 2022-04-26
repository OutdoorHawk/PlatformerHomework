using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDamage : MonoBehaviour
{
   [SerializeField]private float _damage;

    private float[] attackDetails = new float[2];
    private Rigidbody2D _thisRb;

    private void Awake()
    {
        _thisRb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        attackDetails[0] = _damage;
        attackDetails[1] = transform.position.x;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            try
            {
                collision.transform.parent.SendMessage("Damage", attackDetails);
                Destroy(gameObject);
            }
            catch (System.Exception)
            {

                throw;
            }

        }
        else
            StartCoroutine(ProjectileDestroy());
        
    }

    private IEnumerator ProjectileDestroy()
    {
        
     
        yield return new WaitForSeconds(4f);
        Destroy(gameObject);
    }
}
