using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private bool isFacingRight = false;
    [Range(0.01f, 20.0f)] [SerializeField] private float moveSpeed = 0.1f; // moving speed of the player
    [Range(0.01f, 20.0f)] [SerializeField] private float moveRange = 1.0f;

    private Animator animator;
    [Range(0.01f, 20.0f)] [SerializeField] private float startPositionX;
    private bool isMovingRight = false;
    private PolygonCollider2D enemyColldier;
    // Start is called before the first frame update
    void Start()
    {
        enemyColldier = GetComponent<PolygonCollider2D>();
        startPositionX = this.transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        if(isMovingRight)
        {
            if (this.transform.position.x < moveRange + startPositionX)
            {
                isFacingRight = true;
                isMovingRight = true;              
                MoveRight();
            }
            else
            {
                Flip();
                isMovingRight = false;
                isFacingRight = false;
                MoveLeft();
            }
            
        }
        else
        {
            if (this.transform.position.x > startPositionX - moveRange)
            {
                isFacingRight = false;
                isMovingRight = false;
                MoveLeft();
            }
            else
            {
                isMovingRight = true;
                isFacingRight = true;
                Flip();
                MoveRight();
            }
        }
    }

    void Awake()
    {
        //rigidBody = GetComponent<Rigidbody2D>();
        startPositionX = this.transform.position.x;
        animator = GetComponent<Animator>();
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void MoveRight()
    {
        transform.Translate(moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
    }

    void MoveLeft()
    {
        transform.Translate(- (moveSpeed * Time.deltaTime), 0.0f, 0.0f, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if(other.gameObject.transform.position.y > transform.position.y)
            {
                enemyColldier.enabled = false;
                this.animator.SetBool("isDead", true);
                StartCoroutine(FadeAndDestroy());
            }
        }
    }

    IEnumerator FadeAndDestroy()
    {
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        Color startColor = spriteRenderer.color;
        Color endColor = new Color(startColor.r, startColor.g,startColor.b, 0f);
        float elapsedTime = 0;
        float fadeDuration = 1.0f;
        while(elapsedTime < fadeDuration)
        {
            spriteRenderer.color = Color.Lerp(startColor, endColor, elapsedTime/fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}