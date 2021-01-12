using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterMovement))]
public class PlatformerMovement : MonoBehaviour
{
    CharacterMovement characterMovement;

    private Rigidbody2D rb;

    Collider2D characterCollider;
    

    [SerializeField] float walkSpeed = 5, sprintSpeed = 10;
    private float moveSpeed;

    [SerializeField] float maxJumpHeight = 5, minJumpHeight = 3;
    float maxJumpVelocity, minJumpVelocity;

    [SerializeField] float timeToJumpApex = .5f;
    float gravity, wallGravity;

    [SerializeField] float accelerationTimeAir = 1, accelerationTimeGround = .1f;
    [SerializeField] float stopDeacceleratingX = .5f;

    Vector2 wallJump;
    [SerializeField] float wallSlideSpeedMax = 1;
    [SerializeField] float wallStickTime = .05f;

    int wallDirX;
    float timeToWallUnstick;

    public Vector3 velocity;
    float velocityXSmoothing;

    Vector2 directionalInput;
    Vector2 faceDir;
    bool flip;

    bool walking, sprinting, jumping, falling, idling;
    bool wallClimbing, wallSliding;
    public bool walkInput, canWallClimb;
    bool onGround, onWall;

    public bool flipDaSprite;

    public float timeScale = 1;

    public void Awake()
    {
        characterMovement = GetComponent<CharacterMovement>();
        rb = GetComponent<Rigidbody2D>();
        characterCollider = GetComponent<Collider2D>();

        faceDir = Vector2.right;

        CalculatePhysics();
    }

    //caculates gravity, max jump velocity and min jump velocity
    public void CalculatePhysics()
    {
        gravity = -(2 * MinJumpHieght) / Mathf.Pow(timeToJumpApex, 2);
        wallGravity = gravity * .6f;

        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

        moveSpeed = walkSpeed;

        wallJump = new Vector2(sprintSpeed, maxJumpVelocity);
    }

    public void FixedUpdate()
    {
        SetCharacterState();
        rb.transform.position += GetMovement();
        PostMovementUpdate();
    }

    public Vector3 GetMovement()
    {
        return characterMovement.Move(velocity * Time.deltaTime * timeScale, directionalInput);
    }

    public Vector3 GetMovement(Vector3 v)
    {
        return characterMovement.Move(v, directionalInput);
    }

    public void SetCharacterState()
    {
        onGround = characterMovement.sides.below;
        onWall = characterMovement.sides.left || characterMovement.sides.right;
        if (!onWall)
            canWallClimb = true;

        ResetPlayerStates();

        if (velocity.y > 0)
        {
            if (onWall)
                WallClimbing = true;
            else
                Jumping = true;
        }
        else if (velocity.y < 0 && !onGround)
        {
            if (onWall)
                WallSliding = true;
            else
                Falling = true;
        }
        else if (onGround && velocity.x != 0)
        {
            if (walkInput)
            {
                Walking = true;
                moveSpeed = walkSpeed;
            }
            else
            {
                Sprinting = true;
                moveSpeed = sprintSpeed;
            }
        }
        else
        {
            Idling = true;
        }

        CalculateVelocity();
        HandleWall();
        if (characterMovement.collideWithPlatforms && canWallClimb)
        {
            RunUpWall();
        }
    }

