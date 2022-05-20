using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Movement : MonoBehaviourPun
{
    [SerializeField] private float _moveSpeed;
    private CharacterController _controller;
    
    // Start is called before the first frame update
    void Start()
    {
        _controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            ProcessInput();
        }
    }

    private void ProcessInput()
    {
        var movement = new Vector3
        {
            x = Input.GetAxisRaw("Horizontal"),
            y = 0f,
            z = Input.GetAxisRaw("Vertical"),
        }.normalized;

        _controller.SimpleMove(movement * _moveSpeed * Time.deltaTime);
    }
}
