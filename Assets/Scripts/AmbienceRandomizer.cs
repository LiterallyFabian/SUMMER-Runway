using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AmbienceRandomizer : MonoBehaviour
{
    void Start()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.time = Random.Range(0, audioSource.clip.length - 60);
    }
}