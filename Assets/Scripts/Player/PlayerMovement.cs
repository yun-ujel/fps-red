using UnityEngine;
using UnityEngine.InputSystem;

using fpsRed.Utilities;
using Unity.PlasticSCM.Editor.WebApi;

namespace fpsRed.Player
{
    [RequireComponent(typeof(Rigidbody), typeof(CollisionCheck), typeof(BoxCollider))]
    public class PlayerMovement : MonoBehaviour
    {
        #region Parameters
        #region Events
        public class OnCrouchEventArgs : System.EventArgs
        {
            public bool IsCrouched { get; private set; }
            public float HeightDifference { get; private set; }

            public OnCrouchEventArgs(bool isCrouched, float heightDifference)
            {
                IsCrouched = isCrouched;
                HeightDifference = heightDifference;
            }
        }
        public class OnJumpEventArgs : System.EventArgs
        {
            public float JumpForce { get; private set; }
            public OnJumpEventArgs(float jumpForce)
            {
                JumpForce = jumpForce;
            }
        }

        public event System.EventHandler<OnCrouchEventArgs> OnCrouchEvent;
        public event System.EventHandler<OnJumpEventArgs> OnJumpEvent;
        #endregion

        #region Serialized
        [Header("References")]
        [SerializeField] private Transform lookTransform;
        [SerializeField] private LayerMask groundLayers;

        [Header("Speed Values")]
        [SerializeField] private float maxSpeed = 16f;
        [SerializeField] private float crouchSpeed = 8f;

        [Space, SerializeField] private float groundAcceleration = 160f;
        [SerializeField] private float airAcceleration = 32f;

        [Space, SerializeField, Range(0f, 30f)] private float groundFriction = 8f;
        [SerializeField, Range(0f, 30f)] private float slideFriction = 2f;

        [Header("Jump Values")]
        [SerializeField, Range(0f, 30f)] private float jumpForce = 16f;
        [SerializeField, Range(0f, 1f)] private float jumpBuffer = 0.1f;

        [Header("Crouch/Slide Values")]
        [SerializeField, Range(0f, 5f)] private float defaultPlayerHeight = 3f;
        [SerializeField, Range(0f, 5f)] private float crouchedHeight = 1f;

        [Space, SerializeField, Range(0f, 10f)] private float minSlideDuration = 0.5f;
        [SerializeField, Range(0f, 30f)] private float slideForce = 10f;
        private float heightDifference => defaultPlayerHeight - crouchedHeight;
        #endregion

        #region Exposed
        public bool IsCrouched { get; private set; }
        public bool IsSliding => IsCrouched && slideCounter > 0f && collisionCheck.OnGround;
        #endregion

        #region Private
        private Rigidbody body;
        private PlayerInput playerInput;
        private CollisionCheck collisionCheck;
        private BoxCollider boxCollider;

        private float jumpBufferCounter;
        private bool jumpingThisFrame;

        private Vector3 moveInput;

        private Vector3 wishDir;
        private float addSpeed;
        private float currentSpeed;
        private float acceleration;

        private bool crouchInput;
        private float slideCounter;
        #endregion

        #endregion
        private void Start()
        {
            body = GetComponent<Rigidbody>();
            playerInput = GeneralUtils.GetPlayerInput();
            
            collisionCheck = GetComponent<CollisionCheck>();
            boxCollider = GetComponent<BoxCollider>();

            playerInput.actions["Player/Move"].performed += ReceiveMoveInput;
            playerInput.actions["Player/Move"].canceled += ReceiveMoveInput;

            playerInput.actions["Player/Jump"].performed += ReceiveJumpInput;

            playerInput.actions["Player/Crouch"].performed += ReceiveCrouchInput;
            playerInput.actions["Player/Crouch"].canceled += ReceiveCrouchInput;

            collisionCheck.OnHitGroundEvent += OnHitGround;
        }

        private void FixedUpdate()
        {
            ApplyFriction();
            if (!IsSliding)
            {
                ProcessHorizontalMovement();
            }
            ProcessCrouch();

            if (collisionCheck.OnGround)
            {
                slideCounter -= Time.fixedDeltaTime;
            }

            if (jumpBufferCounter > 0f && collisionCheck.OnGround && !jumpingThisFrame)
            {
                Jump();
                return;
            }
            jumpBufferCounter -= Time.fixedDeltaTime;
            jumpingThisFrame = false;
        }

