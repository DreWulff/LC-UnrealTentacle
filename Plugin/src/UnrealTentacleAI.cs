using System.Diagnostics;
using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;

namespace UnrealTentacle {

    [RequireComponent(typeof(Rigidbody))]
    partial class UnrealTentacleAI : EnemyAI
    {
#pragma warning disable 0649, CS8618
        [Tooltip("Cooldown for damage instances.")]
        [SerializeField] private float damageCooldown;
        [Tooltip("Damage done to the player when hit while close.")]
        [SerializeField] private int stingDamage;
        [Tooltip("Damage done to the player when hit by the projectile.")]
        [SerializeField] private int barbDamage;
        [SerializeField] private GameObject mapDotObj;
        [SerializeField] private GameObject colliderObj;
        [SerializeField] private GameObject modelObj;
        [SerializeField] private GameObject scanNodeObj;
        [SerializeField] private int copiesToBeSpawned = 3;

        private float timeSinceDamagingPlayer;
        private float timeOfDeath;
        private Vector3 deadStartingPosition;
        private Vector3 deadTargetPosition;
        private bool onCeiling = false;

        [HideInInspector]
        public Rigidbody rb;
#pragma warning restore 0649, CS8618
        enum State {
            ROAMING,
            ASLEEP,
            ATTACK,
        }

        [Conditional("DEBUG")]
        void LogIfDebugBuild(string text)
        {
            Plugin.Logger.LogInfo(text);
        }

        public override void Start()
        {
            base.Start();
            float randomSize = Random.Range(0.8f, 1f);
            gameObject.transform.transform.localScale = randomSize * gameObject.transform.transform.localScale;
            rb = gameObject.GetComponent<Rigidbody>();
            timeSinceDamagingPlayer = damageCooldown;
            LogIfDebugBuild("Unreal Tentacle Spawned");
            StartRoam();
        }
        
        public override void Update()
        {
            if (isEnemyDead) return;
            base.Update();
        }

        // Runs every frame.
        public void LateUpdate() {
            if (isEnemyDead)
            {
                if (timeOfDeath < 1f)
                {
                    timeOfDeath += Time.deltaTime;
                    transform.position = Vector3.Lerp(deadStartingPosition, deadTargetPosition, timeOfDeath * 10 / (deadTargetPosition - deadStartingPosition).magnitude);
                }
                return;
            }
            // Physics/visual updates depending on state.
            switch (currentBehaviourStateIndex)
            {
                case (int)State.ROAMING:
                    break;
                case (int)State.ASLEEP:
                    break;
                case (int)State.ATTACK:
                    AttackUpdate();
                    break;
                default:
                    break;
            }
        }

        // Runs every AIInterval. Disabled if inSpecialAnimation = true.
        public override void DoAIInterval() {
            
            if (isEnemyDead || StartOfRound.Instance.allPlayersDead)
            {
                return;
            };
            base.DoAIInterval();

            // Behaviour when in a squad.
            switch (currentBehaviourStateIndex)
            {
                // When roaming, if player is spotted, enter SPOTTED state.
                case (int)State.ROAMING:
                    RoamAI();
                    break;

                // When roaming, if player is spotted, enter SPOTTED state.
                case (int)State.ASLEEP:
                    if (spawned)
                    { SleepAI(); }
                    break;

                // When spotted a player, wait for animation to end
                // to attempt a chase. (event in Animation)
                case (int)State.ATTACK:
                    AttackAI();
                    return;

                default:
                    break;
            }
        }

        public override void HitEnemy(int force = 1, PlayerControllerB? playerWhoHit = null, bool playHitSFX = false, int hitID = -1) {
            base.HitEnemy(force, playerWhoHit, playHitSFX, hitID);
            if(isEnemyDead)
            { return; }
            enemyHP -= force;
            if (IsOwner)
            {
                if (enemyHP <= 0 && !isEnemyDead)
                {
                    // Our death sound will be played through creatureVoice when KillEnemy() is called.
                    // KillEnemy() will also attempt to call creatureAnimator.SetTrigger("KillEnemy"),
                    // so we don't need to call a death animation ourselves.

                    // We need to stop our search coroutine, because the game does not do that by default.
                    StopCoroutine(searchCoroutine);
                    KillEnemyOnOwnerClient();
                }
                else if (enemyHP > 0)
                {
                    DoAnimationServerRpc("Stun");
                }
            }
        }

        public override void KillEnemy(bool destroy = false)
        {
            base.KillEnemy(destroy);
            SetDeathPositionClientRpc();
        }

        [ServerRpc]
        public void DoAnimationServerRpc(string animationName)
        { DoAnimationClientRpc(animationName); }

        [ClientRpc]
        public void DoAnimationClientRpc(string animationName)
        { creatureAnimator.SetTrigger(animationName); }

        [ClientRpc]
        public void SetSleepSpeedClientRpc(float value)
        { creatureAnimator.SetFloat("AnimationSpeed", value); }

        [ClientRpc]
        public void SetDeathPositionClientRpc()
        {
            deadStartingPosition = transform.position;
            Ray ray = new Ray(eye.position, Vector3.down);
            if (Physics.Linecast(eye.position, eye.position + Vector3.down * 20, out var hit, StartOfRound.Instance.collidersAndRoomMaskAndDefault, QueryTriggerInteraction.Ignore))
            {
                deadTargetPosition = hit.point + new Vector3(0, 0.2f, 0);
            }
            timeOfDeath = 0f;
        }

        public void SpawnCopy()
        {
            if (IsOwner)
            {
                UnrealTentacleAI copy = Instantiate(enemyType.enemyPrefab, transform.position, transform.rotation, RoundManager.Instance.mapPropsContainer.transform).GetComponent<UnrealTentacleAI>();
                var instanceNetworkObject = copy.GetComponent<NetworkObject>();
                instanceNetworkObject.Spawn();
                copy.DontSpawnCopiesClientRpc();
            }
        }

        [ClientRpc]
        private void DontSpawnCopiesClientRpc()
        { copiesToBeSpawned = 0; }
    }
}