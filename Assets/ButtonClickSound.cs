using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Button))]
public class ButtonClickSound : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSourceCustom;
    private AudioSource _source;
    private Button _button;
    [SerializeField] private AudioClip _buttonClick;
    
    private void Awake()
    {
        _source = _audioSourceCustom ? _audioSourceCustom : GetComponent<AudioSource>();
        _button = GetComponent<Button>();
        _button.onClick.AddListener(PlayButtonClick);
    }

    private void PlayButtonClick()
    {
        _source.PlayOneShot(_buttonClick, 0.25f);
    }
}