        #region Input Methods
        private void ReceiveJumpInput(InputAction.CallbackContext ctx)
        {
            if (collisionCheck.OnGround)
            {
                Jump();
                return;
            }
            jumpBufferCounter = jumpBuffer;
        }

        private void ReceiveMoveInput(InputAction.CallbackContext ctx)
        {
            Vector2 input = ctx.ReadValue<Vector2>();
            moveInput = new Vector3(input.x, 0f, input.y);
        }

        private void ReceiveCrouchInput(InputAction.CallbackContext ctx)
        {
            crouchInput = ctx.performed;
        }
        #endregion

        #region Movement Methods

        private void Jump()
        {
            body.velocity += jumpForce * Vector3.up;
            jumpBufferCounter = 0f;
            jumpingThisFrame = true;

            OnJumpEvent?.Invoke(this, new(jumpForce));
        }

        private void OnHitGround(object sender, System.EventArgs args)
        {
            if (IsCrouched && body.velocity.Horizontal().sqrMagnitude > 0.1f)
            {
                EnterSlide();
            }
        }

        #region Crouch
        private void ProcessCrouch()
        {
            if (!IsCrouched && crouchInput)
            {
                EnterCrouch();
            }

            if (IsCrouched && !crouchInput && CanExitCrouch())
            {
                ExitCrouch();
            }
        }

        private void EnterCrouch()
        {
            IsCrouched = true;

            boxCollider.size = new Vector3(boxCollider.size.x, crouchedHeight, boxCollider.size.z);
            boxCollider.center -= heightDifference / 2 * Vector3.up;

            if (body.velocity.Horizontal().sqrMagnitude > 0.1f)
            {
                EnterSlide();
            }

            OnCrouchEvent?.Invoke(this, new(IsCrouched, heightDifference));
        }

        private void ExitCrouch()
        {
            IsCrouched = false;

            boxCollider.size = new Vector3(boxCollider.size.x, defaultPlayerHeight, boxCollider.size.z);
            boxCollider.center += heightDifference / 2 * Vector3.up;

            OnCrouchEvent?.Invoke(this, new(IsCrouched, heightDifference));
        }

        private bool CanExitCrouch()
        {
            return !Physics.Raycast(transform.position + boxCollider.center, Vector3.up, heightDifference, groundLayers);
        }
        #endregion

        #region Slide
        private void EnterSlide()
        {
            slideCounter = CalculateSlideDuration(out float proportion);

            body.velocity += body.velocity.Horizontal().normalized * (slideForce * Mathf.Clamp01(proportion));
        }

        private float CalculateSlideDuration(out float proportion)
        {
            float velocity = body.velocity.magnitude;
            float multiplier = 1 / maxSpeed;

            proportion = velocity * multiplier;

            return minSlideDuration * proportion;
        }
        #endregion

        #region Horizontal Movement
        private void ProcessHorizontalMovement()
        {
            wishDir = CalculateWishDir();

            float speed = IsCrouched && collisionCheck.OnGround ? crouchSpeed : maxSpeed;

            acceleration = collisionCheck.OnGround ? groundAcceleration : airAcceleration;
            currentSpeed = Vector3.Dot(body.velocity, wishDir);
            addSpeed = Mathf.Clamp(speed - currentSpeed, 0f, acceleration * Time.fixedDeltaTime);

            body.velocity += wishDir * addSpeed;
            
            DrawMovementRays();
        }

        #region Calculation
        private Vector3 CalculateWishDir()
        {
            Vector3 wishDir = lookTransform.forward * moveInput.z;
            wishDir += lookTransform.right * moveInput.x;

            wishDir.y = 0f;

            return wishDir.normalized;
        }

        private void ApplyFriction()
        {
            if (!collisionCheck.OnGround)
            {
                return;
            }
            if (IsSliding)
            {
                body.AddForce(body.velocity * -slideFriction);
                return;
            }

            body.AddForce(body.velocity * -groundFriction);
        }
        #endregion
        #endregion
        #endregion

        #region Debug Methods
        private void DrawMovementRays()
        {
            Debug.DrawRay(transform.position, lookTransform.forward, Color.red);
            Debug.DrawRay(transform.position, wishDir, Color.blue);
            Debug.DrawRay(transform.position, new Vector3(body.velocity.x, 0f, body.velocity.z), Color.green);
        }
        #endregion
    }
}