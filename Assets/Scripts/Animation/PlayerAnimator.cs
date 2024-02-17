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
        [SerializeField] private Rigidbody body;
        [SerializeField] private CollisionCheck collisionCheck;

        [Space]

        [SerializeField] private PlayerMovement movement;
        [SerializeField] private GunHand gunHand;

        private Animator animator;
        private PlayerInput playerInput;

        private float moveInput;

        private void Start()
        {
            animator = GetComponent<Animator>();
            playerInput = GeneralUtils.GetPlayerInput();

            playerInput.actions["Player/Move"].performed += ReceiveMoveInput;
            playerInput.actions["Player/Move"].canceled += ReceiveMoveInput;

            gunHand.OnPunchEvent += OnPunch;
            gunHand.OnShootEvent += OnFire;
        }

        private void Update()
        {
            animator.SetFloat("_Speed", GetSpeed());
            animator.SetBool("_Sliding", movement.IsSliding);
            animator.SetBool("_Aiming", gunHand.IsAiming);
        }

        private void ReceiveMoveInput(InputAction.CallbackContext ctx)
        {
            moveInput = ctx.ReadValue<Vector2>().sqrMagnitude;
        }

        private void OnPunch(object sender, GunHand.OnPunchEventArgs args)
        {
            animator.Play("Punch");
        }

        private void OnFire(object sender, GunHand.OnShootEventArgs args)
        {
            animator.Play("Fire");
        }

        private float GetSpeed()
        {
            return collisionCheck.OnGround ? moveInput : body.velocity.Horizontal().sqrMagnitude;
        }
    }
}