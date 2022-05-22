using System.Collections;
using Photon.Pun;
using UnityEngine;

public class Shotgun : MonoBehaviour
{
    [SerializeField] private TrailRenderer _bulletTrail;
    [SerializeField] private float _bulletTrailTravelTime;
    [SerializeField] private Transform _muzzle;
    [SerializeField] private int _magSize = 5;
    [SerializeField] private float _horizontalBulletSpread = 5f;
    [SerializeField] private float _verticalBulletSpread = 5f;
    [SerializeField] private int _numberOfBullets;

    [SerializeField] private float _maxBulletTravelDistance;
    [SerializeField] private LayerMask _interactionLayer;
    [SerializeField] private GameObject BulletParticles;
    public void Shoot()
    {
        Instantiate(BulletParticles, _muzzle.position, Quaternion.identity);
        
        if (!PhotonNetwork.IsMasterClient) return;
        
        for (int i = 0; i < _numberOfBullets; i++)
        {
            var direction = Quaternion.Euler(
                                Random.Range(-_horizontalBulletSpread, _horizontalBulletSpread), 
                                Random.Range(-_verticalBulletSpread, _verticalBulletSpread), 
                                0f) * -_muzzle.forward;

            if (Physics.Raycast(_muzzle.position, direction, out RaycastHit info, _maxBulletTravelDistance, _interactionLayer))
            {
                ProcessBulletHit(info);
            }
            else
            {
                // show bullet trace
                var bulletTrail = PhotonNetwork.Instantiate(_bulletTrail.name, _muzzle.position, Quaternion.identity);
                var trail = bulletTrail.GetComponent<TrailRenderer>();
                trail.time = _bulletTrailTravelTime;
                
                var pos = _muzzle.position + direction * _maxBulletTravelDistance;
                StartCoroutine(ProcessBulletTrail(trail, pos));
            }
        }
    }

    private void ProcessBulletHit(RaycastHit info)
    {
        // show bullet trace
        var bulletTrail = PhotonNetwork.Instantiate(_bulletTrail.name, _muzzle.position, Quaternion.identity);
        var trail = bulletTrail.GetComponent<TrailRenderer>();
        trail.time = _bulletTrailTravelTime;
        
        StartCoroutine(ProcessBulletTrail(trail, info.point));

        if (info.collider.TryGetComponent<DamageHandler>(out var damageHandler))
        {
            damageHandler.ProcessDamage();
        }
    }


    private IEnumerator ProcessBulletTrail(TrailRenderer trail, Vector3 goalPoint)
    {
        float elapsed = 0f;
        var startPos = trail.transform.position;
        
        while (elapsed < _bulletTrailTravelTime)
        {
            trail.transform.position = Vector3.Lerp(startPos, goalPoint, elapsed / trail.time);
            elapsed += Time.deltaTime;

            yield return null;
        }

        trail.transform.position = goalPoint;
        
        // spawn impact here
        if(PhotonNetwork.IsMasterClient)
            PhotonNetwork.Destroy(trail.gameObject);
    }

}