using System.Collections.Generic;
using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;

namespace UnrealTentacle
{
    partial class UnrealTentacleAI : EnemyAI
    {
        private List<PlayerControllerB> closePlayers = new List<PlayerControllerB>();
        private List<PlayerControllerB> onSightPlayers = new List<PlayerControllerB>();
        private Vector3 targetPlayerPosition;

        /// <summary>
        /// Function to find the closest player.
        /// </summary>
        /// <returns></returns>
        private bool TargetClosestPlayer()
        {
            mostOptimalDistance = 2000f;
            targetPlayer = null;
            for (int i = 0; i < StartOfRound.Instance.connectedPlayersAmount + 1; i++)
            {
                tempDist = Vector3.Distance(transform.position, StartOfRound.Instance.allPlayerScripts[i].transform.position);
                if (tempDist < mostOptimalDistance
                    && CheckLineOfSightForPosition(StartOfRound.Instance.allPlayerScripts[i].transform.position)
                    && tempDist < range)
                {
                    mostOptimalDistance = tempDist;
                    targetPlayer = StartOfRound.Instance.allPlayerScripts[i];
                }
            }
            if (targetPlayer != null) return true;
            return false;
        }

        public bool CheckLineOfSightForPosition(Vector3 objectPosition)
        {
            if (Physics.Linecast(eye.position, objectPosition, out var _, StartOfRound.Instance.collidersAndRoomMaskAndDefault, QueryTriggerInteraction.Ignore))
            {
                return false;
            }
            return true;
        }

        [ServerRpc]
        public void SyncTargetPlayerServerRpc()
        {
            if (targetPlayer == null) return;
            ReceiveTargetPlayerClientRpc(targetPlayer.transform.position);
        }

        [ClientRpc]
        public void ReceiveTargetPlayerClientRpc(Vector3 target)
        {
            if (!IsOwner) targetPlayerPosition = target;
        }
    }
}
