using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]

public class CharacterController2D : MonoBehaviour
{
   

    [SerializeField] private float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
    [SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
    [SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] private LayerMask m_WhatIsWall;                            // A mask determining what is wall to the character
    [SerializeField] private Transform m_GroundCheck;                            // A position marking where to check if the player is grounded.
    [SerializeField] private Transform m_CeilingCheck;                           // A position marking where to check for ceilings

    [SerializeField] private Transform m_WallCheckUp;                           // A positions marking where to check if the player on Wall.
    [SerializeField] private Transform m_WallCheckDown;

  

    [SerializeField] private float _attackDashSpeed;
    [SerializeField] private bool _blockMoveInput;
    [SerializeField] private bool _doubleJumpOn;

   
    public Rigidbody2D m_Rigidbody2D;
    private Rigidbody2D parent_Rigidbody2D;
    private Animator _anim;
    private SpriteRenderer _playerSprite;

    private float m_GroundedRadius = 0.2f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;            // Whether or not the player is grounded.
    const float k_CeilingRadius = 0.2f; // Radius of the overlap circle to determine if the player can stand up
    private float _wallCheckRadiusUp; //������ ����� �������� �� �����
    private float _wallCheckRadiusDown;


    public bool _blockMoveX;
    private bool m_FacingRight = true;  // For determining which way the player is currently facing.
    public bool _currentFacing;
   
    private float _gravityDef;
    public Vector2 _moveVector;
    private Vector3 m_Velocity = Vector3.zero;

    private bool _onWall; //���������� ��� ����������� ����, ��������� �� �������� �� �����
    private bool _jump; //������������ ���������� ��� ������, �.�. �������� �������� ������ � ������� Move ����� PlayerInput
   
    private bool _onPlatform = false; //������� ��������� �� �������� �� ���������, ��� �������� �������� ��������� ���������
    private float _platSpeed; // �������� �������� ���������, ������� ���� ��������� � �������� ���������

    private bool _secondJump;

    [Header("PlayerKnockback")]
    [Space]

    [SerializeField] private float knockbackDuration;
    [SerializeField] private Vector2 knockbackSpeed;
    private PlayerHealth _playerHealth;

    private Vector2 _knockBackDirection;

    private int _facingDirection;
    private int _damageDirection;

    private float _knockbackStartTime;

    [Header("PlayerDash")]
    [Space]
  
    [SerializeField] private float dashTime;
    [SerializeField] private float _dashSpeed;
    [SerializeField] private float _distanceBetweenImages;
    [SerializeField] private float _dashCooldown;

    public bool _isDashing;
    private float _dashTimeLeft;
    private float _lastImageXpos;
    private float _lastDashTime = -100;
    private bool _airBowShot;


    private void Awake()
    {
        _playerSprite = GetComponent<SpriteRenderer>();
         _currentFacing = m_FacingRight;

        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        _gravityDef = m_Rigidbody2D.gravityScale;


        
        _anim = GetComponent<Animator>();

        m_GroundedRadius = m_GroundCheck.GetComponent<CircleCollider2D>().radius;
        _wallCheckRadiusUp = m_WallCheckUp.GetComponent<CircleCollider2D>().radius;
        _wallCheckRadiusDown = m_WallCheckDown.GetComponent<CircleCollider2D>().radius;

        _playerHealth = GetComponent<PlayerHealth>();

        _blockMoveInput = false;

    }
    
    private void FixedUpdate()
    {
        WallJump();// �������� ������ �� �����
        CheckingWall(); // �������� �� ����� ����� ��� ���
        CheckingGround(); // �������� �� ���������� �� �����
        OnWall(); //�������� �� �����
        OnPlatform();// �������� �� ���������� �� ���������


      


    }
    private void Update()
    {


        UpdateKnockbackState();

    }
    private void LateUpdate()
    {
       
        CheckDash();
       
    }




    public void Move(float move, bool crouch, bool jump)
    {
        _jump = jump;
        #region RunAnimations
        //Run Animation bools
        if (move == 0)
        {
            _anim.SetBool("isRunning", false);
        }
        else
        {
            _anim.SetBool("isRunning", true);
        }
        #endregion
       
        if (!_blockMoveX && _playerHealth._isAlive)
        {

            //only control the player if grounded or airControl is turned on
            if (m_Grounded || m_AirControl)
            {



                // Move the character by finding the target velocity
                Vector3 targetVelocity = new Vector2((move * 10f) + _platSpeed, m_Rigidbody2D.velocity.y);

                if (_onWall)
                {
                    targetVelocity = _moveVector;
                }

                if (!_blockMoveInput)
                {
                    m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
                }
             
                // And then smoothing it out and applying it to the character


                // If the input is moving the player right and the player is facing left...
                if (move > 0 && !m_FacingRight)
                {
                    // ... flip the player.
                    Flip();
                }
                // Otherwise if the input is moving the player left and the player is facing right...
                else if (move < 0 && m_FacingRight)
                {
                    // ... flip the player.
                    Flip();
                }
            }
            // If the player should jump...
            if (m_Grounded && jump || _stillGrounded && jump)
            {
                _anim.ResetTrigger("Landing");
                _anim.Play("Jump");
                _anim.SetBool("withSword", false);


                m_Grounded = false;
                _stillGrounded = false;
                _anim.SetBool("StillGrounded", false);


                // Add a vertical force to the player.
                if (_stillGrounded)
                {
                    m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce + 150));
                }
                else
                    m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));


            }
           
            

            if (m_Grounded && m_Rigidbody2D.velocity.y == 0)
            {
                _anim.ResetTrigger("Jump");
                _anim.SetTrigger("Landing"); //Landing Anim Trigger
                _anim.ResetTrigger("DoubleJump");
                _stillGrounded = true;
                _anim.SetBool("StillGrounded", true);
                _secondJump = false;

            }

        }
       
          




    }

    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        _currentFacing = m_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;

       // _playerSprite.flipX = !_playerSprite.flipX;
        
    }

    private float _jumpTimer = 0; // ������ ��� ������� ������� ����� ����� � ���������, � ������� �������� ����� ��������� ������
    public float _jumpTime;
    public bool _stillGrounded;
    private void CheckingGround()
    {
        m_Grounded = (Physics2D.OverlapCircle(m_GroundCheck.position, m_GroundedRadius, m_WhatIsGround));
       
        if (m_Grounded)
        {
            
           
        }
        else
        {
            
            if ((_jumpTimer += Time.deltaTime) >= _jumpTime)
            {
              
                _anim.SetBool("StillGrounded", false);
                _stillGrounded = false;
                _jumpTimer = 0;
              


            }
        }

        _anim.SetBool("OnGround", m_Grounded);
        

        
        
          
        




    }

    private void CheckingWall()
    {
        _onWall = (Physics2D.OverlapCircle(m_WallCheckUp.position, _wallCheckRadiusUp, m_WhatIsWall) && Physics2D.OverlapCircle(m_WallCheckDown.position, _wallCheckRadiusDown, m_WhatIsWall));

        _anim.SetBool("OnWall", _onWall);




    }

    public float jumpWallTime = 0.5f; // time passed since jump button was pressed
    private float jumpWallBlockTimer;
    private float onWallGravTimer;
   
    public Vector2 jumpAngle = new Vector2(3.5f, 10);

    private void OnWall()
    {
        if (_onWall && !m_Grounded)
        {
            m_Rigidbody2D.gravityScale = _gravityDef;
            _moveVector.x = 0;
            _moveVector.y = 0;

            if ((onWallGravTimer += Time.deltaTime) >= jumpWallTime)
            {

                m_Rigidbody2D.gravityScale = _gravityDef;

                onWallGravTimer = 0;




            }
        }
        else if (!_onWall && !m_Grounded)
        {
            m_Rigidbody2D.gravityScale = _gravityDef;

        }

       
    }
    private void WallJump()
    {
        
        if (!m_Grounded && _onWall && _jump)
        {
            jumpWallBlockTimer = 0;
           
            m_AirControl = false;
            Flip();
            _moveVector.x = 0;
            m_Rigidbody2D.gravityScale = _gravityDef;
           
            m_Rigidbody2D.velocity = new Vector2(0, 0);

            m_Rigidbody2D.velocity = new Vector2(transform.localScale.x * jumpAngle.x, jumpAngle.y);
            
        }
        if (!m_AirControl)
        {
            if (m_Grounded)
            {
                m_AirControl = true;
              
                jumpWallBlockTimer = 0;
            }

        }

    }

    [SerializeField] private Transform dopPosition;
  

    public void LedgeGo()
    {
        transform.position = new Vector3(dopPosition.position.x, dopPosition.position.y, transform.position.z);
    }
    public void LedgeOver()
    {
        m_Rigidbody2D.gravityScale = _gravityDef;
    }
    public void StartAnimLedge()
    {
        if (_playerHealth._isAlive)
        {
            _blockMoveX = true;
            m_Rigidbody2D.gravityScale = 0;
            m_Rigidbody2D.velocity = Vector2.zero;
            _anim.Play("LedgeClimb");
        }
       
       

    }

    public void OnPlatform()
    {
        if (_onPlatform)
        {
            _platSpeed = parent_Rigidbody2D.velocity.x;
        }
        else
            _platSpeed = 0;
    }


    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.CompareTag("Platform"))
        {

            parent_Rigidbody2D = coll.gameObject.GetComponent<Rigidbody2D>();

            
            _onPlatform = true;   

        }
        if (coll.gameObject.CompareTag("Wagon"))
        {

            parent_Rigidbody2D = coll.gameObject.GetComponent<Rigidbody2D>();

            m_JumpForce = m_JumpForce + 70;
            _onPlatform = true;

        }

        if (coll.gameObject.CompareTag("Dog") && _playerHealth._isAlive)
        {
          

            //if (coll.transform.position.x > transform.position.x)
            //{
            //    _damageDirection = -1;
            //}
            //else
            //    _damageDirection = 1;

            //PlayerKnockback();

            
           


        }


    }
    void OnCollisionExit2D(Collision2D coll)
    {
        if (coll.gameObject.CompareTag("Platform"))
        {
            m_Grounded = false;
          
            _onPlatform = false;

        }
        if (coll.gameObject.CompareTag("Wagon"))
        {
            m_Grounded = false;
            m_JumpForce = m_JumpForce - 70;
            _onPlatform = false;

        }

    }

  
    private void AttackDash()
    {
        
        Vector3 targetVelocity;

        if (m_FacingRight)
        {
            targetVelocity = new Vector2(1, 0);
        }
        else
        {
            targetVelocity = new Vector2(-1, 0);
        }
            m_Rigidbody2D.velocity = targetVelocity * _attackDashSpeed;

        //m_Rigidbody2D.AddForce(targetVelocity, ForceMode2D.Impulse);
     
    }
    


    public void PlayerKnockback(int damageDirection)
    {
      
            _anim.SetBool("Knockback", true);
            _anim.Play("TakeHit");
            _knockbackStartTime = Time.time;
        _damageDirection = damageDirection;

    }

    private void UpdateKnockbackState()
    {
        if (_anim.GetBool("Knockback"))
        {
            _blockMoveInput = true;

            _knockBackDirection.Set(knockbackSpeed.x * _damageDirection, m_Rigidbody2D.velocity.y);

            m_Rigidbody2D.velocity = _knockBackDirection;
        }

        if (Time.time >= (_knockbackStartTime + knockbackDuration))
        {
            
            _anim.SetBool("Knockback", false);
            _blockMoveInput = false;

        }
       
    }

    private void PlayerStop()
    {
       
        m_Rigidbody2D.velocity = Vector2.zero;
       
        
    }

    public void FreezeInAir()
    {

        m_Rigidbody2D.velocity =  new Vector2 (0, 2);


    }

    private void SetLayer(int layer)
    {
        gameObject.layer = layer;
    }


    private void CheckDash()
    {
        if (_isDashing)
        {
            if (_dashTimeLeft>0)
            {

                gameObject.layer = 16;
                _blockMoveInput = true;

                int playerFacing;

                if (m_FacingRight)
                {
                    playerFacing = 1;
                }
                else
                    playerFacing = -1;

                if (!_airBowShot && !_secondJump)
                {

                    m_Rigidbody2D.velocity = new Vector2(_dashSpeed * playerFacing, m_Rigidbody2D.velocity.y);

                }
                else if (_secondJump)
                {

                }
                else
                {

                    m_Rigidbody2D.velocity = new Vector2(playerFacing, 0.5f);
                   
                }
                   

                _dashTimeLeft -= Time.deltaTime;

                if (Math.Abs(transform.position.x - _lastImageXpos) > _distanceBetweenImages)
                {
                    PlayerAfterImagePool.Instance.GetFromPool();
                    _lastImageXpos = transform.position.x;
                }
            }
        }
        if (_dashTimeLeft <=0 || _onWall)
        {
            _isDashing = false;
            _blockMoveInput = false;
            gameObject.layer = 3;
            _airBowShot = false;
        }
    }

    public void AttemptToDash()
    {
        if (Time.time >= (_lastDashTime + _dashCooldown))
        {
            _isDashing = true;
            _dashTimeLeft = dashTime;
            _lastDashTime = Time.time;
            _airBowShot = false;
            //_anim.Play("Run");

            PlayerAfterImagePool.Instance.GetFromPool();
            _lastImageXpos = transform.position.x;
        }
    }

    public void PhaseBowShot()
    {
        

            _airBowShot = true;
            _isDashing = true;
            _dashTimeLeft = dashTime;
            _lastDashTime = Time.time;
            //_anim.Play("Run");

            PlayerAfterImagePool.Instance.GetFromPool();
            _lastImageXpos = transform.position.x;
        
    }

    public void DoubleJump()
    {
        if (!m_Grounded && !_secondJump && !_stillGrounded && _doubleJumpOn)
        {
            _anim.SetTrigger("DoubleJump");
            _secondJump = true;
            PlayerStop();
            _isDashing = true;
            _dashTimeLeft = dashTime;
            _lastDashTime = Time.time;

            PlayerAfterImagePool.Instance.GetFromPool();
            _lastImageXpos = transform.position.x;

            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
        }
    }

}