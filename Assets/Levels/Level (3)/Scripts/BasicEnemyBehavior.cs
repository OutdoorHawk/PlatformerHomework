using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicEnemyBehavior : MonoBehaviour
{
    protected enum State
    {
        Moving,
        Knockback,
        Attack,
        Idle,
        Dead
    }

    protected State currentState;

    [Header("Moving and knockback")]
    [Space]
    [SerializeField] protected Transform _groundCheck, _wallCheck;
    [SerializeField] protected float _groundCheckDistance, _wallCheckDistance;
    [SerializeField] protected float movementSpeed;

    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] protected float knockbackDuration;
    [SerializeField] protected Vector2 knockbackSpeed;
    [SerializeField] protected GameObject hitParticles;

    [Header("Health")]
    [Space]

    [SerializeField] protected float maxHealth;
    [SerializeField] protected GameObject _enemyUI;
    [SerializeField] protected Image _healthBar;

    [SerializeField] protected float _healthShowTime;



    protected float _healthShowStartTime = -100;

    protected GameObject _alive;

    protected Rigidbody2D _aliveRb;
    protected Animator _aliveAnim;

    protected Vector2 _movement;

    protected int _facingDirection;
    protected int _damageDirection;

    protected float _currentHealth;
    protected float _knockbackStartTime;

    protected bool
        _groundDetected,
        _wallDetected;






    protected virtual void Awake()
    {
        _currentHealth = maxHealth;
        _alive = transform.Find("Alive").gameObject;
        _aliveRb = _alive.GetComponent<Rigidbody2D>();
        _facingDirection = 1;


        _aliveAnim = _alive.GetComponent<Animator>();

    }

    protected void Start()
    {

    }
    protected void Update()
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

    //--WALKING STATE-------------------------------------------------------------------------------------------
    #region Waling State
    protected void EnterMovingState()
    {

    }

    protected void UpdateMovingState()
    {

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

    protected void ExitMovingState()
    {

    }

    #endregion
    //--KNOCKBACK STATE-------------------------------------------------------------------------------------------
    #region Knockback State
    protected void EnterKnockbackState()
    {
        SetLayer(16);
        _knockbackStartTime = Time.time;
        _movement.Set(knockbackSpeed.x * _damageDirection, knockbackSpeed.y);
        _aliveRb.velocity = _movement;
        _aliveAnim.SetBool("Knockback", true);
    }

    protected void UpdateKnockbackState()
    {
        if (Time.time >= _knockbackStartTime + knockbackDuration)
        {
            SwitchState(State.Moving);
        }
    }

    protected void ExitKnockbackState()
    {
        SetLayer(14);
        _aliveAnim.SetBool("Knockback", false);

        if (_damageDirection == -1 && _facingDirection < 0)
        {
            Flip();
        }
        else if (_damageDirection == 1 && _facingDirection > 0)
        {
            Flip();
        }
    }
    #endregion
    //--KNOCKBACK STATE-------------------------------------------------------------------------------------------

    //--ATTACK STATE-------------------------------------------------------------------------------------------
    #region Attack State
    protected void EnterAttackState()
    {






    }

    protected void UpdateAttackState()
    {



    }

    protected void ExitAttackState()
    {

    }
    #endregion
    //--ATTACK STATE-------------------------------------------------------------------------------------------

    //--IDLE STATE-------------------------------------------------------------------------------------------
    #region Idle State

    protected void EnterIdleState()
    {
        _aliveAnim.SetBool("Idle", true);


    }

    protected void UpdateIdleState()
    {



    }

    protected void ExitIdleState()
    {
        _aliveAnim.SetBool("Idle", false);

    }
    #endregion
    //--IDLE STATE-------------------------------------------------------------------------------------------


    //--DEAD STATE-------------------------------------------------------------------------------------------
    #region Dead State
    protected void EnterDeadState()
    {

        Destroy(gameObject);


    }

    protected void UpdateDeadState()
    {

    }

    protected void ExitDeadState()
    {

    }
    #endregion

    //--OTHER FUNCTIONS------------------------------------------------------------------------------------------




    public void Damage(float[] attackDetails)
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
    protected void Flip()
    {
        Vector3 theScale = _alive.transform.localScale;
        theScale.x *= -1;

        _facingDirection *= -1;
        _alive.transform.localScale = theScale;



    }





    protected void SwitchState(State state)
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


    protected IEnumerator Destroy()
    {
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }

    protected void SetLayer(int layer)
    {
        gameObject.layer = layer;
    }

    protected void ShowHealth()
    {
        _healthBar.fillAmount = _currentHealth / maxHealth;

        if (Time.time < _healthShowTime + _healthShowStartTime)
        {
            _enemyUI.gameObject.SetActive(true);

        }
        else
        {
            _enemyUI.gameObject.SetActive(false);
        }

        if (_alive.transform.localScale.x < 0)
        {
            _healthBar.fillOrigin = 1;
        }
        else
            _healthBar.fillOrigin = 0;

    }

    protected void OnDrawGizmos()
    {
        Gizmos.DrawLine(_groundCheck.position, new Vector2(_groundCheck.position.x, _groundCheck.position.y - _groundCheckDistance));
        Gizmos.DrawLine(_wallCheck.position, new Vector2(_wallCheck.position.x + _wallCheckDistance, _wallCheck.position.y));


    }
}
