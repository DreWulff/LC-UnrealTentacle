using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;

namespace UnrealTentacle
{
    [RequireComponent(typeof(Rigidbody))]
    class TentacleProjectile : NetworkBehaviour
    {
        private Rigidbody rb = null!;
        private float TTL;

        void Awake()
        {
            TTL = 3f;
            rb = GetComponent<Rigidbody>();
        }

        void Update()
        {
            if (TTL < 0)
            { Destroy(gameObject); }
            else
            { TTL -= Time.deltaTime; }
        }

        public void StartTrajectory(Vector3 speed)
        {
            rb.velocity = speed;
            transform.LookAt(transform.position + speed);
            transform.Rotate(-90, 0, 0);
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.GetComponent<PlayerControllerB>().DamagePlayer(Plugin.BoundConfig.barbDamage.Value);
                Destroy(gameObject);
            }
            else if ((1051400 & (1 << other.gameObject.layer)) != 0)
            {
                Destroy(gameObject);
            }
        }

        [ClientRpc]
        private void StartTrajectoryClientRpc(Vector3 speed)
        {
            if (IsOwner) return;
            rb.velocity = speed;
            transform.LookAt(transform.position + speed);
            transform.Rotate(-90, 0, 0);
        }

        public void SyncVelocity(Vector3 velocity)
        {
            StartTrajectoryClientRpc(velocity);
        }
    }
}
