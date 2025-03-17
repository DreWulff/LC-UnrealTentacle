using Unity.Netcode;
using UnityEngine;

namespace UnrealTentacle
{
    partial class UnrealTentacleAI : EnemyAI
    {
        [SerializeField] private float range;
        private bool spottedPlayer = false;
        private bool spawned = false;
        private float lookAroundTimer;

        /// <summary>
        /// Behaviour entry point.
        /// </summary>
        public void StartSleep()
        {
            SwitchToBehaviourClientRpc((int)State.ASLEEP);
            for (int i = 1; i < copiesToBeSpawned; i++)
            { SpawnCopy(); }
            // Enable gameobjects
            StopSearch(currentSearch);
            inSpecialAnimation = false;
            agent.speed = 0f;
            agent.enabled = false;
            spottedPlayer = false;
            moveTowardsDestination = false;
            ReEnableEnemyClientRpc();
            SetSleepSpeedClientRpc(1);
        }

        /// <summary>
        /// Called after spawn animation ends.
        /// </summary>
        public void Sleep()
        {
            SwitchToBehaviourClientRpc((int)State.ASLEEP);
            spottedPlayer = false;
            lookAroundTimer = Random.Range(10, 20);
            spawned = true;
            DoAnimationClientRpc("Idle");
        }

        /// <summary>
        /// AI portion of the behaviour. Runs periodically every AIInterval.
        /// </summary>
        public void SleepAI()
        {
            if (TargetClosestPlayer() && !spottedPlayer)
            {
                spottedPlayer = true;
                DoAnimationClientRpc("Alert");
            }
            if (lookAroundTimer > 0)
            {
                lookAroundTimer -= AIIntervalTime;
            }
            else
            {
                DoAnimationClientRpc("LookAround");
                lookAroundTimer = Random.Range(10, 20);
            }
        }

        [ClientRpc]
        private void ReEnableEnemyClientRpc()
        {
            modelObj.SetActive(true);
            mapDotObj.SetActive(true);
            scanNodeObj.SetActive(true);
            colliderObj.SetActive(true);
        }
    }
}