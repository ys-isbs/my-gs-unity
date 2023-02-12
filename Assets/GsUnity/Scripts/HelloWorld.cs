using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelloWorld : MonoBehaviour
{
    //string message = "Hello World";
    //string[] languages = new string[] { "HTML/CSS", "JS", "PHP" };
    public List<string> languages = new List<string> { "HTML/CSS", "JS", "PHP" };

    // Start is called before the first frame update
    void Start()
    {
        //languages[2] = "C#";
        languages.Add("C#");

        for (int i = 0; i < languages.Count; i++)
        {
            Debug.Log(languages[i]);
        }
        //string newMessage = PrintText("ハローワールド");
        //Debug.Log(newMessage);
    }

    public string PrintText(string text)
    {
        return text + "!!!!!";
    }

    // Update is called once per frame
    void Update()
    {
     
    }
}
