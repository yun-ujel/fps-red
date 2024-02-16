using UnityEngine;

namespace fpsRed.Player
{
    [RequireComponent(typeof(Collider))]
    public class CollisionCheck : MonoBehaviour
    {
        [Tooltip("If a collision's normal (on the Y axis) is above this value, the surface will be considered ground")]
        [SerializeField, Range(0f, 1f)] private float groundNormalThreshold = 0.9f;
        public bool OnGround { get; private set; }

        public event System.EventHandler<System.EventArgs> OnHitGroundEvent;

        private bool EvaluateCollision(Collision collision)
        {
            bool landed = false;
            for (int i = 0; i < collision.contactCount; i++)
            {
                landed |= collision.contacts[i].normal.y >= groundNormalThreshold;
            }
            OnGround |= landed;

            return landed;
        }

        private void OnCollisionEnter(Collision collision)
        {
            bool wasOnGround = OnGround;
            if (!wasOnGround && EvaluateCollision(collision))
            {
                OnHitGroundEvent?.Invoke(this, new());
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            _ = EvaluateCollision(collision);
        }

        private void OnCollisionExit(Collision collision)
        {
            OnGround = false;
        }
    }
}