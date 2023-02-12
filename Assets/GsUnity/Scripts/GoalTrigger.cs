using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Fusion;

public class GoalTrigger : MonoBehaviour
{
    public UnityAction<NetworkObject> OnGoalEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") == false) return;

        if (other.TryGetComponent(out NetworkObject networkPlayer))
        {
            OnGoalEvent?.Invoke(networkPlayer);
        }
    }
}
