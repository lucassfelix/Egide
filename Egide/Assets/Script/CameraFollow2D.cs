using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{

    [SerializeField] private GameObject playerGameObject = null;

    [SerializeField] private float timeOffset = 0;

    [SerializeField]
    private Vector2 posOffset = Vector2.zero;

    private Vector3 _cameraCurrentPos;
    private Vector3 _playerCurrentPos;

    [SerializeField]
    private float leftLimit = 0;
    [SerializeField]
    private float rightLimit = 0;
    [SerializeField]
    private float bottomLimit = 0;
    [SerializeField]
    private float topLimit = 0;

    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _cameraCurrentPos = transform.position;
        _playerCurrentPos = playerGameObject.transform.position + new Vector3(posOffset.x,posOffset.y,0);
        _playerCurrentPos.z = -10;

        transform.position = Vector3.Lerp(_cameraCurrentPos,_playerCurrentPos, timeOffset * Time.deltaTime);
        
        transform.position = new Vector3
        (
            Mathf.Clamp(transform.position.x, leftLimit,rightLimit),
            Mathf.Clamp(transform.position.y, bottomLimit,topLimit),
            -10
        );
        
    }
}
