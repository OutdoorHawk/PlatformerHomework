using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
   
        static Vector3 lastCheckpointPosition = Vector3.zero; //Позиция последнего чекпоинта
                                                              //Так как она статическая не будет сбрасываться до выключения игры

        private void Start()
        {
            if (lastCheckpointPosition != Vector3.zero)
            {
                transform.position = lastCheckpointPosition;
                //Телепорт персонажа только если он "собрал" хотя бы один чекпоинт
            }
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Save"))
            {
                lastCheckpointPosition = collision.transform.position;
                //Запись позиции точки сохранения в переменную
            }
        }
    
}
