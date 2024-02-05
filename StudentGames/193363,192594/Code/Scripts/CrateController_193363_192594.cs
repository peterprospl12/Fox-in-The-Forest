using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateController : MonoBehaviour
{

    Rigidbody2D rbody;
    Vector3 startPosition;
    GameObject player;
    PlayerController playerer;
    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        startPosition = rbody.position;

        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerer = player.GetComponent<PlayerController>();
            if (playerer != null)
            {
                playerer.CrateControllers.Add(this);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void bringBack()
    {
        StopAllCoroutines();
        rbody.position = startPosition;
        transform.position = startPosition;
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
    }
}
