using UnityEngine;

namespace UnrealTentacle
{
    class SpotVerifier : MonoBehaviour
    {
        [HideInInspector] public int collisionCounter = 0;

        void OnTriggerEnter(Collider other)
        {
            if ((StartOfRound.Instance.collidersAndRoomMaskAndDefault & (1 << other.gameObject.layer)) != 0)
            { collisionCounter++; }
        }

        void OnTriggerExit(Collider other)
        {
            if ((StartOfRound.Instance.collidersAndRoomMaskAndDefault & (1 << other.gameObject.layer)) != 0)
            { collisionCounter--; }
        }
    }
}
