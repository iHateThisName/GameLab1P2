using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Private fields: _isPrivate
//Public fields: IsPrivate
//Scoped variables: isPrivate

public class PlayerMovment : MonoBehaviour {
    [SerializeField] private float JumpForce = 15f;

    [Header("Speed Settings")]
    [SerializeField]
    [Tooltip("Base speed before accumulating")]
    private float BaseSpeed = 4f;
    [SerializeField]
    [Tooltip("Maximum speed player can reach")]
    private float MaxSpeed = 10f;
    [SerializeField]
    [Tooltip("Time to reach max speed")]
    private float AccelerationTime = 2f;
    [SerializeField]
    [Tooltip("Max fall speed")]
    private float MaxFallSpeed = -75f;

    private Rigidbody2D _rb;

    [Header("Check Point")]
    public Transform GroundCheckPoint;
    public Transform LeftWallCheckPoint;
    public Transform RightWallCheckPoint;

    [Header("Layers")]
    [Tooltip("List of Layers that allowes jumping.")]
    public LayerMask AllowJumpMask;
    [Tooltip("List of Layers that allowes wall jumping.")]
    public LayerMask AllowWallJumpMask;
    [Tooltip("List of Layers speed amplification values. 1.5 increases the max speed 50%")]
    public List<AmplifyLayer> AmplifyLayers;

    [Header("Player State")]
    [SerializeField] private bool _isGrounded;
    [SerializeField] private bool _isWalled;
    [SerializeField] private bool _isWallJumping;
    private bool _isShrinking;

    //Coyote time
    [Header("Coyote Time")]
    [SerializeField] private float CoyoteTime = 0.2f;
    [Tooltip("Able to performe coyoteTime if the counter is above 0")]
    [SerializeField] private float _coyoteCounter;

    //Jump Buffer
    [Header("Jump Buffer")]
    [SerializeField] private float JumpBufferLength = .1f;
    [Tooltip("Able to performe jump if the buffer is above 0")]
    [SerializeField] private float _jumpBufferCount = -1f;

    //Wall Jump
    [Header("Wall Jump")]
    [SerializeField] private float WallJumpForce = 15f;
    [SerializeField] private Vector2 WallJumpDirection = new Vector2(1f, 1.5f);
    [SerializeField] private float WallSlideSpeed = 2f;

    private float _currentSpeed;  // The current speed that will be adjusted over time
    private float _accelerationRate;  // The rate at which the speed will increase

    private TimerScript _timerScript;
    private Animator _animator;
    private SpriteRenderer[] _spriteFlip;

    private bool _isPlayerFacingRight = false;

    private void Start() {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteFlip = GetComponentsInChildren<SpriteRenderer>();
        _timerScript = GetComponent<TimerScript>();
        _currentSpeed = BaseSpeed; // Initialize current speed
        _accelerationRate = (MaxSpeed - BaseSpeed) / AccelerationTime;
        WallJumpDirection.Normalize();
    }

