using UnityEngine;
using UnityEngine.InputSystem;

using fpsRed.Utilities;
namespace fpsRed.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class GunHand : MonoBehaviour
    {
        public class OnPunchEventArgs : System.EventArgs
        {

        }

        public class OnFireEventArgs : System.EventArgs
        {

        }

        public event System.EventHandler<OnPunchEventArgs> OnPunchEvent;
        public event System.EventHandler<OnFireEventArgs> OnFireEvent;

        public bool IsAiming => fireHeld || queuedFire;

        [Header("References")]
        [SerializeField] private Transform lookTransform;

        [Header("Gun Controls")]
        [SerializeField, Range(0f, 2f)] private float fireBufferTime = 0.5f;
        [SerializeField] private float punchToAimTransitionTime;

        [Header("Punch Controls")]
        [SerializeField, Range(0f, 1f)] private float punchCooldown;

        [Header("Movement")]
        [SerializeField, Range(0f, 30f)] private float punchForce;
        [SerializeField, Range(0f, 30f)] private float gunRecoil;

        private PlayerInput playerInput;
        private Rigidbody body;

        private bool fireHeld;
        private float fireHeldCounter;
        private float timeSinceLastPunch;

        private bool queuedFire;

        private void Start()
        {
            playerInput = GeneralUtils.GetPlayerInput();
            body = GetComponent<Rigidbody>();

            playerInput.actions["Player/Fire"].performed += ReceiveFireInput;
            playerInput.actions["Player/Fire"].canceled += ReceiveFireInput;
        }

        private void ReceiveFireInput(InputAction.CallbackContext ctx)
        {
            fireHeld = ctx.performed;

            if (ctx.performed && timeSinceLastPunch > punchCooldown)
            {
                Punch();
                return;
            }

            if (fireHeldCounter > fireBufferTime)
            {
                queuedFire = true;
            }
        }

        private void Punch()
        {
            body.velocity += punchForce * lookTransform.forward;

            fireHeldCounter = 0f;
            timeSinceLastPunch = 0f;

            OnPunchEvent?.Invoke(this, new OnPunchEventArgs());
        }

        private void Fire()
        {
            body.velocity -= gunRecoil * lookTransform.forward;
            queuedFire = false;

            OnFireEvent?.Invoke(this, new OnFireEventArgs());
        }

        private void Update()
        {
            UpdateCounters();
        }

        private void UpdateCounters()
        {
            timeSinceLastPunch += Time.deltaTime;
            if (fireHeld)
            {
                fireHeldCounter += Time.deltaTime;
                return;
            }

            if (queuedFire && timeSinceLastPunch > punchToAimTransitionTime)
            {
                Fire();
            }
        }
    }
}