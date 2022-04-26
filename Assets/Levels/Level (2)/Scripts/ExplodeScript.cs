using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeScript : MonoBehaviour
{
    [SerializeField] private GameObject generateExplotion;
    [SerializeField] private GameObject flObjects;
    [SerializeField] private Animator anim;

    GameObject MyGenObject;
  
   private SliderJoint2D[] _sl;
   private SpringJoint2D[] _sp;
   private WheelJoint2D[] _wh;

    private void Awake()
    {
        _sl = flObjects.GetComponentsInChildren<SliderJoint2D>();
        _sp = flObjects.GetComponentsInChildren<SpringJoint2D>();
        _wh = flObjects.GetComponentsInChildren<WheelJoint2D>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {   //Удаление всех джоинтов на парящих объектах
        if (collision.gameObject.CompareTag("Player"))
        {
            anim.SetTrigger("Death");
            MyGenObject = Instantiate(generateExplotion, transform.position, transform.rotation);

            for (int i = 0; i < _sl.Length; i++)
            {
                Destroy(_sl[i]);

            }
            for (int i = 0; i < _sp.Length; i++)
            {
                Destroy(_sp[i]);

            }
            for (int i = 0; i < _wh.Length; i++)
            {
                Destroy(_wh[i]);

            }
        }
        
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
           
            StartCoroutine(explodeDelay());



        }

    }
    /// <summary>
    /// Задержка перед удалением эффектора взрыва
    /// </summary>
    /// <returns></returns>
    private IEnumerator explodeDelay()
    {
        yield return new WaitForSeconds(1.2f);
      
        Destroy(gameObject);
        Destroy(MyGenObject);
    
    }
}