    private void Update() {
        _isGrounded = Physics2D.OverlapCircle(GroundCheckPoint.position, 0.1f, AllowJumpMask);
        _isWalled = Physics2D.OverlapCircle(LeftWallCheckPoint.position, 0.2f, AllowWallJumpMask) || Physics2D.OverlapCircle(RightWallCheckPoint.position, 0.2f, AllowWallJumpMask);
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        if (horizontalInput != 0) {
            if (horizontalInput > 0.1f && !_isPlayerFacingRight) {
                foreach (var s in _spriteFlip) {
                    s.flipX = true;
                }
                _isPlayerFacingRight = true;
            } else if (horizontalInput < -0.1f && _isPlayerFacingRight) {
                foreach (var s in _spriteFlip) {
                    s.flipX = false;
                }
                _isPlayerFacingRight = false;
            }
        }

        // Set Current Speed
        if (_isGrounded && _isWalled) {
            _currentSpeed = BaseSpeed;
        } else if (horizontalInput != 0 && !_isWallJumping) {
            //Accumelating Speed
            _currentSpeed += _accelerationRate * Time.deltaTime;  // Gradually increase speed

            float amplyfiedMaxSpeed = MaxSpeed * GetAmplifyValue();

            // If the current speed exceeds the max speed, smoothly transition it down
            if (_currentSpeed > amplyfiedMaxSpeed) {
                _currentSpeed = Mathf.Lerp(_currentSpeed, amplyfiedMaxSpeed, 0.2f);
            }
            _currentSpeed = Mathf.Clamp(_currentSpeed, BaseSpeed, amplyfiedMaxSpeed);

            //Start timer
            if (!_timerScript.IsTimerRunning) _timerScript.StartTimer();

        } else {
            _currentSpeed = BaseSpeed;  // Reset to base speed when not moving
        }

        // Applyiing velocity
        if (!_isWallJumping && (!_isWalled || _isGrounded)) {
            _rb.velocity = new Vector2(Mathf.Lerp(_rb.velocity.x, Input.GetAxisRaw("Horizontal") * _currentSpeed, 0.2f), _rb.velocity.y);
        }

        //CoyoteTime
        if (_isGrounded) {
            _coyoteCounter = CoyoteTime;
        } else if (_coyoteCounter > 0f) {
            _coyoteCounter -= Time.deltaTime;
        }

        //Jump Buffer
        if (Input.GetKeyDown(KeyCode.Space)) {
            _jumpBufferCount = JumpBufferLength;
        } else if (_jumpBufferCount > 0f) {
            _jumpBufferCount -= Time.deltaTime;
        }

        //Normal Jumps
        if (_jumpBufferCount >= 0f && _coyoteCounter > 0f) {
            _rb.velocity = new Vector2(_rb.velocity.x, JumpForce);
            _coyoteCounter = -1f;
            _jumpBufferCount = -1f;
        }
        //Small Jumps
        if (Input.GetKeyUp(KeyCode.Space) && _rb.velocity.y > 0) {
            _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y * .5f);
            _coyoteCounter = -1f;
        }

        // Wall Jump
        if (_jumpBufferCount >= 0f && _isWalled && !_isGrounded) {
            _isWallJumping = true;
            _currentSpeed = BaseSpeed;

            // Determine the direction based on which wall the player is on
            float wallDirection = Physics2D.OverlapCircle(LeftWallCheckPoint.position, .2f, AllowWallJumpMask) ? 1 : -1; //TODO, Check is happening twice

            _rb.velocity = new Vector2(wallDirection * WallJumpDirection.x * WallJumpForce,
                                       WallJumpDirection.y * WallJumpForce);
            _jumpBufferCount = -1f;

            // Invoke method to reset wall jumping after a short delay
            Invoke(nameof(ResetWallJumping), 0.2f);
        }

        // Wall Slide
        if (_isWalled && !_isGrounded && _rb.velocity.y < 0 && !_isWallJumping) {
            _rb.velocity = new Vector2(_rb.velocity.x, -WallSlideSpeed);
        }

        // Max fall speed
        if (_rb.velocity.y < MaxFallSpeed) {
            _rb.velocity = new Vector2(_rb.velocity.x, MaxFallSpeed);
        }

        //Shrink
        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            _isShrinking = !_isShrinking;
            _animator.SetBool("isShrinking", _isShrinking);
        }
    }
    private void ResetWallJumping() {
        _isWallJumping = false;
    }

    private float GetAmplifyValue() {
        Collider2D collider = Physics2D.OverlapCircle(GroundCheckPoint.position, .2f, ~(1 << 0)); // ~(1 << 0) ignores the Default layer.
        if (collider != null) {
            //Debug.Log($"Detected collider on layer: {LayerMask.LayerToName(collider.gameObject.layer)}");
            foreach (var layerValuePair in AmplifyLayers) { //Todo, expansiv for loop, maby use dictionary?
                // Check if the collider's layer is included in the LayerMask
                if ((layerValuePair.Layer.value & (1 << collider.gameObject.layer)) != 0) {
                    return layerValuePair.Value;
                }
            }
        }
        return 1f; // Default value
    }

}
