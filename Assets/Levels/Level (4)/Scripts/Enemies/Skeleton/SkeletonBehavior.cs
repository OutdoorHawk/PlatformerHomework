using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonBehavior : BasicEnemyBehavior
{
    [Header("Attacking")]
    [Space]


    [SerializeField]
    private GameObject _playerDetectCollider;

    private AlivePlayerDetect _playerDetect;
    private bool _DetectComponent;


    [SerializeField]
    private float _attackCooldownTime, _idleWaitTime;

    private float _attackStartTime, _idleWaitStartTime;




    private bool _playerDetected;

    private new void  Awake()
    {
        _currentHealth = maxHealth;
        _alive = transform.Find("Alive").gameObject;
        _aliveRb = _alive.GetComponent<Rigidbody2D>();
        _facingDirection = 1;

       

      _DetectComponent = _playerDetectCollider.TryGetComponent<AlivePlayerDetect>(out _playerDetect);
        
        _aliveAnim = _alive.GetComponent<Animator>();



     
    }


    private new void Update()
    {
        switch (currentState)
        {
            case State.Moving:
                UpdateMovingState();
                break;
            case State.Knockback:
                UpdateKnockbackState();
                break;
            case State.Attack:
                UpdateAttackState();
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

    private new void EnterMovingState()
    {
        _aliveAnim.SetBool("Idle", false);
    }
    #region Attack State

    private new void EnterAttackState()
    {
        if (Time.time > _attackCooldownTime + _attackStartTime)
        {
            if (!_playerDetect._playerDetected[1])
            {
                if (_alive.transform.localScale.x > 0)
                {
                    Flip();
                }

            }

            if (_playerDetect._playerDetected[1])
            {
                if (_alive.transform.localScale.x < 0)
                {
                    Flip();
                }

            }

            _aliveAnim.SetBool("Attack", true);
            _attackStartTime = Time.time;
        }
        else SwitchState(State.Idle);





    }

    private new void UpdateAttackState()
    {


        DetectPlayerNearby();


    }

    private new void ExitAttackState()
    {
        _aliveAnim.SetBool("Attack", false);
    }

    #endregion

    #region Moving State

    private new void UpdateMovingState()
    {
        DetectPlayerNearby();

        _groundDetected = Physics2D.Raycast(_groundCheck.position, Vector2.down, _groundCheckDistance, whatIsGround);
        _wallDetected = Physics2D.Raycast(_wallCheck.position, transform.right, _wallCheckDistance, whatIsGround);

        if (!_groundDetected || _wallDetected)
        {
            Flip();
        }
        else
        {
            _movement.Set(movementSpeed * _facingDirection, _aliveRb.velocity.y);
            _aliveRb.velocity = _movement;
        }
    }
    #endregion


    #region Idle State

    private new void EnterIdleState()
    {
        _aliveAnim.SetBool("Idle", true);

        _idleWaitStartTime = Time.time;

       

    }

    private new void  UpdateIdleState()
    {
        if (Time.time > (_idleWaitStartTime + _idleWaitTime))
        {
            SwitchState(State.Moving);
        }

        DetectPlayerNearby();

    }


    private new void ExitIdleState()
    {
        _aliveAnim.SetBool("Idle", false);

    }

    #endregion

    #region Dead State

    private new void EnterDeadState()
    {
        
                SetLayer(16);
                _aliveAnim.Play("Death");
                StartCoroutine(Destroy());
               
        

    }
    #endregion

    #region Other Functions
    private void DetectPlayerNearby()
    {

        if (_DetectComponent)
        {
            if (_playerDetect._playerDetected[0])
            {
                SwitchState(State.Attack);

            }

            if (!_playerDetect._playerDetected[0] && (Time.time > (_idleWaitStartTime + _idleWaitTime)))
            {
                SwitchState(State.Moving);

            }



        }


    }

    public new void Damage(float[] attackDetails)
    {
        _currentHealth -= attackDetails[0];

        Vector3 hitParticlesPosition = new Vector3(_alive.transform.position.x, _alive.transform.position.y + 0.4f);

        Instantiate(hitParticles, hitParticlesPosition, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));


        if (attackDetails[1] > _alive.transform.position.x)
        {
            _damageDirection = -1;
        }
        else
            _damageDirection = 1;

        

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

    private new void SwitchState(State state)
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

    #endregion

}
