using UnityEngine;

namespace UnrealTentacle
{
    class TentacleAnimationEvents : MonoBehaviour
    {
        [SerializeField]
        private UnrealTentacleAI mainAI;

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
