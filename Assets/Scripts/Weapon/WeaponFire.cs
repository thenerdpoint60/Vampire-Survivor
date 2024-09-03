using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace VampireSurvivor
{
    public class WeaponFire : MonoBehaviour
    {
        [SerializeField] private Transform firePoint;
        [SerializeField] private Transform weaponTransform;
        [SerializeField] private WeaponStatsSO weaponStats;
        [SerializeField] private float recoilDistance = 0.1f;
        [SerializeField] private float recoilDuration = 0.1f;
        [SerializeField] private AudioSource weaponSource;
        [SerializeField] private AudioClip weaponFireSfx;

        private float nextFireTime = 0f;
        private List<Collider2D> targetColliders = new();
        private bool pauseFiring;

        private void OnEnable()
        {
            EventManager.StartListening(GameEvents.GamePause, OnGamePauseStateChange);
        }

        private void OnDisable()
        {
            EventManager.StopListening(GameEvents.GamePause, OnGamePauseStateChange);
        }

        private void OnGamePauseStateChange(object obj)
        {
            pauseFiring = (bool)obj;
        }

        private void Update()
        {
            if (pauseFiring)
                return;

            if (Time.time >= nextFireTime && targetColliders.Count > 0)
            {
                Collider2D nearestTarget = FindNearestTarget();
                if (nearestTarget != null)
                {
                    Fire(nearestTarget.transform.position);
                    nextFireTime = Time.time + weaponStats.FireRate;
                }
            }
        }

        public void Fire(Vector2 targetPosition)
        {
            Vector2 direction = (targetPosition - (Vector2)firePoint.position).normalized;
            GameObject projectile = PoolManager.Instance.GetFromPool(GamePoolType.Projectile);
            weaponSource.PlayOneShot(weaponFireSfx);
            projectile.transform.position = firePoint.position;
            projectile.GetComponent<Projectile>().SetDirection(direction);
            ApplyRecoil();
        }

        private void ApplyRecoil()
        {
            Vector3 originalPosition = weaponTransform.localPosition;
            Vector3 recoilPosition = originalPosition - weaponTransform.right * recoilDistance;

            weaponTransform.DOLocalMove(recoilPosition, recoilDuration)
                .OnComplete(() => weaponTransform.DOLocalMove(originalPosition, recoilDuration));
        }

        public void AddTarget(Collider2D target)
        {
            if (!targetColliders.Contains(target))
            {
                targetColliders.Add(target);
            }
        }

        public void RemoveTarget(Collider2D target)
        {
            if (targetColliders.Contains(target))
            {
                targetColliders.Remove(target);
            }
        }

        private Collider2D FindNearestTarget()
        {
            Collider2D nearestTarget = null;
            float shortestDistance = Mathf.Infinity;

            foreach (var target in targetColliders)
            {
                if (target == null)
                {
                    continue;
                }

                float distance = Vector2.Distance(firePoint.position, target.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestTarget = target;
                }
            }

            return nearestTarget;
        }

    }
}