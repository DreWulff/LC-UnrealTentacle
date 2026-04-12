using Unity.Netcode;
using UnityEngine;

namespace UnrealTentacle
{
    partial class UnrealTentacleAI : EnemyAI
    {
        [SerializeField] private Transform tentacleBone = null!;
        [SerializeField] private Transform tentacleTip = null!;
        [SerializeField] private GameObject barbProjectile = null!;
        [SerializeField] private float barbSpeed = 0f;

        public void StartAttack()
        {
            SwitchToBehaviourClientRpc((int)State.ATTACK);
            DoAnimationClientRpc("Attack");
        }

        private void AttackUpdate()
        {
            if (IsOwner && targetPlayer != null)
            {
                SyncTargetPlayerServerRpc();
                targetPlayerPosition = targetPlayer.transform.position;
            }
            RotateTowardsPlayer(targetPlayerPosition);
        }

        private void AttackAI()
        {
            if (!TargetClosestPlayer())
            {
                Sleep();
            }
        }

        public void ShootProjectile()
        {
            if (IsOwner && targetPlayer != null)
            {
                ShootProjectile(targetPlayer.transform.position);
            }
        }

        public void ShootProjectile(Vector3 target)
        {
            TentacleProjectile barb = Instantiate(barbProjectile, tentacleTip.transform.position, tentacleTip.transform.rotation, RoundManager.Instance.mapPropsContainer.transform).GetComponent<TentacleProjectile>();
            barb.StartTrajectory((target + new Vector3(0, 2, 0) - tentacleTip.position).normalized * barbSpeed);
            var instanceNetworkObject = barb.GetComponent<NetworkObject>();
            instanceNetworkObject.Spawn();
            barb.SyncVelocity((target + new Vector3(0, 2, 0) - tentacleTip.position).normalized * barbSpeed);
        }

        private void RotateTowardsPlayer(Vector3 target)
        {
            tentacleBone.LookAt(target);
            if (transform.eulerAngles.z == 180)
            {
                tentacleBone.eulerAngles = new Vector3(tentacleBone.eulerAngles.x - 45, tentacleBone.eulerAngles.y, tentacleBone.localScale.z + 180);
            }
        }
    }
}
