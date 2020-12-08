using System;
using UnityEngine;

public class ShieldScript : MonoBehaviour
{
    public GameObject shieldToLaunch;

    public float forceFactor;

    private BoxCollider2D _collider;
    private PlayerMovement _parentMovement;

    private void Start()
    {
        _parentMovement = GetComponentInParent<PlayerMovement>();
        _collider = GetComponent<BoxCollider2D>();
    }
    
    

    // Update is called once per frame
    private void Update()
    {

        if (_parentMovement.GetShielded())
            _collider.enabled = true;


        if (!Input.GetKeyDown(KeyCode.Space) || !_parentMovement.GetShielded()) return;
       
        _collider.enabled = false;
        var facingRight = transform.parent.rotation.y == 0;
        if (facingRight)
        {
            var newShield = Instantiate(shieldToLaunch , transform.position + new Vector3(1f,1f,0), Quaternion.identity);
            var rb = newShield.GetComponent<Rigidbody2D>();
            var velocity = rb.velocity;
            velocity =  new Vector2(forceFactor, velocity.y + 1);
            rb.velocity = velocity;
        }
        else
        {
            var newShield = Instantiate(shieldToLaunch , transform.position - new Vector3(1f,-1f,0), Quaternion.identity);
            var rb = newShield.GetComponent<Rigidbody2D>();
            var velocity = rb.velocity;
            velocity =  new Vector2(-forceFactor, velocity.y + 1);
            rb.velocity = velocity;
        }

        _parentMovement.SetCanKill(false);
        _parentMovement.SetShielded(false);
    }
}