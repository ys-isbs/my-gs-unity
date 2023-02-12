using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class CannonBall : NetworkBehaviour
{
    [SerializeField] float destroyTime;
    [Networked] private TickTimer life { get; set; }

    // Start is called before the first frame update
    //void Start()
    //{
    //    Destroy(this.gameObject, destroyTime);
    //}

    public void Init()
    {
        life = TickTimer.CreateFromSeconds(Runner, destroyTime);
    }

    public override void FixedUpdateNetwork()
    {
        if (life.Expired(Runner))
        {
            Runner.Despawn(Object);
        }
    }
}
