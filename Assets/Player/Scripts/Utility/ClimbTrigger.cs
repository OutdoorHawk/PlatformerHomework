using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbTrigger : MonoBehaviour
{
    CharacterController2D CharConrol;
    BoxCollider2D box;
    // Start is called before the first frame update
   
    private void Awake()
    {
        CharConrol = GetComponentInParent<CharacterController2D>();
        box = GetComponents<BoxCollider2D>()[0];
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 7 || collision.gameObject.layer == 8)
        {
            box.enabled = false;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 7|| collision.gameObject.layer == 8)
        {
            box.enabled = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 8 || collision.gameObject.layer == 0)
        {
            if (CharConrol.m_Rigidbody2D.velocity.y <= 0)
            {
                
                CharConrol.StartAnimLedge();
            }
           
        }
        else
        {
            box.enabled = false;

            StartCoroutine(collDelay());


        }
    }
    private IEnumerator collDelay()
    {
        yield return new WaitForSeconds(1f);
        box.enabled = true;


    }
}
