using UnityEngine;

public class AudioRepo : MonoBehaviour
{
    public static AudioRepo Instance;
    
    [SerializeField] private AudioClip _mainSceneMusic;
    
    [SerializeField] private AudioClip _sheepDeath;
    [SerializeField] private AudioClip _baaaaah;
    [SerializeField] private AudioClip _shotgunShot;
    [SerializeField] private AudioClip _shapeShift;
    [SerializeField] private AudioClip _wolfDed;
    [SerializeField] private AudioClip _buttonClicks;
    
    private void Awake()
    {
        if (Instance != null) Destroy(Instance);

        Instance = this;
    }
}
