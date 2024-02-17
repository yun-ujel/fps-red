using UnityEngine;
using UnityEngine.InputSystem;

using fpsRed.Utilities;
using fpsRed.Environment.Interactable;

namespace fpsRed.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class GunHand : MonoBehaviour
    {
        public class OnPunchEventArgs : System.EventArgs
        {
            public Collider[] Hits { get; private set; }
            public OnPunchEventArgs(Collider[] hits)
            {
                Hits = hits;
            }
        }

        public class OnShootEventArgs : System.EventArgs
        {
            public RaycastHit Hit { get; private set; }
            public RaycastHit[] PierceHits { get; private set; }
            public Ray InitialRay { get; private set; }
            public float MaxDistance { get; private set; }

            public OnShootEventArgs(RaycastHit hit, RaycastHit[] pierceHits, Ray initialRay, float maxDistance)
            {
                Hit = hit;
                PierceHits = pierceHits;
                InitialRay = initialRay;
                MaxDistance = maxDistance;
            }
        }

        public event System.EventHandler<OnPunchEventArgs> OnPunchEvent;
        public event System.EventHandler<OnShootEventArgs> OnShootEvent;

        public bool IsAiming => punchHeld || queuedShot;

        [Header("References")]
        [SerializeField] private Transform lookTransform;

        [Header("Gun Controls")]
        [SerializeField, Range(0f, 2f)] private float shootBufferTime = 0.3f;
        [SerializeField] private float punchToAimTransitionTime;

        [Space, SerializeField] private LayerMask hitShotLayers;
        [SerializeField] private LayerMask pierceShotLayers;
        [SerializeField] private float maxShotRange = 100f;

        [Header("Punch Controls")]
        [SerializeField, Range(0f, 1f)] private float punchCooldown;

        [Space, SerializeField] private Transform punchPosition;
        [SerializeField] private LayerMask punchLayers;

        [Space, SerializeField, Range(0, 10)] private int punchChecks = 3;
        private int punchCheckCounter;

        [Header("Movement")]
        [SerializeField, Range(0f, 30f)] private float punchForce;
        [SerializeField, Range(0f, 30f)] private float gunRecoil;

        private PlayerInput playerInput;
        private Rigidbody body;

        private bool punchHeld;
        private float punchHeldCounter;
        private float timeSinceLastPunch;

        private bool queuedShot;

        private void Start()
        {
            playerInput = GeneralUtils.GetPlayerInput();
            body = GetComponent<Rigidbody>();

            playerInput.actions["Player/Fire"].performed += ReceiveFireInput;
            playerInput.actions["Player/Fire"].canceled += ReceiveFireInput;
        }

        private void ReceiveFireInput(InputAction.CallbackContext ctx)
        {
            if (ctx.performed && timeSinceLastPunch > punchCooldown)
            {
                Punch();
                return;
            }

            if (punchHeldCounter > shootBufferTime && punchHeld)
            {
                queuedShot = true;
            }

            punchHeld &= ctx.performed;
        }

        private void Punch()
        {
            body.velocity += punchForce * lookTransform.forward;

            punchHeldCounter = 0f;
            timeSinceLastPunch = 0f;

            punchCheckCounter = 0;

            punchHeld = true;

            Collider[] results = RunPunchCheck();

            OnPunchEvent?.Invoke(this, new OnPunchEventArgs(results));
        }

        private Collider[] RunPunchCheck()
        {
            Collider[] results = Physics.OverlapSphere(punchPosition.position, 1f, punchLayers, QueryTriggerInteraction.Collide);

            for (int i = 0; i < results.Length; i++)
            {
                if (results[i].TryGetComponent(out IPunchable punchable))
                {
                    punchable.OnPunched();
                }
            }

            punchCheckCounter++;

            return results;
        }

        private void Shoot()
        {
            body.velocity -= gunRecoil * lookTransform.forward;

            punchHeld = false;
            queuedShot = false;

            Ray shot = new Ray(lookTransform.position, lookTransform.forward);

            bool hitGround = Physics.Raycast(shot, out RaycastHit hit, maxShotRange, hitShotLayers, QueryTriggerInteraction.Collide);
            float maxDistance = hitGround ? hit.distance : maxShotRange;

            RaycastHit[] results = Physics.RaycastAll(shot, maxDistance, pierceShotLayers, QueryTriggerInteraction.Collide);

            for (int i = 0; i < results.Length; i++)
            {
                if (results[i].collider.TryGetComponent(out IShootable shootable))
                {
                    shootable.OnShot(results[i], shot);
                }
            }

            OnShootEvent?.Invoke(this, new OnShootEventArgs(hit, results, shot, maxDistance));
        }

        private void Update()
        {
            UpdateCounters();
        }

        private void FixedUpdate()
        {
            if (punchCheckCounter <= punchChecks)
            {
                _ = RunPunchCheck();
            }
        }

        private void UpdateCounters()
        {
            timeSinceLastPunch += Time.deltaTime;
            if (punchHeld)
            {
                punchHeldCounter += Time.deltaTime;
                return;
            }

            if (queuedShot && timeSinceLastPunch > punchToAimTransitionTime)
            {
                Shoot();
            }
        }
    }
}