using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlObjScript : MonoBehaviour
{
    private SliderJoint2D _slJoint;
    private Rigidbody2D _rb;
    private JointMotor2D motor;
    [SerializeField] private float motorSpeed;
    private void Awake()
    {
        _slJoint = GetComponent<SliderJoint2D>();
        _rb = GetComponent<Rigidbody2D>();

        motor = _slJoint.motor;
       


    }
    // Update is called once per frame
    void Update()
    {// Движение летающих платформ
        try
        {
            _slJoint.motor = motor;


            if (_slJoint.limitState == JointLimitState2D.LowerLimit)
            {
                motor.motorSpeed = motorSpeed;
            }
            else if (_slJoint.limitState == JointLimitState2D.UpperLimit)
                motor.motorSpeed = motorSpeed * -1;
        }
        catch (System.Exception)
        {

           
        }
        

    }
}
