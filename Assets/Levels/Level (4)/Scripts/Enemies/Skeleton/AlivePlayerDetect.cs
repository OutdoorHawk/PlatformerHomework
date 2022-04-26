using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlivePlayerDetect : MonoBehaviour
{
    public bool[] _playerDetected;

   
  

   

   


   
    private void Start()
    {
        
        _playerDetected = new bool[2];

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _playerDetected[0] = true;

            if (collision.transform.position.x < transform.position.x)
            {
                _playerDetected[1] = false;
            }
            else
            {
                _playerDetected[1] = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _playerDetected[0] = false;
        }
    }
   

    
}
