using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fpsRed.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class Gravity : MonoBehaviour
    {
        [SerializeField, Range(0f, 10f)] private float gravityScale = 1f;
        private Rigidbody body;

        private void Start()
        {
            body = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            Vector3 gravity = gravityScale * -9.81f * Vector3.up;
            body.AddForce(gravity, ForceMode.Acceleration);
        }
    }
}