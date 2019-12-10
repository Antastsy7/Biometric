using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SphereMovement : MonoBehaviour
{
    public Vector3 velocity_cr;
    public Vector3 scale_cr = new Vector3(5, 5, 5);
    public Vector3 velocity_hr;

    public float life = 100.0f;
    public bool breakable = false;

    private float lifespan = 15.0f; 
    private GameObject manager;
    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.FindWithTag("GameManager");
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += velocity_cr;
        transform.localScale += scale_cr;
        lifespan -= Time.deltaTime;
        if (lifespan <= 0.0f || life <= 0.0f )
        {
            if (life <= 0.0f)
            {
                manager.GetComponent<MyGestureController>().score += 100;
            }
            Destroy(gameObject);
        }

        if (Vector3.Distance(transform.position, new Vector3(0, 4, -313)) < transform.localScale.magnitude / 3)
        {
            manager.SendMessage("Gethit");
            Destroy(gameObject);
        }
        
        
    }

    void GetMessage(float distance)
    {
        
        if (breakable) life -= 1;
        else
        {
            velocity_cr += velocity_hr/transform.localScale.magnitude * (Mathf.Pow(100 / distance, 2));
            manager.GetComponent<MyGestureController>().score += 1;
        }
    }

}
