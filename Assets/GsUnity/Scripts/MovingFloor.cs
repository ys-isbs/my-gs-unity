using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class MovingFloor : NetworkBehaviour
{
    //[SerializeField] float movementSpeed = 1f;
    //float angleRange = 10f;
    [SerializeField] float movingSpeed;
    [SerializeField] float movingRange;
    [SerializeField] float offset;
    Transform body;
    [Networked] float cycleCount { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        body = transform.GetChild(0);
    }

    public override void FixedUpdateNetwork()
    {
        cycleCount += Runner.DeltaTime;
        //float cycle = Mathf.Sin(Time.time * movingSpeed + offset);
        var cycle = Mathf.Sin(cycleCount * movingSpeed + offset);
        //angleValue = cycle * angleRange;
        //Vector3 localPos = body.localPosition;
        //localPos.x = angleValue;
        //body.localPosition = localPos;
        body.localPosition = new Vector3(cycle * movingRange, 0, 0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(body.transform);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}
