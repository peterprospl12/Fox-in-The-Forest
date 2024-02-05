using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformController : MonoBehaviour
{
    // Start is called before the first frame update
    [Range(0.01f, 20.0f)][SerializeField] private float moveSpeed = 0.1f; // moving speed of the player
    [Range(0.01f, 20.0f)][SerializeField] private float moveRange = 1.0f;
    [Range(0.01f, 20.0f)][SerializeField] private Vector2 startPosition;
    GameObject gracz;
    private bool onCycle = false;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(onCycle)
        {
            transform.Translate(-(moveSpeed * Time.deltaTime), 0.0f, 0.0f, Space.World);
            if(transform.position.x < startPosition.x - moveRange)
            {
                onCycle = false;
                gracz.transform.SetParent(null);
                this.transform.position = startPosition;
            }
        }
    }

    void Awake()
    {
        startPosition = this.transform.position;
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
}
