using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [Header("Horizontal Movement")] [SerializeField]
    private float maxSpeed = 7;

    [SerializeField] private float shieldedMaxSpeed = 3;

    [SerializeField] private float accelerationRate = 3f;

    [SerializeField] private float deaccelerationRate = 1f;

    [SerializeField] private float shieldedAccelerationRate = 1f;

    [SerializeField] private float shieldedDeaccelerationRate = 3f;

    [Space(10)] [Header("Vertical Movement")] [SerializeField]
    private float shieldedJumpTopVelocity = 3;

    [SerializeField] private float shieldedJumpDeaccelerationRate = 3;

    [SerializeField] private float jumpTopVelocity = 3;

    [SerializeField] private float shieldJumpVelocity = 3;
    
    [SerializeField] private float jumpDeaccelerationRate = 3;

    [SerializeField] private LayerMask groundLayerMask = 0;
    [SerializeField] private LayerMask shieldLayerMask = 0;
    [SerializeField] private LayerMask enemyLayerMask = 0;

    


    private Collider2D _collider2D;
    private bool _isShielded;
    private bool _canKill;
    private bool _onTopOfShield;
    private bool _onTopOfEnemy;
    private float _timeElapsed;

    private float _horizontalAxis,
        _horizontalLerpValue = 0.5f,
        _varAcceleration,
        _varDeacceleration,
        _varMaxSpeed,
        _jumpVelocity,
        _varJumpDeacceleration,
        _varJumpTopSpeed;

    private Rigidbody2D _rb;
    private Animator _playerAnimator;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider2D = GetComponent<Collider2D>();
        _jumpVelocity = -jumpTopVelocity;
        _isShielded = true;
        _playerAnimator = GetComponent<Animator>();
        _canKill = false;
        _timeElapsed = 0;
    }

    public void SetShielded(bool setIsShielded)
    { 
        _playerAnimator.SetBool("isShielded",setIsShielded);
        _isShielded = setIsShielded;
    }

    public bool GetShielded()
    {
        return _isShielded;
    }

    private void Update()
    {

        _timeElapsed += Time.deltaTime;
        
        if (transform.position.x < -15f || transform.position.y < -8f)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        
        //Changes velocity if shielded.
        if (_isShielded)
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
        _rb.velocity = new Vector2(_varMaxSpeed * _horizontalAxis,  _jumpVelocity);
    }

    private bool IsGrounded()
    {
        var bounds = _collider2D.bounds;
        const float extraHeight = .2f;
        var raycastHitGround =
            Physics2D.BoxCast(bounds.center,bounds.size, 0, Vector2.down,  extraHeight, groundLayerMask);
        
        var hitGround = raycastHitGround.collider != null;

        return hitGround;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Shield") && Input.GetKey(KeyCode.W) && _onTopOfShield)
        {
            _jumpVelocity = shieldJumpVelocity;
            _canKill = true;
        }
        else if(other.gameObject.CompareTag("Enemy"))
        {
            if(!_isShielded)
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            else if (_canKill && _onTopOfEnemy)
            {
                Destroy(other.gameObject);
                _canKill = false;
            }
        }else if (other.gameObject.CompareTag("Ground"))
        {
            _canKill = false;
        }else if (other.gameObject.CompareTag("End"))
        {
            Debug.Log("Tempo: " + Mathf.FloorToInt(_timeElapsed%60) + " segundos.");
            other.collider.enabled = false;
        }
    }
    

    private bool CheckRoof()
    {
        var bounds = _collider2D.bounds;
        var raycastHit =
            Physics2D.BoxCast(bounds.center + (Vector3.up * bounds.extents.y), Vector2.one/5f , 0, Vector2.up,  0,groundLayerMask);

        return raycastHit.collider != null;
    }

    public void SetCanKill(bool canKill)
    {
        _canKill = canKill;
    }
    
    private void HandleHorizontalMovement()
    {
        if (Input.GetKey(KeyCode.D) && _horizontalLerpValue >= 0.5)
        {
            //Change player orientation
            transform.rotation = Quaternion.identity;
            _horizontalLerpValue = Mathf.MoveTowards(_horizontalLerpValue, 1f, _varAcceleration * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.A) && _horizontalLerpValue <= 0.5)
        {
           //Change shield position
           transform.rotation = new Quaternion(0f,-180f,0f,0);
           _horizontalLerpValue = Mathf.MoveTowards(_horizontalLerpValue, 0f, _varAcceleration * Time.deltaTime);
        }
        else
        {
            _horizontalLerpValue = Mathf.MoveTowards(_horizontalLerpValue, 0.5f, _varDeacceleration * Time.deltaTime);
        }

        _horizontalAxis = Mathf.Lerp(-1, 1, _horizontalLerpValue);

        _playerAnimator.SetBool("isWalking", _horizontalAxis != 0);
    }
    
    private bool OnTopOfEnemy()
    {
        var bounds = _collider2D.bounds;
        const float extraHeight = .2f;
        var raycastHitEnemy =
            Physics2D.BoxCast(bounds.center - new Vector3(0,bounds.extents.y,0), bounds.size, 0, Vector2.down,  extraHeight, enemyLayerMask);
        
        return raycastHitEnemy.collider != null;
    }

    private bool OnTopOfShield()
    {
        var bounds = _collider2D.bounds;
        const float extraHeight = .2f;
        var raycastHitShield =
            Physics2D.BoxCast(bounds.center - new Vector3(0,bounds.extents.y,0), bounds.size, 0, Vector2.down,  extraHeight, shieldLayerMask);
        
        return raycastHitShield.collider != null;
    }

    private void HandleVerticalMovement()
    {
        if (CheckRoof())
        {
            _jumpVelocity = -1;
        }

        _onTopOfShield = OnTopOfShield();
        _onTopOfEnemy = OnTopOfEnemy();
        if (IsGrounded())
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                _jumpVelocity = _varJumpTopSpeed;
                if (_isShielded)
                    _canKill = true;
            }
            
            _varJumpTopSpeed = 1;
        }
        
        _jumpVelocity = Mathf.MoveTowards(_jumpVelocity, -_varJumpTopSpeed, _varJumpDeacceleration * Time.deltaTime);
        
        _playerAnimator.SetBool("isJumping",_varJumpTopSpeed != 1f);

        
    }
}