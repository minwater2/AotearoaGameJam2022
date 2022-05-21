using Photon.Pun;
using UnityEngine;


[RequireComponent(typeof(PlayerController))]
public class Player : MonoBehaviourPun
{
    public float MoveSpeed;
    
    private PlayerController _playerController;
    
    private Camera _camera;
    private Vector3 _mousePos;
    
    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
        _playerController = GetComponent<PlayerController>();

    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            ProcessInput();
            
            Rotate();
        }
    }

    private void ProcessInput()
    {
        Vector3 velocity = new Vector3(
            Input.GetAxisRaw("Horizontal") * MoveSpeed,
            0,
            Input.GetAxisRaw("Vertical") * MoveSpeed);
        _playerController.Move(velocity);
    }
    
    private void Rotate()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        
        if (groundPlane.Raycast(ray, out float rayDistance))
        {
            var point = ray.GetPoint(rayDistance);
            _playerController.LookAt(point);
        }
    }


}
