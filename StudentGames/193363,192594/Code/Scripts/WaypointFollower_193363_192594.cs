using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WaypointFollower : MonoBehaviour
{
    [SerializeField] private GameObject[] waypoints;
    private int currentWaypoint = 0;
    [SerializeField] [Range(0.01f, 20.0f)] private float speed = 0.5f;
    bool onCycle = false;
    GameObject gracz;
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            gracz = other.gameObject;
            if (!onCycle)
            {
                onCycle = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(onCycle)
        {
            float distance = Vector2.Distance(this.transform.position, waypoints[currentWaypoint].transform.position);
            if (distance < 0.1f)
            {
                currentWaypoint++;
                if (currentWaypoint > 2)
                {
                    onCycle = false;
                    gracz.transform.SetParent(null);
                    this.transform.position = waypoints[0].transform.position;
                    currentWaypoint = 0;
                    return;
                }
            }
            transform.position = Vector2.MoveTowards(this.transform.position, waypoints[currentWaypoint].transform.position, speed * Time.deltaTime);
        }
    }
}