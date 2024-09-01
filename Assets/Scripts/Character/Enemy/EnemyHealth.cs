using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace VampireSurvivor
{
    public class EnemyHealth : CharacterHealth
    {
        [SerializeField] private UnityEvent<float> onHealthUpdated;
        [SerializeField] private GamePoolType poolType;
        [SerializeField] private GameObject parentTransform;
        [SerializeField] private Animator enemyAnimator;

        private void OnEnable()
        {
            StartMovingAnimation();
        }

        public override void Damage(int damage)
        {
            base.Damage(damage);

            //PlayHitAnimation();
            if (HasDied())
            {
                HandleDeath();
            }

            UpdateHealthUI();
        }

        private void UpdateHealthUI()
        {
            float currentHealthPercentage = (float)GetCurrentHealth / GetMaxHealth;
            onHealthUpdated?.Invoke(currentHealthPercentage);
        }

        private void PlayHitAnimation()
        {
            enemyAnimator.SetTrigger("Hit");
        }

        private void StartMovingAnimation()
        {
            enemyAnimator.SetBool("Moving", true);
        }

        private void HandleDeath()
        {
            enemyAnimator.SetTrigger("Dead");
            float animationDuration = enemyAnimator.GetCurrentAnimatorStateInfo(0).length;

            DOVirtual.DelayedCall(animationDuration, () =>
            {
                Debug.Log("Death animation completed");

                GameObject gameXP = PoolManager.Instance.GetFromPool(GamePoolType.XP);
                gameXP.transform.position = parentTransform.transform.position;

                PoolManager.Instance.ReturnToPool(poolType, parentTransform);
            });
        }
    }
}