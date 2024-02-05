using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    [Header("Movement parameters")]
    [Range(0.01f, 20.0f)][SerializeField] private float moveSpeed = 0.1f; // moving speed of the player
    [Space(10)][Range(0.01f, 20.0f)][SerializeField] private float jumpForce = 6.0f;

    private Rigidbody2D rigidBody;
    private Animator animator;
    private bool isWalking;
    private bool isFacingRight;
    private int keysFound = 0;
    private const int keysNumber = 3;
    private AudioSource source;

    [SerializeField] public LayerMask groundLayer;

    [SerializeField] public AudioClip bonusSound;
    [SerializeField] public AudioClip enemyKilled;
    [SerializeField] public AudioClip keySound;
    [SerializeField] public AudioClip deadSound;
    [SerializeField] public AudioClip heartSound;
    [SerializeField] public AudioClip jumpSound;
    [SerializeField] public AudioClip deathSound;
    [SerializeField] public AudioClip checkpointSound;

    [SerializeField] public GameObject secretDoor1;
    private bool leverPressed1 = false;
    private Renderer playerRenderer;
    private bool secretDoorIsMoving = false;
    [SerializeField] public AudioClip secretDoorSound;

    [SerializeField] public GameObject secretDoor2;
    private bool leverPressed2 = false;

    [SerializeField] public GameObject secretDoor3;
    private bool leverPressed3 = false;

    const float rayLength = 1.55f;
    public GameObject gameOverScreen;
    private int lives = 3;

    private Vector2 startPosition;
    public float shakeTime = 1.5f;
    public float shakeMagnitude = 0.1f;

    public List<BoxFall> FallingBoxes;
    public List<CrateController> CrateControllers;
    // Start is called before the first frame update
    void Start()
    {
        isWalking = false;
        isFacingRight = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.currentGameState == GameState.GS_GAME)
        {
            isWalking = false;
            if (!secretDoorIsMoving && (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))) // right
            {
                transform.Translate(moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
                isWalking = true;
                if (!isFacingRight)
                {
                    Flip();
                }
            }
            else if (!secretDoorIsMoving && (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))) // left
            {
                transform.Translate(-(moveSpeed * Time.deltaTime), 0.0f, 0.0f, Space.World);
                isWalking = true;
                if (isFacingRight)
                {
                    Flip();
                }
            }

            if((Input.GetKeyUp(KeyCode.K)))
            {
                Dying();
            }

            if (!secretDoorIsMoving && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)))
            {
                Jump();
                source.PlayOneShot(jumpSound, AudioListener.volume);
            }
            animator.SetBool("isGrounded", IsGrounded());
            animator.SetBool("isWalking", isWalking);
        }
    }

    void Awake()
    {
        GameObject.FindGameObjectsWithTag("SecretDoor").FirstOrDefault();
        playerRenderer = this.GetComponent<Renderer>();
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        startPosition = transform.position;
        source = GetComponent<AudioSource>();
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
    bool IsGrounded()
    {
        return Physics2D.Raycast(this.transform.position, Vector2.down, rayLength, groundLayer.value);
    }

    void Jump()
    {
        if (IsGrounded())
        {
            rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("ChangeRotationZ"))
        {
            rigidBody.freezeRotation = false;
        }

        if (other.CompareTag("NoRotation"))
        {
            rigidBody.freezeRotation = true;
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }

        if (other.CompareTag("Checkpoint") && (Vector3)startPosition!=other.transform.position)
        {
            startPosition = other.transform.position;
            source.PlayOneShot(checkpointSound, AudioListener.volume);
        }

        if (other.CompareTag("Bonus"))
        {
            GameManager.instance.AddPoints(10);
            other.gameObject.SetActive(false);
            source.PlayOneShot(bonusSound, AudioListener.volume);
        }

        if(other.CompareTag("Lever"))
        {
            if (!leverPressed1)
                StartCoroutine(MoveCameraToDoorPosition(secretDoor1));
            leverPressed1 = true;
        }

        if (other.CompareTag("Lever2"))
        {
            if (!leverPressed2)
                StartCoroutine(MoveCameraToDoorPosition(secretDoor2));
            leverPressed2 = true;
        }

        if (other.CompareTag("Lever3"))
        {
            if (!leverPressed3)
                StartCoroutine(MoveCameraToDoorPosition(secretDoor3));
            leverPressed3 = true;
        }

        if (other.CompareTag("MovingPlatform"))
        {
            transform.SetParent(other.transform);
        }

        if (other.CompareTag("Key"))
        {
            keysFound++;
            SpriteRenderer spriteRenderer = other.GetComponent<SpriteRenderer>();
            Color objectColor = spriteRenderer.color;
            GameManager.instance.AddKeys(objectColor);
            other.gameObject.SetActive(false);
            source.PlayOneShot(keySound, AudioListener.volume);
        }

        if (other.CompareTag("Heart"))
        {
            lives++;
            GameManager.instance.AddLife();
            other.gameObject.SetActive(false);
            source.PlayOneShot(heartSound, AudioListener.volume);

        }

        if (other.CompareTag("End") && keysFound == keysNumber)
        {
            GameManager.instance.AddPoints(100 * lives);
            GameManager.instance.LevelCompleted();
        }

        if (other.CompareTag("FallLevel"))
        {
            Dying();
        }

        if (other.CompareTag("Enemy"))
        {
            if (transform.position.y > other.gameObject.transform.position.y + 0.5f)
            {
                Vector2 velocity = rigidBody.velocity;
                velocity.y = jumpForce;
                rigidBody.velocity = velocity;
                GameManager.instance.AddPoints(30);
                GameManager.instance.AddEnemyKill();
                source.PlayOneShot(enemyKilled, AudioListener.volume);
            }
            else
            {
                Dying();
            }
        }
    }

    void Dying()
    {
        lives -= 1;
        source.PlayOneShot(deathSound, AudioListener.volume);
        if (lives <= 0)
        {
            GameManager.instance.EnableEndingScreen();
        }
        else
        {
            GameManager.instance.RemoveLife();
            this.transform.position = startPosition;
            foreach (var skrzynia in FallingBoxes)
            {
                BoxFall boxFall = skrzynia.GetComponent<BoxFall>();
                if (boxFall != null)
                {
                    boxFall.bringBack();
                }
            }
            foreach (var skrzynia in CrateControllers)
            {
                CrateController box = skrzynia.GetComponent<CrateController>();
                if (box != null)
                {
                    box.bringBack();
                }
            }
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if(transform.parent != null)
        {
            transform.SetParent(null);
        }
    }

    IEnumerator MoveCameraToDoorPosition(GameObject scrtDoor)
    {
        secretDoorIsMoving = true;
        Vector3 position = transform.position;
        Vector2 secretDoorPosition = scrtDoor.transform.position;
        playerRenderer.enabled = false;
        transform.position = scrtDoor.transform.position;
        yield return new WaitForSeconds(1.0f);
        source.PlayOneShot(secretDoorSound, AudioListener.volume);
        rigidBody.bodyType = RigidbodyType2D.Static;
        float elapsedTime = 0.0f;
        while (elapsedTime < shakeTime)
        {
            elapsedTime += Time.deltaTime;
            scrtDoor.transform.position = secretDoorPosition + new Vector2(Random.Range(-shakeMagnitude, shakeMagnitude), Random.Range(-shakeMagnitude, shakeMagnitude));
            yield return null;
        }
        Destroy(scrtDoor, 1.0f);
        yield return new WaitForSeconds(2.0f);
        rigidBody.bodyType = RigidbodyType2D.Dynamic;
        playerRenderer.enabled = true;
        transform.position = position;
        secretDoorIsMoving = false;
    }

    public void PutOnTopOfTheBox(float xPos, float yPos)
    {
        Vector3 newPosition = transform.position;
        newPosition.y = yPos;
        newPosition.x = xPos;
        transform.position = newPosition;
    }

}
