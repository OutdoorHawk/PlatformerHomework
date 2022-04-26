using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(CharacterController2D))]
public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float _maxHealth;
    [SerializeField] private Image _healthBar;
    [SerializeField] private Animator _DeadUIAnim;

    private CharacterController2D _playerMovement;
  
    public bool _isAlive;
    private Animator _anim;

    
    private float _currentHealth;
    private int _damageDirection;


    private void Update()
    {
        
        _healthBar.fillAmount = _currentHealth / _maxHealth;
    }
    private void Awake()
    {
        _playerMovement = GetComponent<CharacterController2D>();
           _currentHealth = _maxHealth;
        _isAlive = true;
        _anim = GetComponent<Animator>();
    }

   

    public void Damage(float[] attackDetails)
    {
        _currentHealth -= attackDetails[0];


      
        if (attackDetails[1] > transform.position.x)
        {
            _damageDirection = -1;
        }
        else
            _damageDirection = 1;

        //Hit particle

        if (_currentHealth > 0.0f)
        {
            _anim.SetTrigger("TakeHit");

            CheckIsAlive();
        }
        else if (_currentHealth < 0.0f)
        {
            CheckIsAlive();
        }

        _playerMovement.PlayerKnockback(_damageDirection);

    }

    private void CheckIsAlive()
    {
        if (_currentHealth > 0)
            _isAlive = true;
        else
        {
            _isAlive = false;
            Death();
        }
           
    }

    private void Death()
    {
        gameObject.layer = 16;
        _anim.SetTrigger("Death");
        _DeadUIAnim.gameObject.SetActive(true);


    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

   
}
