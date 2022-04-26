using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
   
        static Vector3 lastCheckpointPosition = Vector3.zero; //������� ���������� ���������
                                                              //��� ��� ��� ����������� �� ����� ������������ �� ���������� ����

        private void Start()
        {
            if (lastCheckpointPosition != Vector3.zero)
            {
                transform.position = lastCheckpointPosition;
                //�������� ��������� ������ ���� �� "������" ���� �� ���� ��������
            }
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Save"))
            {
                lastCheckpointPosition = collision.transform.position;
                //������ ������� ����� ���������� � ����������
            }
        }
    
}