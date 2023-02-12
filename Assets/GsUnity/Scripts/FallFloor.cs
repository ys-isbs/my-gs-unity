using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class FallFloor : NetworkBehaviour
{
    [SerializeField] float waitingTime = 3f;
    [SerializeField] float initializeTime = 5f;
    [SerializeField] Color warningColor = Color.red;

    Rigidbody rb;
    Vector3 initialPosition;
    Quaternion initialRotation;
    Color initialColor;
    MeshRenderer floorMeshRenderer;

    [Networked(OnChanged = nameof(OnWarning))]
    private NetworkBool isWarning { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        floorMeshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
        initialColor = floorMeshRenderer.material.color;
    }

    private static void OnWarning(Changed<FallFloor> changed)
    {
        changed.Behaviour.ChangeColor();
    }

    private void ChangeColor()
    {
        if (isWarning)
        {
            floorMeshRenderer.material.color = warningColor;
        }
        else
        {
            floorMeshRenderer.material.color = initialColor;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //floorMeshRenderer.material.color = warningColor;
            isWarning = true;
            StartCoroutine(Fall());
        }
    }

    IEnumerator Fall()
    {
        yield return new WaitForSeconds(waitingTime);
        rb.isKinematic = false;

        yield return new WaitForSeconds(initializeTime);
        Init();
    }

    void Init()
    {
        rb.isKinematic = true;
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        //floorMeshRenderer.material.color = initialColor;
        isWarning = false;
    }
}
