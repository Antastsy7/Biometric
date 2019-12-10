using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burst : MonoBehaviour
{
    public GameObject headeffect;
    public float lifespan = 10.0f;
    private Transform core, line, head;
    private float origin_length, current_length;
    private Vector3[] origin_scale;
    private RaycastHit hit;
    
    // Start is called before the first frame update
    void Start()
    {
        head = Instantiate(headeffect).transform;
    }
    
    

    // Update is called once per frame
    void Update()
    {
           
    }
    private void FixedUpdate()
    {
        //transform.Rotate(0.0f,0.1f,0.0f);
        Vector3 forward = transform.TransformDirection(Vector3.forward) * origin_length;
        Debug.DrawRay(transform.position, forward, Color.green);
        Ray ray = new Ray(transform.position, forward);

        
        
        if (Physics.Raycast(ray, out hit, 6000f))
        {
            head.position = hit.point;
            head.gameObject.SetActive(true);
            hit.transform.gameObject.SendMessage("GetMessage", Vector3.Distance(hit.point, core.position));

            current_length = Vector3.Distance(core.position, head.position);
            for (int i = 0; i < line.childCount; i++)
            {
                line.GetChild(i).localScale = new Vector3(origin_scale[i].x, origin_scale[i].y, origin_scale[i].z * current_length / origin_length);
            }
            
        }
        else
        {
            head.gameObject.SetActive(false);
            current_length = 6000f;
            for (int i = 0; i < line.childCount; i++)
            {
                line.GetChild(i).localScale = new Vector3(origin_scale[i].x, origin_scale[i].y, origin_scale[i].z * current_length / origin_length);
            }
        }
        
        
    }

    private void Awake()
    {
        core = transform.GetChild(0);
        line = transform.GetChild(1);
        origin_length = 1500f;
        origin_scale = new Vector3[10];
        for (int i = 0; i < line.childCount; i++)
        {
            origin_scale[i] = line.GetChild(i).localScale;
        }

    }

    private void OnDestroy()
    {
        Destroy(head.gameObject);
    }
}
