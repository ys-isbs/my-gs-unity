using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Fusion;
using Cinemachine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float maxSpeed = 5f;
    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] float jumpPower = 5f;
    [SerializeField] Transform foot;
    Animator animator;
    [SerializeField] AudioClip jumpSe;
    [SerializeField] Transform freeLookAt;

    //PlayerInput input;
    //InputAction moveInput;
    //InputAction jumpInput;
    //InputAction emoteInput; // Day1課題

    Rigidbody rb;
    [Networked] Vector3 moveDirection { get; set; }
    [Networked] NetworkButtons buttonsPrevious { get; set; }
    [Networked] int jumpCount { get; set; }
    int lastVisibleJump;
    [Networked] int emote1Count { get; set; }
    int lastVisibleEmote1;

    float distanceToGround;
    bool isGrounded;
    //private bool isJumping = false; // Day2課題
    //public bool IsJumping
    //{
    //    get { return this.isJumping; }
    //    private set { this.isJumping = value; }
    //}
    //public bool GetIsJumping()
    //{
    //    return isJumping;
    //}
    AudioSource audioSource;

    [Networked] public string Name { get; set; }
    [SerializeField] PlayerNameLabel nameLabelPref;
    [SerializeField] Transform nameLabelTarget;

    public override void Spawned()
    {
        Init();
        SetNameLabel();
    }

    void Init()
    {
        rb = GetComponent<Rigidbody>();
        //input = GetComponent<PlayerInput>();
        //moveInput = input.actions["Move"];
        //jumpInput = input.actions["Jump"];
        //emoteInput = input.actions["Emote"]; // Day1課題
        audioSource = GetComponent<AudioSource>();
        animator = GetComponentInChildren<Animator>();

        distanceToGround = transform.position.y - foot.position.y + 0.1f;

        lastVisibleJump = jumpCount;
        lastVisibleEmote1 = emote1Count;

        if (Object.HasInputAuthority)
        {
            CinemachineFreeLook freeLookCam = FindObjectOfType<CinemachineFreeLook>();
            freeLookCam.Follow = transform;
            freeLookCam.LookAt = freeLookAt;
            freeLookCam.Priority = 2;
        }
    }

    private void SetNameLabel()
    {
        Transform canvas = Transform.FindObjectOfType<Canvas>().transform;
        var label = Instantiate(nameLabelPref, canvas);
        label.Init(this, nameLabelTarget);
    }

    private void Update()
    {
        isGrounded = CheckGrounded();
        animator.SetBool("IsGrounded", isGrounded);
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out PlayerInputData inputData))
        {
            moveDirection = inputData.moveDirection;

            if (moveDirection != Vector3.zero)
            {
                Rotate();
            }
            Move();

            if (isGrounded && inputData.buttons.WasPressed(buttonsPrevious, InputButtons.Jump))
            {
                Jump();
                jumpCount++;
            }

            if (isGrounded && inputData.buttons.WasPressed(buttonsPrevious, InputButtons.Emote1))
            {
                emote1Count++;
            }

            buttonsPrevious = inputData.buttons;
        }
    }

    public override void Render()
    {
        animator.SetFloat("Speed", moveDirection.magnitude);

        if (jumpCount > lastVisibleJump)
        {
            audioSource.PlayOneShot(jumpSe);
            animator.SetTrigger("Jump");
        }

        if (emote1Count > lastVisibleEmote1)
        {
            animator.SetTrigger("Emote1");
        }

        lastVisibleJump = jumpCount;
        lastVisibleEmote1 = emote1Count;
    }

    // Update is called once per frame
    //void Update()
    //{
    //    isGrounded = CheckGrounded();
    //    animator.SetBool("IsGrounded", isGrounded);

    //    Vector2 moveInputValue = moveInput.ReadValue<Vector2>();
    //    Vector3 camForward = new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z).normalized;
    //    Vector3 verticalValue = moveInputValue.y * camForward;
    //    Vector3 holizontalValue = moveInputValue.x * cam.transform.right;
    //    moveDirection = verticalValue + holizontalValue;

    //    animator.SetFloat("Speed", moveDirection.magnitude);

    //    // プレイヤーの回転
    //    if (moveDirection != Vector3.zero)
    //    {
    //        Rotate();
    //    }

    //    // ジャンプ
    //    if (isGrounded && jumpInput.WasPressedThisFrame())
    //    {
    //        Jump();
    //    }

    //    if (emoteInput.WasPressedThisFrame()) // Day1課題
    //    {
    //        animator.SetTrigger("One");
    //    }
    //}

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        float currentSpeed = rb.velocity.magnitude;
        if (currentSpeed > maxSpeed) return;

        rb.AddForce(moveDirection * moveSpeed, ForceMode.VelocityChange);
    }

    void Rotate()
    {
        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Runner.DeltaTime);
    }

    void Jump()
    {
        //IsJumping = true; // Day2課題
        //Debug.Log(IsJumping);
        //StartCoroutine(SetIsJumpingFalse());
        rb.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
    }

    //IEnumerator SetIsJumpingFalse() // Day2課題
    //{
    //    yield return new WaitForSeconds(0.95f);
    //    IsJumping = false;
    //    Debug.Log(IsJumping);
    //}

    bool CheckGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, distanceToGround);
    }
}
