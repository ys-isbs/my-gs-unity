using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Fusion;

public class StartGate : NetworkBehaviour
{
    [SerializeField] GameObject gate;
    [SerializeField] GameObject podium;
    [SerializeField] GameObject npc;
    [Networked] TickTimer timer { get; set; }
    public UnityAction<int> OnCountEvent;
    int preCount;

    public void StartCountdown(int time)
    {
        timer = TickTimer.CreateFromSeconds(Runner, time);
    }

    public override void Render()
    {
        if (timer.IsRunning)
        {
            float remaining = timer.RemainingTime(Runner) ?? 0;
            int count = Mathf.CeilToInt(remaining);

            if (count != preCount)
            {
                OnCountEvent?.Invoke(count);
            }
            preCount = count;
        }

        if (timer.Expired(Runner))
        {
            gate.SetActive(false);
            podium.SetActive(false);
            npc.SetActive(false);
        }
    }
}
