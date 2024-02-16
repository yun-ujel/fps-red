using UnityEngine;
using UnityEngine.InputSystem;

using fpsRed.Player;
using fpsRed.Utilities;

namespace fpsRed.Animation
{
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimator : MonoBehaviour
    {
        [Header("Player References")]
        [SerializeField] private PlayerMovement movement;
        [SerializeField] private Rigidbody body;
        [SerializeField] private CollisionCheck collisionCheck;

        private Animator animator;
        private PlayerInput playerInput;

        private float moveInput;

        private void Start()
        {
            animator = GetComponent<Animator>();
            playerInput = GeneralUtils.GetPlayerInput();

            playerInput.actions["Player/Move"].performed += ReceiveMoveInput;
            playerInput.actions["Player/Move"].canceled += ReceiveMoveInput;
        }

        private void Update()
        {
            animator.SetFloat("_Speed", GetSpeed());
            animator.SetBool("_Sliding", movement.IsSliding);
        }

        private void ReceiveMoveInput(InputAction.CallbackContext ctx)
        {
            moveInput = ctx.ReadValue<Vector2>().sqrMagnitude;
        }

        private float GetSpeed()
        {
            return collisionCheck.OnGround ? moveInput : body.velocity.Horizontal().sqrMagnitude;
        }
    }
}