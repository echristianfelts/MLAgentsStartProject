using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationandMovementController : MonoBehaviour
{
    // Reference Variables
    PlayerInput playerInput;
    CharacterController characterController;
    Animator animator;

    // Variables for input values.
    Vector2 currentMovementInput;
    Vector3 currentMovement;
    Vector3 currentRunMovement;
    bool isMovementPressed;
    bool isRunPressed;
    public float rotationFactorPerFrame = 1.0f;
    public float gravity = 9.81f;
    public float jumpForce = 3f;
    public int jumpDuration = 5;
    int currentJumpIndexTime = 0;
    float jumpForceIndexedDecrement;

    // TEST  TEST  TEST
    Vector3 vecDir = Vector3.zero;
    public float speed = 6f;
    public float runSpeed = 3f;
    float gravitySlice = 0f;



    void Awake()
    {
        gravity *= 100f;
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        playerInput.CharacterControls.Move.started += onMovementInput;
        playerInput.CharacterControls.Move.canceled += onMovementInput;
        playerInput.CharacterControls.Move.performed += onMovementInput;

        playerInput.CharacterControls.Run.started += onRun;
        playerInput.CharacterControls.Run.canceled += onRun;
        //  playerInput.CharacterControls.Run.performed += onMovementInput;



        // Somthing somthing storing player input..?
        // So this is wiring up connections from context.ReadValue then plugging it into currentMovement.
        //  Which is then used in Update.  ok.  I kind of get it..
        /*
        playerInput.CharacterControls.Move.started += context =>
        {
            currentMovementInput = context.ReadValue<Vector2>();
            currentMovement.x = currentMovementInput.x;
            currentMovement.y = currentMovementInput.y;
            // Get Gabe to explain this shit to me....
            isMovementPressed = currentMovementInput.x != 0 || currentMovement.y != 0;

            Debug.Log("Current Input Vector =" + context.ReadValue<Vector2>());

        };
        playerInput.CharacterControls.Move.canceled += context =>
        {
            currentMovementInput = context.ReadValue<Vector2>();
            currentMovement.x = currentMovementInput.x;
            currentMovement.y = currentMovementInput.y;
            // Get Gabe to explain this shit to me....
            isMovementPressed = currentMovementInput.x != 0 || currentMovement.y != 0;

            Debug.Log("Current Input Vector =" + context.ReadValue<Vector2>());

        };
        playerInput.CharacterControls.Move.performed += context =>
        {
            currentMovementInput = context.ReadValue<Vector2>();
            currentMovement.x = currentMovementInput.x;
            currentMovement.z = currentMovementInput.y;
            // Get Gabe to explain this shit to me....
            isMovementPressed = currentMovementInput.x != 0 || currentMovement.y != 0;

            Debug.Log("Current Input Vector =" + context.ReadValue<Vector2>());

        };*/
    }

    void onRun(InputAction.CallbackContext context)
    {
        isRunPressed = context.ReadValueAsButton();
    }

    void handleRotation()
    {
        Vector3 positionToLookAt;
        // The change in positon that our character should look at.
        positionToLookAt.x = currentMovement.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = currentMovement.z;
        // the current rotation ofthe character
        Quaternion currentRotation = transform.rotation;

        if (isMovementPressed)
        {
            // creates new rotation based on where the player is currently pressing
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
        }
    }

    void onMovementInput(InputAction.CallbackContext context)
    {
        /*
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x;
        currentMovement.z = currentMovementInput.y;
        // Get Gabe to explain this shit to me....
        isMovementPressed = currentMovementInput.x != 0 || currentMovement.z != 0;

        Debug.Log("Current Input Vector =" + context.ReadValue<Vector2>());
        */
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            isMovementPressed = true;
        }

    }

    void handleAnimation()
    {
        // get parameter values from animator.
        bool isWalking = animator.GetBool("isWalking");
        bool isRunning = animator.GetBool("isRunning");

        if (isMovementPressed && !isWalking)
        {
            animator.SetBool("isWalking",true);
        }
        else if (!isMovementPressed && isWalking)
        {
            animator.SetBool("isWalking",false);
        }

        if (isRunPressed && !isRunning)
        {
            animator.SetBool("isRunning", true);
        }
        else if (!isRunPressed && isRunning)
        {
            animator.SetBool("isRunning", false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        float vertVal = Input.GetAxis("Vertical");
        if(vertVal < 0f)
        {
            vertVal *= 0.25f;
        }
        var tmpDirection = Vector3.zero;
        if(vertVal > 0f && isRunPressed)
        {
                vertVal *= runSpeed;
        }
        if(characterController.isGrounded)
        {
            gravitySlice = 0;
        }
        else
        {
            gravitySlice = gravity * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentJumpIndexTime = jumpDuration;

            if (characterController.isGrounded)
            {
                currentJumpIndexTime = jumpDuration;
            }

        }
        if (currentJumpIndexTime > 0)
        {
            gravitySlice = gravity * Time.deltaTime * jumpForceIndexedDecrement * (-1);
            jumpForceIndexedDecrement *= .75f;
        }
        tmpDirection.y -= gravitySlice;
        tmpDirection.z = vertVal * speed;
        transform.Rotate(0, Input.GetAxis("Horizontal"), 0);




        currentMovement = transform.TransformDirection(tmpDirection);
        //  controller.Move(direction * Time.deltaTime);
        //////////

        //  handleRotation();
        handleAnimation();
        characterController.Move(currentMovement * Time.deltaTime); // Uing our constantly updated currentMovement variable...  What?


        if ((Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0))
        {
            isMovementPressed = false;

        }
        if (currentJumpIndexTime > 0)
        {
            currentJumpIndexTime -= 1;
        }
        else
        {
            jumpForceIndexedDecrement = jumpForce;
        }

    }

    void OnEnable()
    {
        playerInput.CharacterControls.Enable();
    }

    void OnDisable()
    {
        playerInput.CharacterControls.Disable();
    }
}
