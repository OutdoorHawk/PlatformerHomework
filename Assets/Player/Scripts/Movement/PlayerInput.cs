using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cinemachine;



namespace Platformer.Inputs
{
    [RequireComponent(typeof(CharacterController2D))]
    [RequireComponent(typeof(Shooter))]

    public class PlayerInput : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float charSpeed;
        [SerializeField] private bool dashActive;
        private bool _jump;
        private float _horizontal;
        


        [Header("Utility")]
        [SerializeField] private CinemachineVirtualCamera cm;
        [SerializeField] private Animator uiAnim;
        [SerializeField] private Text pressEText;
        private Animator _anim;
        private CharacterController2D _playerMovement;
        private ShowHintScript _hintScript;
        private bool _canInteract;

        

        [Header("Combat")]
        [SerializeField] private float startTimeBtwAttack = 1f;
        private PlayerCombatController _playerCombat;
      
        private float _timeBtwAttack;
        private bool gotInput, isAttacking, isFirstAttack;
        private float inputTimer = 1f;

        private float lastInputTime = Mathf.NegativeInfinity;



        public bool _isGroundBowShoot = false;






        
        private Scene _currentScene;


        private void Awake()
        {
            _playerMovement = GetComponent<CharacterController2D>();
            _playerCombat = GetComponent<PlayerCombatController>();
            _anim = GetComponent<Animator>();
           
           
            _timeBtwAttack = startTimeBtwAttack;

       
        }

        private void Start()
        {
            
        }

        void Update()
        {


            _horizontal = Input.GetAxis(GlobalStringVars.HORIZONTAL_AXIS) * charSpeed;


            if (Input.GetKeyDown(KeyCode.Space))
            {
                _jump = true;

                _playerMovement.DoubleJump();
            }

           

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {

                



                    gotInput = true;
                    lastInputTime = Time.time;




                    if (gotInput)
                    {

                    _playerCombat.GroundBowShot();


                    }

                    if (Time.time >= lastInputTime + inputTimer)
                    {
                        //Wait for new input
                        gotInput = false;
                    _anim.SetBool("BowShot", false);


                }

                  

                
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (_canInteract)
                {
                    pressEText.gameObject.SetActive(false);

                    _hintScript.ShowHint();

                }

            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                if (dashActive)
                {
                    _playerMovement.AttemptToDash();
                }
                
            }


            _playerMovement.Move(_horizontal * Time.fixedDeltaTime, false, _jump);



        }

        private void FixedUpdate()
        {


            
            _jump = false;


        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("CamCollider"))
            {
                uiAnim.Play("ScreenShading");
                cm.Follow = null;


            }
            if (collision.gameObject.CompareTag("RestartTrigger"))
            {



            }

            if (collision.gameObject.CompareTag("Hint"))
            {
                pressEText.gameObject.SetActive(true);
                _canInteract = true;
                _hintScript = collision.GetComponent<ShowHintScript>();


            }

        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Hint"))
            {
                _canInteract = false;
                pressEText.gameObject.SetActive(false);
                _hintScript = collision.GetComponent<ShowHintScript>();
                _hintScript.EndHint();

            }

        }


            private void BowShotEnded()
        {
           
            _anim.SetBool("isAttacking", false);

           
        }

       
    }
}

