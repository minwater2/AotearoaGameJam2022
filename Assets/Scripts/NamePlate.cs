using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NamePlate : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    private Camera _camera;
    
    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
    }

    public void Init(string nameText)
    {
        gameObject.SetActive(true);
        _nameText.text = nameText;
    }
    
    // Update is called once per frame
    void Update()
    {
        transform.LookAt(_camera.transform.position);
    }
}
