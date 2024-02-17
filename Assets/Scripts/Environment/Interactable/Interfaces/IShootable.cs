using UnityEngine;

namespace fpsRed.Environment.Interactable
{
    public interface IShootable
    {
        void OnShot(RaycastHit hit, Ray initialRay);
    }
}