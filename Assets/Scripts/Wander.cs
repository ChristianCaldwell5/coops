using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static UnityEngine.GraphicsBuffer;

public class wander : MonoBehaviour
{

    // TODO: Move this to game manager. Testing for now
    
    float cameraHeight;
    float cameraWidth;
    float aspectRatio;
    float idleBoundWidth;
    float idleBoundHeight;
    float movementSpeed = 1.0f;
    float tolerance = 0.01f;
    bool isMoving = false;
    Vector2 target;

    private Coroutine currentMovementCoroutine;
    private SpriteRenderer mainSprite;
    private ShadowCaster2D shadowCaster;
    private Vector3[] originalShapePath;

    // Start is called before the first frame update
    void Start()
    {
        // get sprite renderers
        mainSprite = GetComponent<SpriteRenderer>();
        shadowCaster = GetComponent<ShadowCaster2D>();
        originalShapePath = shadowCaster.shapePath.Clone() as Vector3[];

        Camera mainCamera = Camera.main;
        cameraHeight = 2f * mainCamera.orthographicSize;
        idleBoundHeight = cameraHeight - 1;
        aspectRatio = (float)Screen.width / (float)Screen.height;
        cameraWidth = cameraHeight * aspectRatio;
        idleBoundWidth = cameraWidth - 1;
        StartCoroutine(WanderMovement());

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    IEnumerator WanderMovement()
    {
        float xPos;
        float yPos;
        float pauseBeforWander;
        // update to use game manager 
        while (true)
        {
            if(!isMoving)
            {
                Debug.Log("Not moving, finding new locale and starting");
                pauseBeforWander = Random.Range(2, 6);
                yield return new WaitForSeconds(pauseBeforWander);
                xPos = Random.Range(-idleBoundWidth / 2, idleBoundWidth / 2);
                yPos = Random.Range(-idleBoundHeight / 2, idleBoundHeight / 2);
                target = new Vector2(xPos, yPos);
                // Stop the current movement coroutine if it's running
                if (currentMovementCoroutine != null)
                {
                    StopCoroutine(currentMovementCoroutine);
                }
                currentMovementCoroutine = StartCoroutine(MoveTowardsLocale(target));
                Debug.Log("Position is: " + xPos + ", " + yPos);
            }
            yield return null;
        }
    }

    IEnumerator MoveTowardsLocale(Vector2 target)
    {
        FlipSprite(target);
        Debug.Log("Starting to move!");
        isMoving = true;
        while(Vector2.Distance(transform.position, target) > tolerance)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, movementSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = target;
        isMoving = false;
    }

    private void FlipSprite(Vector2 t)
    {
        if (t.x < transform.position.x && !mainSprite.flipX)
        {
            mainSprite.flipX = true;
        } else if (t.x > transform.position.x && mainSprite.flipX)
        {
            mainSprite.flipX = false;
        }
        // else, do nothing
    }
}
