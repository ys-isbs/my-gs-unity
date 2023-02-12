using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Cannon : NetworkBehaviour
{
    [SerializeField] GameObject cannonBallPrefab;
    [SerializeField] Transform muzzle;
    [SerializeField] float rotationSpeed = 1f;
    [SerializeField] float angleRange = 60f;
    [SerializeField] Transform cannonFx;
    [SerializeField] GameObject shotFx;
    Transform body;
    float angleValue;
    [SerializeField] AudioClip shotSe;
    AudioSource audioSource;
    [Networked] float cycleCount { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        body = transform.GetChild(0);
        audioSource = GetComponent<AudioSource>();
    }

    public override void FixedUpdateNetwork()
    {
        cycleCount += Runner.DeltaTime;
        float cycle = Mathf.Sin(cycleCount * rotationSpeed);
        angleValue = cycle * angleRange;
        body.localRotation = Quaternion.AngleAxis(angleValue, Vector3.up);
    }

    public void Shot(float power)
    {
        if (Runner.IsClient) return;

        //var cannonBall = Instantiate(cannonBallPrefab, muzzle.position, Quaternion.identity);
        var cannonBall = Runner.Spawn(
            cannonBallPrefab,
            muzzle.position,
            Quaternion.identity,
            null,
            (Runner, ball) => ball.GetComponent<CannonBall>().Init()
            );

        var cannonBallRb = cannonBall.GetComponent<Rigidbody>();
        cannonBallRb.AddForce(muzzle.forward * power, ForceMode.VelocityChange);


        RPC_Effect();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_Effect()
    {
        audioSource.PlayOneShot(shotSe);
        Instantiate(shotFx, cannonFx);
    }
}
