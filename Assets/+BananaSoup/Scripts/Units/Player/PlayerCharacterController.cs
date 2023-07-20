using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.SceneManagement;
using BananaSoup.Units;

public class PlayerCharacterController : PlayerBase
{
    [Space]
    [SerializeField] private float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
    [SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
    [SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
    [SerializeField] private Transform m_WallCheck;                             //Posicion que controla si el personaje toca una pared
    [SerializeField] private bool isWallSlidingEnabled;
    [SerializeField] private bool isWallJumpingEnabled;

    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;            // Whether or not the player is grounded.
    private bool m_FacingRight = true;  // For determining which way the player is currently facing.
    private float limitFallSpeed = 25f; // Limit fall speed

    [SerializeField] private bool isDoubleJumpEnabled = false;
    private bool canDoubleJump = false; //If player can double jump
    private bool m_IsWall = false; //If there is a wall in front of the player
    private bool isWallSliding = false; //If player is sliding in a wall
    private bool oldWallSlidding = false; //If player is sliding in a wall in the previous frame
    private bool canCheck = false; //For check if player is wallsliding

    [SerializeField] private float life = 10f; //Life of the player
    [SerializeField] private bool invincible = false; //If player can die
    private bool canMove = true; //If player can move

    private Animator animator;
    [SerializeField] private ParticleSystem particleJumpUp; //Trail particles
    [SerializeField] private ParticleSystem particleJumpDown; //Explosion particles

    private float jumpWallStartX = 0;
    private float jumpWallDistX = 0; //Distance between player and wall
    private bool limitVelOnWallJump = false; //For limit wall jump distance with low fps

    private bool canBeStunned = true;

    [Header("Events")]
    [Space]

    public UnityEvent OnFallEvent;
    public UnityEvent OnLandEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    // Coroutines
    private Coroutine stunRoutine = null;
    private Coroutine stunCooldownRoutine = null;
    private Coroutine invincibleRoutine = null;
    private Coroutine waitToCheckRoutine = null;
    private Coroutine waitToEndSlidingRoutine = null;
    private Coroutine waitToDeadRoutine = null;

    // Animation strings
    private const string isJumpingParam = "IsJumping";
    private const string jumpUpParam = "JumpUp";
    private const string isWallSlidingParam = "IsWallSliding";
    private const string isDoubleJumpingParam = "IsDoubleJumping";
    private const string hitParam = "Hit";
    private const string isDeadParam = "IsDead";

    private void Awake()
    {
        Setup();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        Stunned += StunPlayer;
    }

    private void OnDisable()
    {
        TryStopCoroutine(ref stunRoutine);
        TryStopCoroutine(ref invincibleRoutine);
        TryStopCoroutine(ref waitToCheckRoutine);
        TryStopCoroutine(ref waitToEndSlidingRoutine);
        TryStopCoroutine(ref waitToDeadRoutine);

        Stunned -= StunPlayer;
    }

    private void FixedUpdate()
    {
        GroundCheck();
        WallCheck();
        LimitWallJumpVelocity();
    }

    private void Setup()
    {
        animator = GetComponent<Animator>();
        if ( animator == null )
        {
            Debug.LogError($"{name} is missing a Animator!");
        }

        if ( OnFallEvent == null )
        {
            OnFallEvent = new UnityEvent();
        }

        if ( OnLandEvent == null )
        {
            OnLandEvent = new UnityEvent();
        }
    }

    private void WallCheck()
    {
        m_IsWall = false;

        if ( !m_Grounded )
        {
            OnFallEvent.Invoke();
            Collider[] collidersWall = Physics.OverlapSphere(m_WallCheck.position, k_GroundedRadius, m_WhatIsGround);
            for ( int i = 0; i < collidersWall.Length; i++ )
            {
                if ( collidersWall[i].gameObject != null )
                {
                    m_IsWall = true;
                }
            }
        }
    }

    private void GroundCheck()
    {
        bool wasGrounded = m_Grounded;
        m_Grounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider[] colliders = Physics.OverlapSphere(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for ( int i = 0; i < colliders.Length; i++ )
        {
            if ( colliders[i].gameObject != gameObject )
            {
                m_Grounded = true;
            }

            if ( !wasGrounded )
            {
                OnLandEvent.Invoke();
                if ( !m_IsWall )
                {
                    particleJumpDown.Play();
                }

                if ( isDoubleJumpEnabled )
                {
                    canDoubleJump = true;
                }

                if ( rb.velocity.y < 0f )
                {
                    limitVelOnWallJump = false;
                }
            }
        }
    }

    private void LimitWallJumpVelocity()
    {
        if ( limitVelOnWallJump )
        {
            if ( rb.velocity.y < -0.5f )
            {
                limitVelOnWallJump = false;
            }

            jumpWallDistX = (jumpWallStartX - transform.position.x) * transform.localScale.x;

            if ( jumpWallDistX < -0.5f && jumpWallDistX > -1f )
            {
                canMove = true;
            }
            else if ( jumpWallDistX < -1f && jumpWallDistX >= -2f )
            {
                canMove = true;
                rb.velocity = new Vector2(10f * transform.localScale.x, rb.velocity.y);
            }
            else if ( jumpWallDistX < -2f )
            {
                limitVelOnWallJump = false;
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
            else if ( jumpWallDistX > 0 )
            {
                limitVelOnWallJump = false;
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }
    }

    public void Move(float move, bool jump)
    {
        if ( !canMove )
        {
            return;
        }

        //only control the player if grounded or airControl is turned on
        if ( m_Grounded || m_AirControl )
        {
            if ( rb.velocity.y < -limitFallSpeed )
            {
                rb.velocity = new Vector2(rb.velocity.x, -limitFallSpeed);
            }
            // Move the character by finding the target velocity
            Vector3 targetVelocity = new Vector2(move, rb.velocity.y);

            // Update player's velocity
            rb.velocity = targetVelocity;

            // If the input is moving the player right and the player is facing left...
            if ( move > 0 && !m_FacingRight && !isWallSliding )
            {
                // ... flip the player.
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if ( move < 0 && m_FacingRight && !isWallSliding )
            {
                // ... flip the player.
                Flip();
            }
        }

        // If the player should jump...
        if ( m_Grounded && jump )
        {
            // Add a vertical force to the player.
            animator.SetBool(isJumpingParam, true);
            animator.SetBool(jumpUpParam, true);
            m_Grounded = false;
            rb.AddForce(new Vector2(0f, m_JumpForce));
            if ( isDoubleJumpEnabled )
            {
                canDoubleJump = true;
            }
            particleJumpDown.Play();
            particleJumpUp.Play();
        }
        else if ( !m_Grounded && jump && canDoubleJump && !isWallSliding )
        {
            canDoubleJump = false;
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(0f, m_JumpForce / 1.2f));
            animator.SetBool(isDoubleJumpingParam, true);
        }
        else if ( m_IsWall && !m_Grounded )
        {
            //Debug.Log("Wall");

            if ( !oldWallSlidding && rb.velocity.y < 0 )
            {
                //Debug.Log("Snap the wall");

                isWallSliding = true;
                m_WallCheck.localPosition = new Vector3(-m_WallCheck.localPosition.x, m_WallCheck.localPosition.y, 0);
                Flip();
                StartCoroutine(WaitToCheck(0.1f));
                if ( isDoubleJumpEnabled )
                {
                    canDoubleJump = true;
                }
                if ( isWallSlidingEnabled )
                {
                    animator.SetBool(isWallSlidingParam, true);
                }
            }

            // Player slides down the wall
            if ( isWallSliding )
            {
                //Debug.Log("Player slides down the wall");

                if ( move * transform.localScale.x > 0.1f )
                {
                    StartCoroutine(WaitToEndSliding());
                }
                else
                {
                    oldWallSlidding = true;
                    if ( isWallSlidingEnabled )
                    {
                        rb.velocity = new Vector2(-transform.localScale.x * 2, -5);
                    }
                    else
                    {
                        //Debug.Log("Wall Sliding NOT Enabled");
                        rb.velocity = new Vector2(-transform.localScale.x * 2, -9.8f);
                    }
                }
            }

            if ( jump && isWallSliding && isWallJumpingEnabled )
            {
                //Debug.Log("Player jumps of the wall");

                animator.SetBool(isJumpingParam, true);
                animator.SetBool(jumpUpParam, true);
                rb.velocity = new Vector2(0f, 0f);
                rb.AddForce(new Vector2(transform.localScale.x * m_JumpForce * 1.2f, m_JumpForce));
                jumpWallStartX = transform.position.x;
                limitVelOnWallJump = true;
                if ( isDoubleJumpEnabled )
                {
                    canDoubleJump = true;
                }
                isWallSliding = false;
                animator.SetBool(isWallSlidingParam, false);
                oldWallSlidding = false;
                m_WallCheck.localPosition = new Vector3(Mathf.Abs(m_WallCheck.localPosition.x), m_WallCheck.localPosition.y, 0);
                canMove = false;
            }
        }
        else if ( isWallSliding && !m_IsWall && canCheck )
        {
            //Debug.Log("Sliding ended");

            isWallSliding = false;
            animator.SetBool(isWallSlidingParam, false);
            oldWallSlidding = false;
            m_WallCheck.localPosition = new Vector3(Mathf.Abs(m_WallCheck.localPosition.x), m_WallCheck.localPosition.y, 0);
            if ( isDoubleJumpEnabled )
            {
                canDoubleJump = true;
            }
        }
    }

    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        if ( m_FacingRight )
        {
            transform.rotation = Quaternion.Euler(Vector3.zero);
        }
        else if ( !m_FacingRight )
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }

    public void ApplyDamage(float damage, Vector3 position)
    {
        if ( !invincible )
        {
            animator.SetBool(hitParam, true);
            life -= damage;
            Vector2 damageDir = Vector3.Normalize(transform.position - position) * 40f;
            rb.velocity = Vector2.zero;
            rb.AddForce(damageDir * 10);
            if ( life <= 0 )
            {
                StartCoroutine(WaitToDead());
            }
            else
            {
                StartCoroutine(StunRoutine(0.25f));
                StartCoroutine(MakeInvincible(1f));
            }
        }
    }

    public override void PlayerFinished()
    {
        canMove = false;
        invincible = true;

        Debug.Log($"PlayerID {PlayerID} reached the goal!");

        // TODO: Launch event to inform GameManager that the player has finished
    }

    public void StunPlayer(float time)
    {
        if ( stunRoutine == null && canBeStunned )
        {
            canBeStunned = false;
            stunRoutine = StartCoroutine(StunRoutine(time));
        }
    }

    #region Coroutines
    private IEnumerator StunRoutine(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
        TryStopCoroutine(ref stunRoutine);
        SetIsStunnedFalse();

        if ( stunCooldownRoutine == null )
        {
            stunCooldownRoutine = StartCoroutine(StunCooldownRoutine(stunCooldown));
        }
    }

    private IEnumerator StunCooldownRoutine(float time)
    {
        yield return new WaitForSeconds(time);
        canBeStunned = true;
        TryStopCoroutine(ref stunCooldownRoutine);
    }

    private IEnumerator MakeInvincible(float time)
    {
        invincible = true;
        yield return new WaitForSeconds(time);
        invincible = false;
    }

    private IEnumerator WaitToCheck(float time)
    {
        canCheck = false;
        yield return new WaitForSeconds(time);
        canCheck = true;
    }

    private IEnumerator WaitToEndSliding()
    {
        yield return new WaitForSeconds(0.1f);
        if ( isDoubleJumpEnabled )
        {
            canDoubleJump = true;
        }
        isWallSliding = false;
        animator.SetBool(isWallSlidingParam, false);
        oldWallSlidding = false;
        m_WallCheck.localPosition = new Vector3(Mathf.Abs(m_WallCheck.localPosition.x), m_WallCheck.localPosition.y, 0);
    }

    private IEnumerator WaitToDead()
    {
        animator.SetBool(isDeadParam, true);
        canMove = false;
        invincible = true;
        GetComponent<Attack>().enabled = false;
        yield return new WaitForSeconds(0.4f);
        rb.velocity = new Vector2(0, rb.velocity.y);
        yield return new WaitForSeconds(1.1f);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    private void TryStopCoroutine(ref Coroutine routine)
    {
        if ( routine != null )
        {
            StopCoroutine(routine);
            routine = null;
        }
    }
    #endregion Coroutines
}
