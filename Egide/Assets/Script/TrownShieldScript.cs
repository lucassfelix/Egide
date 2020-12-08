using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrownShieldScript : MonoBehaviour
{
    [SerializeField] private String enemyTag = "Enemy";
    [SerializeField] private String groundTag = "Ground";
    [SerializeField] private String playerTag = "Player";
    [SerializeField] private String wallTag = "Wall";
    [SerializeField] private float timeAtWall = 3.5f;

    
    private Rigidbody2D _rb;
    private bool _canKill;
    private Animator _shieldAnimator;
    
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _canKill = true;
        _shieldAnimator = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {

        if (other.gameObject.CompareTag(groundTag))
        {
            _canKill = false;
            _shieldAnimator.SetBool("isSpinning", false);
        }
        else if (other.gameObject.CompareTag(playerTag))
        {
            other.gameObject.GetComponent<PlayerMovement>().SetShielded(true);
            Destroy(gameObject);
        }
        else if (other.gameObject.CompareTag(enemyTag) && _canKill)
        {
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag(wallTag))
        {
            _canKill = false;
            _rb.velocity = Vector2.zero;
            _rb.freezeRotation = true;
            _rb.gravityScale = 0;
            _shieldAnimator.SetBool("isSpinning", false);
            StartCoroutine(ShieldAtWallTimer());
        }
    }

    private void Update()
    {
        if (transform.position.x < -15f || transform.position.y < -8f)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    IEnumerator ShieldAtWallTimer()
    {
        
        yield return new WaitForSeconds(timeAtWall);

        _rb.freezeRotation = false;
        _rb.gravityScale = 1;
        _rb.AddTorque(-10f);
    }
}
