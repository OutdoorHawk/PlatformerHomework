using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WagonScript : MonoBehaviour
{
    [SerializeField] private float motorSpeed;

    private WheelJoint2D[] _wheels;
    private JointMotor2D motor;
    
    private void Awake()
    {
        motor.maxMotorTorque = 10000;
        _wheels = GetComponentsInChildren<WheelJoint2D>();
        

    }

    // Update is called once per frame
    void Update()
    {
        motor.motorSpeed = motorSpeed;
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _wheels[0].motor = motor;
            _wheels[1].motor = motor;
         
        }
    }
}
