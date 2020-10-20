using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMovementeScript : MonoBehaviour
{

    [SerializeField]
    private float maxSpeed = 5;

    [SerializeField]
    private float jumpStrength = 5;

    [SerializeField]
    private float accelerationRate = 1f;
    [SerializeField]
    private float deaccelerationRate = 1f;

    private Rigidbody2D rb;
    private float axis;
    private float lerpValue = 0.5f;

    void Start()
    {
        this.rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.D))
        {
            lerpValue = Mathf.MoveTowards(lerpValue, 1f ,accelerationRate * Time.deltaTime);
        }
        else if(Input.GetKey(KeyCode.A))
        {
            lerpValue = Mathf.MoveTowards(lerpValue, 0f ,accelerationRate * Time.deltaTime);
        }
        else
        {
            lerpValue = Mathf.MoveTowards(lerpValue, 0.5f ,deaccelerationRate * Time.deltaTime);
        }

        axis = Mathf.Lerp(-1,1,lerpValue);
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(maxSpeed * axis,rb.velocity.y);
    }

}
