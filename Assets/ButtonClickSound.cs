using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Button))]
public class ButtonClickSound : MonoBehaviour
{
    private AudioSource _audioSource;
    private Button _button;
    [SerializeField] private AudioClip _buttonClick;
    
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _button = GetComponent<Button>();
        _button.onClick.AddListener(PlayButtonClick);
    }

    private void PlayButtonClick()
    {
        _audioSource.PlayOneShot(_buttonClick, 0.25f);
    }
}
