using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccessTest : MonoBehaviour
{
    [SerializeField] HelloWorld helloWorld;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("外部から参照 " + helloWorld.languages[0]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
