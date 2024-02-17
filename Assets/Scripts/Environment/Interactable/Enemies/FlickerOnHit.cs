using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fpsRed.Environment.Interactable.Enemies
{
    public class FlickerOnHit : MonoBehaviour, IPunchable, IShootable
    {
        private float timeSinceHit;
        private Renderer visual;

        private void Start()
        {
            visual = GetComponent<Renderer>();
        }

        public void OnPunched()
        {
            visual.enabled = false;
            timeSinceHit = 0f;
        }

        public void OnShot(RaycastHit hit, Ray ray)
        {
            visual.enabled = false;
            timeSinceHit = 0f;
        }

        private void Update()
        {
            if (timeSinceHit > 1f)
            {
                visual.enabled = true;
                return;
            }
            timeSinceHit += Time.deltaTime;
        }
    }
}