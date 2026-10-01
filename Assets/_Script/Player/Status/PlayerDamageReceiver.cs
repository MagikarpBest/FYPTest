using UnityEngine;

    [RequireComponent(typeof(PlayerStatus))]
    public class PlayerDamageReceiver : MonoBehaviour
    {
        private PlayerStatus _playerStatus;

        private void Awake()
        {
            _playerStatus = GetComponent<PlayerStatus>();
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            IDamageSource damageable = hit.collider.GetComponentInParent<IDamageSource>();

            if (damageable == null)
                return;

            _playerStatus.TakeDamage(damageable.Damage);
        }

    #if UNITY_EDITOR
        [InspectorButton("DEBUG: Receive 1 damage", true)] // Only show button in play mode
        private void TestMethod()
        {
            _playerStatus.TakeDamage(1);
        }
    #endif
    }
