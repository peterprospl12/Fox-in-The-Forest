using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BoxFall : MonoBehaviour
{
    Rigidbody2D rbody;
    Vector3 startPosition;
    public float shakeTime = 1.5f;
    public float shakeMagnitude = 0.1f;
    public bool shouldComeBack = true;
    GameObject player;
    PlayerController playerer;
    [SerializeField] public AudioClip destroySound;
    private BoxCollider2D boxCollider;
    private AudioSource source;
    private Renderer boxRenderer;
    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        boxRenderer = this.GetComponent<Renderer>();
        source = GetComponent<AudioSource>();
        startPosition = rbody.position;

        player = GameObject.FindGameObjectWithTag("Player");
        if(player!= null)
        {
            playerer = player.GetComponent<PlayerController>();
            if (playerer != null)
            {
                playerer.FallingBoxes.Add(this);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            if (this.CompareTag("BoxFall2"))
            {
                if (col.gameObject.transform.position.y > transform.position.y)
                {
                    source.PlayOneShot(destroySound, AudioListener.volume);
                    StartCoroutine(fullShakeCoroutine());
                }
            }
            else
            {
                float playerX = col.bounds.center.x;
                float boxLeft = boxCollider.bounds.min.x;
                float boxRight = boxCollider.bounds.max.x;
                if (col.gameObject.transform.position.y > transform.position.y && playerX > boxLeft && playerX < boxRight)
                {
                    source.PlayOneShot(destroySound, AudioListener.volume);
                    StartCoroutine(fullShakeCoroutine());
                }
            }
        }
    }

    IEnumerator fullShakeCoroutine()
    {
        yield return StartCoroutine(ShakePlatformAndDisable());

        yield return new WaitForSeconds(2.0f);

        if(shouldComeBack)
        {
            rbody.bodyType = RigidbodyType2D.Static;
            boxCollider.enabled = true;
            boxRenderer.enabled = true;
            transform.position = startPosition;
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            movePlayer();
        }
    }

    IEnumerator ShakePlatformAndDisable()
    {
        yield return StartCoroutine(ShakePlatform());

        yield return new WaitForSeconds(2.0f);

        boxCollider.enabled = false;
        boxRenderer.enabled = false;
    }

    IEnumerator ShakePlatform()
    {
        float elapsedTime = 0.0f;
        while (elapsedTime<shakeTime)
        {
            elapsedTime += Time.deltaTime;
            transform.position = startPosition + new Vector3(Random.Range(-shakeMagnitude, shakeMagnitude), Random.Range(-shakeMagnitude, shakeMagnitude), 0.0f);
            yield return null;
        }
        rbody.bodyType = RigidbodyType2D.Dynamic;
        rbody.AddForce(Vector2.up * 1.5f, ForceMode2D.Impulse);
    }

    public void bringBack()
    {
        StopAllCoroutines();
        if (boxCollider.enabled == false)
        {
            boxCollider.enabled = true;
        }
        if (boxRenderer.enabled == false)
        {
            boxRenderer.enabled = true;
        }
        if (rbody.bodyType == RigidbodyType2D.Dynamic)
        {
            rbody.bodyType = RigidbodyType2D.Static;
        }
        movePlayer();
        gameObject.SetActive(true);
        rbody.position = startPosition;
        transform.position = startPosition;
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        rbody.bodyType = RigidbodyType2D.Static;
    }

    private void movePlayer()
    {
        if (playerer != null)
        {
            float playerX = player.transform.position.x;
            float boxLeft = boxCollider.bounds.min.x;
            float boxRight = boxCollider.bounds.max.x;

            float playerY = player.transform.position.y;
            float boxBottom = boxCollider.bounds.min.y;
            float boxTop = boxCollider.bounds.max.y;

            if(playerX > boxLeft && playerX<boxRight && playerY> boxBottom && playerY<boxTop)
            {
                playerer.PutOnTopOfTheBox(boxCollider.transform.position.x, boxTop);
            }
        }
    }
}
