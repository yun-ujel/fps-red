using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fpsRed.Player
{
    [RequireComponent(typeof(Collider))]
    public class CollisionCheck : MonoBehaviour
    {
        [Tooltip("If a collision's normal (on the Y axis) is above this value, the surface will be considered ground")]
        [SerializeField, Range(0f, 1f)] private float groundNormalThreshold = 0.9f;

        public bool OnGround { get; private set; }

        private void EvaluateCollision(Collision collision)
        {
            for (int i = 0; i < collision.contactCount; i++)
            {
                OnGround |= collision.contacts[i].normal.y >= groundNormalThreshold;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            EvaluateCollision(collision);
        }

        private void OnCollisionStay(Collision collision)
        {
            EvaluateCollision(collision);
        }

        private void OnCollisionExit(Collision collision)
        {
            OnGround = false;
        }
    }
}