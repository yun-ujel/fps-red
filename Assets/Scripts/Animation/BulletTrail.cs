using System.Collections;
using UnityEngine;

using fpsRed.Player;
namespace fpsRed.Animation
{
    public class BulletTrail : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GunHand gunHand;
        [SerializeField] private Transform shotStart;

        [Space, SerializeField] private TrailRenderer trail;

        private void Start()
        {
            gunHand.OnShootEvent += OnShoot;
        }

        private void OnShoot(object sender, GunHand.OnShootEventArgs args)
        {
            Vector3 destination = args.Hit.point;
            if (args.Hit.point == Vector3.zero)
            {
                destination = args.InitialRay.origin + args.InitialRay.direction * args.MaxDistance;
            }

            TrailRenderer trailInstance = Instantiate(trail, shotStart.position, Quaternion.identity);
            _ = StartCoroutine(SpawnTrail(trailInstance, destination, 1 / trail.time, trail.time));
        }

        private IEnumerator SpawnTrail(TrailRenderer trail, Vector3 hitPoint, float trailSpeed, float destroyTime)
        {
            float time = 0f;

            while (time < 1)
            {
                if (time > 0f)
                {
                    trail.transform.position = hitPoint;
                }

                trail.widthMultiplier = Mathf.Lerp(trail.startWidth, 0f, time);
                time += Time.deltaTime * trailSpeed;

                yield return null;
            }

            Destroy(trail.gameObject);
        }
    }
}