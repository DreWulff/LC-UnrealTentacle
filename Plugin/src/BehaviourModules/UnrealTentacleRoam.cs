using Unity.Netcode;
using UnityEngine;

namespace UnrealTentacle
{
    partial class UnrealTentacleAI : EnemyAI
    {
        [SerializeField] private float searchRoofTimer;
        [SerializeField] private SpotVerifier spotVerifier;
        private Vector3 projectedPosition;
        private float searchRandomFactor;

        /// <summary>
        /// Behaviour entry point.
        /// </summary>
        private void StartRoam()
        {
            SwitchToBehaviourClientRpc((int)State.ROAMING);
            inSpecialAnimation = false;
            searchRandomFactor = Random.Range(0f, searchRoofTimer / 2);
            DisableScanNodeClientRpc();
            StartSearch(transform.position);
        }

        /// <summary>
        /// AI portion of the behaviour. Runs periodically every AIInterval.
        /// Disabled when inSpecialAnimation = true;
        /// </summary>
        private void RoamAI()
        {
            Ray ray = new Ray(transform.position, -Physics.gravity);
            if (Physics.Raycast(ray, out RaycastHit forwardHit, 20f, StartOfRound.Instance.collidersAndRoomMaskAndDefault))
            {
                if (forwardHit.distance < 15f
                    && forwardHit.distance > 2f
                    && searchRoofTimer <= searchRandomFactor)
                {
                    projectedPosition = forwardHit.point;
                    spotVerifier.transform.position = projectedPosition;
                    if (spotVerifier.collisionCounter <= 0)
                    { ValidateSpotClientRpc(); }
                }
            }
            if (searchRoofTimer <= 0)
            {
                StartSleep();
            }
            else
            {
                agent.speed = 500f;
                searchRoofTimer -= AIIntervalTime;
            }
        }

        [ClientRpc]
        public void ValidateSpotClientRpc()
        {
            StartSleep();
            transform.position = projectedPosition - new Vector3(0, 0.05f, 0);
            transform.eulerAngles = new Vector3(180, Random.Range(0, 360), 0);
        }

        [ClientRpc]
        private void DisableScanNodeClientRpc()
        {
            agent.enabled = true;
            scanNodeObj.SetActive(false);
        }
    }
}