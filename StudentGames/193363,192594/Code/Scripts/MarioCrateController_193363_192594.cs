using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarioCrateController : MonoBehaviour
{
    Rigidbody2D rbody;
    Vector3 startPosition;
    public float shakeTime = 0.2f;
    public float shakeMagnitude = 0.1f;
    [SerializeField] public AudioClip destroySound;
    private AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        startPosition = rbody.position;
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            if (col.gameObject.transform.position.y < transform.position.y)
            {
                source.PlayOneShot(destroySound, AudioListener.volume);

                StartCoroutine(ShakePlatform());

                Destroy(gameObject, 0.5f);
            }
        }
    }

    IEnumerator ShakePlatform()
    {
        float elapsedTime = 0.0f;
        while (elapsedTime < shakeTime)
        {
            elapsedTime += Time.deltaTime;
            transform.position = startPosition + new Vector3(Random.Range(-shakeMagnitude, shakeMagnitude), Random.Range(-shakeMagnitude, shakeMagnitude), 0f);
            yield return null;
        }
        rbody.bodyType = RigidbodyType2D.Dynamic;
        rbody.AddForce(Vector2.up * 1.5f, ForceMode2D.Impulse);
    }
}
