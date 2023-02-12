using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip goalSe;

    public void PlayGoalSe()
    {
        audioSource.PlayOneShot(goalSe);
    }
}
