using UnityEngine;
using UnityEngine.InputSystem;

using fpsRed.Utilities;

namespace fpsRed.Player
{
    [RequireComponent(typeof(Rigidbody), typeof(CollisionCheck))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform lookTransform;

        [Space, Header("Speed Values")]
        [SerializeField] private float maxSpeed;
        [SerializeField] private float groundAcceleration;
        [SerializeField] private float airAcceleration;

        [Space, SerializeField, Range(0f, 30f)] private float groundFriction;

        [Space, Header("Jump Values")]
        [SerializeField, Range(0f, 30f)] private float jumpForce;
        [Space, SerializeField, Range(0f, 1f)] private float jumpBuffer = 0.1f;
        private float jumpBufferCounter;

        private Vector3 moveInput;

        private Vector3 wishDir;
        private float addSpeed;
        private float currentSpeed;
        private float acceleration;

        private Rigidbody body;
        private CollisionCheck collisionCheck;
        private PlayerInput playerInput;

        private void Start()
        {
            body = GetComponent<Rigidbody>();
            collisionCheck = GetComponent<CollisionCheck>();
            playerInput = GeneralUtils.GetPlayerInput();

            playerInput.actions["Player/Move"].performed += ReceiveMoveInput;
            playerInput.actions["Player/Move"].canceled += ReceiveMoveInput;

            playerInput.actions["Player/Jump"].performed += ReceiveJumpInput;
        }

        private void ReceiveJumpInput(InputAction.CallbackContext ctx)
        {
            if (collisionCheck.OnGround)
            {
                Jump();
                return;
            }
            jumpBufferCounter = jumpBuffer;
        }

        private void Jump()
        {
            body.velocity += jumpForce * Vector3.up;
            jumpBufferCounter = 0f;
        }

        private void ReceiveMoveInput(InputAction.CallbackContext ctx)
        {
            Vector2 input = ctx.ReadValue<Vector2>();
            moveInput = new Vector3(input.x, 0f, input.y);
        }

        private void ProcessHorizontalMovement()
        {
            CalculateWishDir();

            ApplyFriction();

            acceleration = collisionCheck.OnGround ? groundAcceleration : airAcceleration;
            currentSpeed = Vector3.Dot(body.velocity, wishDir);
            addSpeed = Mathf.Clamp(maxSpeed - currentSpeed, 0f, acceleration * Time.fixedDeltaTime);

            body.velocity += wishDir * addSpeed;
            
            DrawMovementRays();
        }

        private void DrawMovementRays()
        {
            Debug.DrawRay(transform.position, lookTransform.forward, Color.red);
            Debug.DrawRay(transform.position, wishDir, Color.blue);
            Debug.DrawRay(transform.position, new Vector3(body.velocity.x, 0f, body.velocity.z), Color.green);
        }

        private void CalculateWishDir()
        {
            wishDir = lookTransform.forward * moveInput.z;
            wishDir += lookTransform.right * moveInput.x;

            wishDir.y = 0f;

            wishDir.Normalize();
        }

        private void ApplyFriction()
        {
            if (collisionCheck.OnGround)
            {
                body.AddForce(body.velocity * -groundFriction);
            }
        }

        private void FixedUpdate()
        {
            ProcessHorizontalMovement();
        }

        private void Update()
        {
            if (jumpBufferCounter > 0f && collisionCheck.OnGround)
            {
                Jump();
                return;
            }
            jumpBufferCounter -= Time.deltaTime;
        }
    }
}