    public void CalculateVelocity()
    {
        float targetVelocityX = directionalInput.x * moveSpeed;

        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (characterMovement.sides.below) ? accelerationTimeGround : accelerationTimeAir);
        velocity.y += ((characterMovement.sides.left || characterMovement.sides.right) ? wallGravity : gravity) * Time.deltaTime * timeScale;
    }

    public void HandleWall()
    {
        if(WallClimbing || WallSliding)
        {
            wallDirX = (characterMovement.sides.left) ? -1 : 1;

            if (velocity.y < -wallSlideSpeedMax)
                velocity.y = -wallSlideSpeedMax;

            if (timeToWallUnstick > 0)
            {
                velocityXSmoothing = 0;
                velocity.x = 0;

                if (directionalInput.x != wallDirX && directionalInput.x != 0)
                {
                    if (characterMovement.collideWithPlatforms)
                        JumpOffWall();
                    timeToWallUnstick -= Time.deltaTime;
                }

                if (timeToWallUnstick <= 0)
                {
                    velocity.x = walkSpeed * -wallDirX;
                    timeToWallUnstick = wallStickTime;
                }
            }
            else
            {
                timeToWallUnstick = wallStickTime;
            }
        }
    }

    public void RunUpWall()
    {
        if ((characterMovement.sides.left && directionalInput.x == -1) || 
            (characterMovement.sides.right && directionalInput.x == 1))
        {
            canWallClimb = false;
            velocity.y = maxJumpVelocity;
            velocity.x = 0;
        }
    }

    public void JumpOffWall()
    {
        velocity.x = wallJump.x * -wallDirX;
        velocity.y = wallJump.y;
    }

    //when jumping button is pressed return velocity for wall jumping or max jump velocity
    public void OnJumpInputDown()
    {
        if (characterMovement.sides.below)
        {
            velocity.y = maxJumpVelocity;
        }
    }

    //when jump button is released try to lower the velocity y to min jump velocity
    //this make it jump higher the longer jump is held
    public void OnJumpInputUp()
    {
        if (velocity.y > minJumpVelocity)
            velocity.y = minJumpVelocity;
    }

    public void AnimateCharacter(Animator anim, bool canIntr)
    {
        anim.SetBool("Walking", walking && canIntr);
        anim.SetBool("Sprinting", sprinting && canIntr);
        anim.SetBool("WallClimbing", wallClimbing && canIntr);
        anim.SetBool("WallSliding", wallSliding && canIntr);
        anim.SetBool("Falling", falling && canIntr);
        anim.SetBool("Jumping", jumping && canIntr);
    }

    public void PostMovementUpdate()
    {
        if (characterMovement.sides.below || characterMovement.sides.above)
        {
            if (characterMovement.sides.slidingDownMaxSlope)
                velocity.y += characterMovement.sides.slopeNormal.y * -gravity * Time.deltaTime * timeScale;

            else
                velocity.y = 0;
        }

        if (directionalInput.x == 0 && Mathf.Abs(velocity.x) < stopDeacceleratingX)
        {
            velocity.x = 0;
        }
    }

    public void ResetPlayerStates()
    {
        Walking = Sprinting = Jumping = Falling = Idling = false;
        WallClimbing = WallSliding = false;
    }

    public void FlipSprite(Transform sprite, Vector3 offset)
    {
        if ((velocity.x != 0 && directionalInput.x != 0) || flipDaSprite)
        {
            faceDir = directionalInput;
            flip = directionalInput.x == -1;
            flipDaSprite = false;

            if(flip)
            {
                sprite.rotation = Quaternion.Euler(0, 180, 0);
                sprite.localPosition = new Vector3(-offset.x, offset.y);
            }
            else
            {
                sprite.rotation = Quaternion.Euler(0, 0, 0);
                sprite.localPosition = new Vector3(offset.x, offset.y);
            }
            //characterCollider.offset = flip ? leftCollider : rightCollider;
        }
    }

    public void FlipSpriteMousePos(Transform sprite, Vector3 offset)
    {
        Vector3 camPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        camPos.z = 0;
        float angle = Vector3.SignedAngle(Vector3.right, camPos - transform.position, Vector3.forward);

        if (Mathf.Abs(angle) < 90)
        {
            DirectionalInput = Vector3.right;
        }
        else
        {
            DirectionalInput = Vector3.left;
        }

        faceDir = directionalInput;
        flip = directionalInput.x == -1;

        if (flip)
        {
            sprite.rotation = Quaternion.Euler(0, 180, 0);
            sprite.localPosition = new Vector3(-offset.x, offset.y);
        }
        else
        {
            sprite.rotation = Quaternion.Euler(0, 0, 0);
            sprite.localPosition = new Vector3(offset.x, offset.y);
        }
    }

    public void OffsetAfterWallUp()
    {
        transform.position += new Vector3(wallDirX, characterCollider.bounds.size.y);
    }

    public bool CheckForGroundInFront(Vector3 dir)
    {
        Vector3 rayOrigin = new Vector3(0, characterCollider.bounds.min.y);
        rayOrigin.x = (dir.x == 1) ? characterCollider.bounds.max.x : characterCollider.bounds.min.x;

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, .1f, characterMovement.collisionMask);

        //Debug.DrawRay(rayOrigin, Vector2.down * .1f, Color.red);

        return hit;
    }

    /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Getters/Setters ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
    
    public CharacterMovement GetCharacterMovement()
    {
        return characterMovement;
    }

    public Vector3 DirectionalInput
    {
        get { return directionalInput; }
        set { directionalInput = value; }
    }

    public Vector2 FaceDir
    {
        get { return faceDir; }
    }

    public float WalkSpeed
    {
        get { return walkSpeed; }
        set { walkSpeed = value; }
    }

    public float SprintSpeed
    {
        get { return sprintSpeed; }
        set { sprintSpeed = value; }
    }

    public float AccelerationTimeAir
    {
        get { return accelerationTimeAir; }
        set { accelerationTimeAir = value; }
    }

    public float AccelerationTimeGround
    {
        get { return accelerationTimeGround; }
        set { accelerationTimeGround = value; }
    }

    public float StopDeacceleratingX
    {
        get { return stopDeacceleratingX; }
        set { stopDeacceleratingX = value; }
    }

    public float MaxjumpHeight
    {
        get { return maxJumpHeight; }
        set
        {
            maxJumpHeight = value;
            CalculatePhysics();
        }
    }

    public float MinJumpHieght
    {
        get { return minJumpHeight; }
        set
        {
            minJumpHeight = value;
            CalculatePhysics();
        }
    }

    public float TimeToJumpApex
    {
        get { return timeToJumpApex; }
        set
        {
            timeToJumpApex = value;
            CalculatePhysics();
        }
    }

    public Vector2 WallJump
    {
        get { return wallJump; }
        set { wallJump = value; }
    }

    public float WallSlideSpeedMax
    {
        get { return wallSlideSpeedMax; }
        set { wallSlideSpeedMax = value; }
    }

    public float WallStickTime
    {
        get { return wallStickTime; }
        set { wallStickTime = value; }
    }

    public bool Walking
    {
        get { return walking; }
        set { walking = value; }
    }

    public bool Sprinting
    {
        get { return sprinting; }
        set { sprinting = value; }
    }

    public bool Jumping
    {
        get { return jumping; }
        set { jumping = value; }
    }

    public bool WallSliding
    {
        get { return wallSliding; }
        set { wallSliding = value; }
    }

    public bool WallClimbing
    {
        get { return wallClimbing; }
        set { wallClimbing = value; }
    }

    public bool Idling
    {
        get { return idling; }
        set { idling = value; }
    }

    public bool Falling
    {
        get { return falling; }
        set { falling = value; }
    }

    public bool WalkInput
    {
        get { return walkInput; }
        set { walkInput = value; }
    }

    public bool Flip
    {
        get { return flip; }
    }

    public bool OnGround
    {
        get { return onGround; }
    }

    public bool OnWall
    {
        get { return onWall; }
    }
}
