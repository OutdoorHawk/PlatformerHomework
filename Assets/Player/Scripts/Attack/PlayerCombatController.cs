using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Shooter))]
[RequireComponent(typeof(CharacterController2D))]

public class PlayerCombatController : MonoBehaviour
{

    public bool combatEnabled;
    [SerializeField]
    private float inputTimer, attack1Radius, attack1Damage;
    [SerializeField]
    private Transform attack1HitBoxPos;
    [SerializeField]
    private Transform jumpAttackHitBoxPos;
    [SerializeField]
    private LayerMask whatIsDamageable;
    [SerializeField]
    private float _lastAttackTimer, _lastShotTimer;
    [SerializeField]
    private bool _comboInput;
    private bool _continueCombo;
    [SerializeField]
    private float _attackCooldownTime;

    private bool gotInput, isAttacking, isFirstAttack;

    private float lastInputTime = Mathf.NegativeInfinity;
    private float _lastAttackTime, _lastShotTime;

    private float _damageIncrease;

    private bool _bowShot;

    private float[] attackDetails = new float[2];

    private Animator anim;
    private Rigidbody2D rb;
    private CharacterController2D _playerMovement;
    private Shooter _shooter;

    private int _attackCounter = 0;




    private void Awake()
    {
        _shooter = GetComponent<Shooter>();
        _playerMovement = GetComponent<CharacterController2D>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {

        anim.SetBool("canAttack", combatEnabled);
    }

    private void Update()
    {
        CheckCombatInput();
        CheckAttacks();
    }

    private void LateUpdate()
    {
        CheckBowShot();
    }

    private void CheckCombatInput()
    {

        if (Input.GetMouseButtonDown(0))
        {


            if (!anim.GetBool("withSword") && anim.GetBool("OnGround")) // Достать мечь перед атакой, если он убран
            {
                _lastAttackTime = _lastAttackTimer;
                anim.SetBool("withSword", true);
               
            }
            else
            {
                if (combatEnabled)
                {
                   
                    gotInput = true;
                    lastInputTime = Time.time;
                }
            }






        }


    }

    private void CheckAttacks()
    {
        if (gotInput)
        {
            //Perform Attack1
            if (!isAttacking && anim.GetBool("OnGround"))
            {
                
                gotInput = false;
                isAttacking = true;
                isFirstAttack = !isFirstAttack;



                _attackCounter++;
                anim.SetInteger("attackCounter", _attackCounter);
                anim.SetBool("firstAttack", isFirstAttack);
                anim.SetBool("isAttacking", isAttacking);

                
                if (_comboInput)
                {

                    _continueCombo = true;

                }
                _lastAttackTime = _lastAttackTimer;

            }
            if (!isAttacking && !anim.GetBool("OnGround"))
            {
                anim.SetBool("jumpAttack", true);
                anim.SetBool("isAttacking", isAttacking);
            }
        }

        if (Time.time >= lastInputTime + inputTimer)
        {
            //Wait for new input
            gotInput = false;
            isFirstAttack = false;
            isAttacking = false;
            _attackCounter = 0;
            _continueCombo = false;
            _damageIncrease = 0;
            ReturnSwordTimer();

            anim.SetBool("jumpAttack", false);


        }
    }
    
    private void CheckAttackHitBox()
    {
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attack1HitBoxPos.position, attack1Radius, whatIsDamageable);

        attackDetails[0] = attack1Damage + _damageIncrease;
        attackDetails[1] = transform.position.x;

        foreach (Collider2D collider in detectedObjects)
        {
            try
            {
                if (collider.CompareTag("Enemy"))
                {
                    collider.transform.parent.SendMessage("Damage", attackDetails);

                }
            }
            catch (System.Exception)
            {

                throw;
            }




           
        }
    }

    private void CheckJumpAttackHitBox()
    {
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(jumpAttackHitBoxPos.position, attack1Radius, whatIsDamageable);

        attackDetails[0] = attack1Damage;
        attackDetails[1] = transform.position.x;

        foreach (Collider2D collider in detectedObjects)
        {
            

            if (collider.CompareTag("Enemy"))
            {
                collider.transform.parent.SendMessage("Damage", attackDetails);
                
            }



          
        }
    }



    private void FinishAttack1()
    {

        
        anim.SetBool("isAttacking", isAttacking);

        anim.SetBool("jumpAttack", false);


        if (_continueCombo)
        {
            combatEnabled = true;
            gotInput = true;
            lastInputTime = Time.time;
            _attackCounter++;

            _damageIncrease += 10;


        }
        else
            lastInputTime = 0;



        anim.SetInteger("attackCounter", _attackCounter);
       
    }

    private void FinishLastAttack()
    {
        _attackCounter = 0;
        _damageIncrease = 0;
        anim.SetBool("isAttacking", isAttacking);
        anim.SetInteger("attackCounter", _attackCounter);
        anim.SetBool("jumpAttack", false);
        gotInput = false;
    }

    public void GroundBowShot()
    {
        if (Time.time > _lastShotTime + _lastShotTimer)
        {
           
            _lastShotTime = Time.time;

            anim.SetTrigger("BowShot");

            _shooter.SetDirection(_playerMovement._currentFacing);

            combatEnabled = false;

            if (!anim.GetBool("OnGround") && !_bowShot)
            {
                _bowShot = true;
                _playerMovement.PhaseBowShot();
                
            }

          

           

        }
    }

    public void CheckBowShot()
    {
        if (anim.GetBool("OnGround"))
        {
            _bowShot = false;
           
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attack1HitBoxPos.position, attack1Radius);
    }



    private void ReturnSwordTimer()
    {

        if (anim.GetBool("withSword"))
        {
            _lastAttackTime -= Time.deltaTime;
        }
       
            

        if (anim.GetBool("isRunning"))
        {
            _lastAttackTime = _lastAttackTimer;
        }
        if (_lastAttackTime <= -1)
        {
           
           anim.SetBool("withSword", false);
        }

    }
    private void AttackCooldownStart()
    {
        if (!_continueCombo)
        {
            anim.SetInteger("attackCounter", 0);
            gotInput = false;
            combatEnabled = false;
            StartCoroutine(AttackCooldown());
        }
      
    }
    private IEnumerator AttackCooldown()
    {


        yield return new WaitForSeconds(_attackCooldownTime);
        combatEnabled = true;
    }

}
