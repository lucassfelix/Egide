using System;
using JetBrains.Annotations;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    [Header("Horizontal Movement")]
    [SerializeField]
    private float maxSpeed = 7;
    
    [SerializeField]
    private float shieldedMaxSpeed = 3;

    [SerializeField]
    private float accelerationRate = 3f;
    [SerializeField]
    private float deaccelerationRate = 1f;

    [SerializeField]
    private float shieldedAccelerationRate = 1f;
    [SerializeField]
    private float shieldedDeaccelerationRate = 3f;

    [Space(10)] 
    
    [Header("Vertical Movement")]
    [SerializeField]
    private float shieldedJumpTopVelocity = 3;
    
    [SerializeField]
    private float shieldedJumpDeaccelerationRate = 3;
    
    [SerializeField]
    private float jumpTopVelocity = 3;
    
    [SerializeField]
    private float jumpDeaccelerationRate = 3;
    
    [SerializeField]
    private bool isGrounded;

    [Header("Toggle mode")]
    
    [SerializeField]
    private bool isShielded;
    
    private const string GROUND_TAG = "Ground";
    
    private Rigidbody2D _rb;
    private GameObject _shield;
    private float _horizontalAxis, _horizontalLerpValue = 0.5f, _varAcceleration, _varDeacceleration,_varMaxSpeed, _jumpVelocity, _varJumpDeacceleration, _varJumpTopSpeed;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag(GROUND_TAG))
        {
            isGrounded = true;
        };
    }

    private void Start()
    {
         _shield = GameObject.Find("Shield");
        _rb = GetComponent<Rigidbody2D>();
        _jumpVelocity = -jumpTopVelocity;
    }

    private void HandleHorizontalMovement()
    {
        if(Input.GetKey(KeyCode.D) && _horizontalLerpValue >= 0.5)
        {

            if (isShielded)
            {
                var position = _shield.transform.localPosition;
                position = new Vector3(Mathf.Abs(position.x) , position.y,position.z);
                _shield.transform.localPosition = position;
            }
            
            
            _horizontalLerpValue = Mathf.MoveTowards(_horizontalLerpValue, 1f ,_varAcceleration * Time.deltaTime);
            
        }
        else if(Input.GetKey(KeyCode.A) && _horizontalLerpValue <= 0.5)
        {
            if (isShielded)
            {
                var position = _shield.transform.localPosition;
                position = new Vector3(-Mathf.Abs(position.x) , position.y,position.z);
                _shield.transform.localPosition = position;
            }
            
            _horizontalLerpValue = Mathf.MoveTowards(_horizontalLerpValue, 0f ,_varAcceleration * Time.deltaTime);
        }
        else
        {
            _horizontalLerpValue = Mathf.MoveTowards(_horizontalLerpValue, 0.5f ,_varDeacceleration * Time.deltaTime);
        }

        _horizontalAxis = Mathf.Lerp(-1,1,_horizontalLerpValue);
    }
    

    private void HandleVerticalMovement()
    {
        if (isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                isGrounded = false;
                _jumpVelocity = _varJumpTopSpeed;
            }
            else
            {
                _jumpVelocity = 0;
            }
        }
        else
        {
            _jumpVelocity = Mathf.MoveTowards(_jumpVelocity, -_varJumpTopSpeed, _varJumpDeacceleration * Time.deltaTime);
        }
        

    }
    private void Update()
    {
        //Changes "IsShielded" state
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isShielded = !isShielded;
        }
        //Changes velocity if shielded.
        if (isShielded)
        {
            _varAcceleration = shieldedAccelerationRate;
            _varDeacceleration = shieldedDeaccelerationRate;
            _varMaxSpeed = shieldedMaxSpeed;
            _varJumpDeacceleration = shieldedJumpDeaccelerationRate;
            _varJumpTopSpeed = shieldedJumpTopVelocity;
        }
        else
        {
            _varAcceleration = accelerationRate;
            _varDeacceleration = deaccelerationRate;
            _varMaxSpeed = maxSpeed;
            _varJumpDeacceleration = jumpDeaccelerationRate;
            _varJumpTopSpeed = jumpTopVelocity;
        }
        
        HandleVerticalMovement();
        HandleHorizontalMovement();
        
    }

    private void FixedUpdate()
    {
        _rb.velocity = new Vector2(_varMaxSpeed * _horizontalAxis , _jumpVelocity);
    }

}
