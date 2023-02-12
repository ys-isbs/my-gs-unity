using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class CannonManager : NetworkBehaviour
{
    [SerializeField] Cannon[] cannons;
    [SerializeField] float power;
    [SerializeField] float shotMinInterval;
    [SerializeField] float shotMaxInterval;
    bool isWaitingToShot;

    private void OnTriggerStay(Collider other)
    {
        if (Runner != null && Runner.IsClient) return;
        if (isWaitingToShot) return;

        if (other.CompareTag("Player"))
        {
            //if (!isWaitingToShot)
            //{
            //    StartCoroutine(ReadyToShot());
            //}

            isWaitingToShot = true;
            StartCoroutine(ReadyToShot());
        }
    }

    IEnumerator ReadyToShot()
    {
        //isWaitingToShot = true;

        float waitingTime = Random.Range(shotMinInterval, shotMaxInterval);
        int cannonIndex = Random.Range(0, cannons.Length);

        yield return new WaitForSeconds(waitingTime);
        cannons[cannonIndex].Shot(power);
        isWaitingToShot = false;
    }
}
