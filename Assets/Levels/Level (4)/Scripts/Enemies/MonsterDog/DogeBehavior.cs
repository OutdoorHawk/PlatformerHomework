using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogeBehavior : BasicEnemyBehavior
{

    [SerializeField] protected GameObject deathChunkParicles;

    [SerializeField]
    private float _idleWaitTime;



    private float _idleWaitStartTime;


   

    protected new void Update()
    {
        switch (currentState)
        {
            case State.Moving:
                UpdateMovingState();
                break;
            case State.Knockback:
                UpdateKnockbackState();
                break;

            case State.Idle:
                UpdateIdleState();
                break;
            case State.Dead:
                UpdateDeadState();
                break;
        }
        ShowHealth();


    }

    private new void EnterDeadState()
    {
      
        Instantiate(deathChunkParicles, _alive.transform.position, deathChunkParicles.transform.rotation);
        Destroy(gameObject);


    }


    private new void UpdateMovingState()
    {
       
        _groundDetected = Physics2D.Raycast(_groundCheck.position, Vector2.down, _groundCheckDistance, whatIsGround);
        _wallDetected = Physics2D.Raycast(_wallCheck.position, transform.right, _wallCheckDistance, whatIsGround);

        if (!_groundDetected || _wallDetected)
        {
            SwitchState(State.Idle);
            
        }
        else
        {
            _movement.Set(movementSpeed * _facingDirection, _aliveRb.velocity.y);
            _aliveRb.velocity = _movement;
        }
    }


    #region Idle State

    private new void EnterIdleState()
    {
        _aliveAnim.SetBool("Idle", true);
        _idleWaitStartTime = Time.time;

    }
    
    private new void UpdateIdleState()
    {
        if (Time.time > (_idleWaitStartTime + _idleWaitTime))
        {
            Flip();
            SwitchState(State.Moving);
        }


    }

    private new void ExitIdleState()
    {
        _aliveAnim.SetBool("Idle", false);

    }
    #endregion
    public new void Damage(float[] attackDetails)
    {
        _currentHealth -= attackDetails[0];



        Instantiate(hitParticles, _alive.transform.position, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));


        if (attackDetails[1] > _alive.transform.position.x)
        {
            _damageDirection = -1;
        }
        else
            _damageDirection = 1;

        //Hit particle

        if (_currentHealth > 0.0f)
        {
            SwitchState(State.Knockback);
        }
        else if (_currentHealth < 0.0f)
        {
            SwitchState(State.Dead);
        }

        _healthShowStartTime = Time.time;
    }

    protected new void SwitchState(State state)
    {
        switch (currentState)
        {
            case State.Moving:
                ExitMovingState();
                break;

            case State.Knockback:
                ExitKnockbackState();
                break;

            case State.Attack:
                ExitAttackState();
                break;

            case State.Idle:
                ExitIdleState();
                break;

            case State.Dead:
                ExitDeadState();
                break;

        }

        switch (state)
        {
            case State.Moving:
                EnterMovingState();
                break;

            case State.Knockback:
                EnterKnockbackState();
                break;

            case State.Attack:
                EnterAttackState();
                break;

            case State.Idle:
                EnterIdleState();
                break;

            case State.Dead:
                EnterDeadState();
                break;

        }

        currentState = state;
    }
   

}
