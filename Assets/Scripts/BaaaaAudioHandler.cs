using UnityEngine;

public class BaaaaAudioHandler : MonoBehaviour
{
    [SerializeField] private AudioSource _source;

    [SerializeField] private AudioClip[] _clips;
    
    void Update()
    {
        if (Random.Range(0, 50000f) < 1f)
        {
            _source.PlayOneShot(_clips[Random.Range(0, _clips.Length)]);
        }
    }
}
