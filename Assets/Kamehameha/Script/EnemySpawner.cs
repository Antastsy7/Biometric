using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyprefab, brokenprfeb;
    public float spawn_frequency = 3.0f;
    private float time_count;
    private Vector3 initial_position, initial_speed;
    
    // Start is called before the first frame update
    void Start()
    {
        time_count = Random.Range(0,spawn_frequency);   
    }

    // Update is called once per frame
    void Update()
    {
        time_count -= Time.deltaTime;
        if (time_count <= 0)
        {
            float x = Random.Range(-0.2f, 0.2f);
            float y = Random.Range(-0.2f, 0.2f);
            float z = Random.Range(0.5f, 1);
            initial_position = new Vector3(x, y, z) * 5000;

            
            initial_position = new Vector3(initial_position.x,Mathf.Abs(initial_position.y),Mathf.Abs(initial_position.z));
            initial_speed = -Vector3.Normalize(initial_position) * 20;

            GameObject enemy;
            if (Random.value > 0.5f)
            {
                enemy = Instantiate(enemyprefab);
            }
            else
            {
                enemy = Instantiate(brokenprfeb);
                enemy.GetComponent<SphereMovement>().breakable = true;
            }
            
            enemy.transform.position = initial_position;
            enemy.GetComponent<SphereMovement>().velocity_cr = initial_speed;
            enemy.GetComponent<SphereMovement>().velocity_hr = -1 * initial_speed;
            time_count = Random.Range(spawn_frequency, spawn_frequency * 2);
        }
    }
}
