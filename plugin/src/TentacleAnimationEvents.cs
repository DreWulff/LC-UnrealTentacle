using UnityEngine;

namespace UnrealTentacle
{
    [RequireComponent (typeof(UnrealTentacleAI))]
    class TentacleAnimationEvents : MonoBehaviour
    {
        [SerializeField]
        private UnrealTentacleAI mainAI = null!;

        public void EndSpawn()
        { mainAI.Sleep(); }

        public void StartAttack()
        { mainAI.StartAttack(); }

        public void Shoot()
        { mainAI.ShootProjectile(); }

        public void EndStun()
        { mainAI.DoAnimationClientRpc("Alert"); }
    }
}
