using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.UI;

public enum GirlDamageTaken
{
    NoDamage,

    /// <summary>
    /// Damage was taken and the girl died
    /// </summary>
    Died,

    /// <summary>
    /// Damage was taken and the girl is still alive
    /// </summary>
    Alive
}


public class Girl : MonoBehaviour
{
    public float Speed;
    public float JumpStrength;
    public float fastestReJumpTime;
    public bool IsContollerTarget;

    public AudioClip JumpSound;
    public AudioClip AirJumpSound;

    public List<GameObject> RemainigHearts;

    new Rigidbody2D rigidbody;

    ReloadManager reloadManager;

    HashSet<Platform> standingOnPlatforms;
    float lastJumpTime = 0;
    bool wasJumpDown = true;

    const int totalJumpAirJumpCount = 1;
    int remainingAirJumps = totalJumpAirJumpCount;

    bool recoveringFromDamage = false;

    SpriteRenderer spriteRenderer;
    Animator animator;
    AudioSource audioSource;
    
    //Variables added by Connor Simmons:
    [SerializeField] float platformInvulnDuration;
    [SerializeField] private int maxHearts;
    [SerializeField] private float stunShooterDuration;
    [SerializeField] private float stunLaserSpeed;
    [SerializeField] private float stunLaserShootSpeed;
    [SerializeField] private GameObject stunLaserPrefab;
    [SerializeField] private GameObject robot;
    [SerializeField] private float distPushedDown;   
    private int currentHearts;
    private bool canStunRobot = false;
    

    //Coroutine currentRespawnAnimation;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        standingOnPlatforms = new HashSet<Platform>();
    }

    private void Start()
    {
        reloadManager = FindObjectOfType<ReloadManager>();
        currentHearts = maxHearts;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        ControlMovement();
        CheckFallen();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var platform = collision.collider.GetComponent<Platform>();
        if (platform != null)
        {
            float highestContactY = float.NegativeInfinity;
            foreach (var contact in collision.contacts)
            {
                if (contact.point.y > highestContactY) highestContactY = contact.point.y;
            }
            if (highestContactY < transform.position.y)
            {
                standingOnPlatforms.Add(platform);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        var platform = collision.collider.GetComponent<Platform>();
        if (platform != null)
        {
            standingOnPlatforms.Remove(platform);
        }
    }

    /// <summary>
    /// Check whether the girl has fallen off the screen
    /// </summary>
    protected void CheckFallen()
    {
        var mainCam = Camera.main;

        float camVerticalSize = mainCam.orthographicSize;

        float camBottomY = mainCam.transform.position.y - camVerticalSize;
        if (transform.position.y < camBottomY)
        {
            // Has fallen. Deal damage or re-start game
            if (TakeDamage() != GirlDamageTaken.Died)
            {
                ResetPosition();
            }
        }
    }

    public GirlDamageTaken TakeDamage()
    {
        if (recoveringFromDamage) return GirlDamageTaken.NoDamage;

        if (currentHearts == 1)
        {
            reloadManager.Died();
            return GirlDamageTaken.Died;
        }
        else
        {
            //The lines commented out here were lines from the original codebase that were the logic for
            //keeping track of the characters hp. Since I had to edit this function and didnt want to claim 
            //that i made the entire function by moving it to the section with my other functions I left them  
            //in the code but commented out 
            
            var rightmostHeart = RemainigHearts[currentHearts - 1];
            rightmostHeart.SetActive(false);
            currentHearts--;
            
            //RemainigHearts.RemoveAt(RemainigHearts.Count - 1);
            StartCoroutine(DamageTakingAnimation());

            // if (RemainigHearts.Count == 1)
            // {
            //     StartCoroutine(BlinkHeartAnimation(RemainigHearts[0].GetComponent<Image>()));
            // }

            if (currentHearts == 1)
            {
                StartCoroutine(BlinkHeartAnimation(RemainigHearts[0].GetComponent<Image>()));
            }

            return GirlDamageTaken.Alive;
        }
    }

    protected void ResetPosition()
    {
        // Find the lowermost platform on the screen
        var allPlatforms = FindObjectsOfType<Platform>();
        var bottomOfScreen = GlobalManager.Instance.GetBottomOfScreen();
        Platform bottomMostPlatform = null;
        foreach (var item in allPlatforms)
        {
            float currY = item.transform.position.y;
            if (currY > bottomOfScreen)
            {
                if (bottomMostPlatform == null || bottomMostPlatform.transform.position.y > currY)
                {
                    bottomMostPlatform = item;
                }
            }
        }
        if (bottomMostPlatform == null)
        {
            Debug.Log("Could not find a platform on the screen. You died");
            reloadManager.Died();
        }
        else
        {
            // The multiplier should be the height of the girl
            transform.position = bottomMostPlatform.transform.position + Vector3.up * 1;
        }
    }

    protected void ControlMovement()
    {
        bool touchingGround = standingOnPlatforms.Count > 0;
        if (touchingGround)
        {
            remainingAirJumps = totalJumpAirJumpCount;
        }

        float horizontal;
        bool isJumpDown;
        if (IsContollerTarget)
        {
            horizontal = Input.GetAxis("KeyHorizontal");
            isJumpDown = Input.GetButton("KeyJump");
        }
        else
        {
            horizontal = Input.GetAxis("JoyHorizontal");
            isJumpDown = Input.GetButton("JoyJump");
        }
        
        // F=m*a
        // a = ds/dt
        var targetSpeed = horizontal * Speed;
        var speedDiff = targetSpeed - rigidbody.velocity.x;
        rigidbody.AddForce(new Vector3(speedDiff, 0, 0) / Time.fixedDeltaTime);

        if (touchingGround || remainingAirJumps > 0)
        {
            var elapsedSinceLastJump = Time.time - lastJumpTime;
            if (isJumpDown && !wasJumpDown && elapsedSinceLastJump > fastestReJumpTime)
            {
                if (touchingGround) audioSource.PlayOneShot(JumpSound, 0.7f);
                else audioSource.PlayOneShot(AirJumpSound, 1);
                
                var vel = rigidbody.velocity;
                vel.y = JumpStrength;
                rigidbody.velocity = vel;
                lastJumpTime = Time.time;
                if (!touchingGround)
                {
                    remainingAirJumps -= 1;
                }
            }
        }

        animator.enabled = horizontal != 0;
        if (horizontal > 0)
        {
            var euler = transform.rotation.eulerAngles;
            euler.y = 0;
            transform.rotation = Quaternion.Euler(euler);
        }
        if (horizontal < 0)
        {
            var euler = transform.rotation.eulerAngles;
            euler.y = 180;
            transform.rotation = Quaternion.Euler(euler);
        }
        
        wasJumpDown = isJumpDown;
    }

    IEnumerator DamageTakingAnimation()
    {
        recoveringFromDamage = true;
        try
        {
            var renderer = GetComponent<SpriteRenderer>();
            for (int i = 0; i < 7; i++)
            {
                renderer.enabled = !renderer.enabled;
                yield return new WaitForSeconds(0.1f);
            }
            renderer.enabled = true;
        }
        finally
        {
            recoveringFromDamage = false;
        }
    }

    IEnumerator BlinkHeartAnimation(Image target)
    {
        while (target)
        {
            target.enabled = !target.enabled;
            yield return new WaitForSeconds(0.15f);
        }
    }
    
    //Functions Added by Connor Simmons:

    //When the character collides with an object, checks if the object is any of the power ups, if so, calls
    //the corresponding functions and then destroys the powerup game object
    public void OnTriggerEnter2D(Collider2D other)
    {

        switch (other.tag)
        {
            case "PlatformInvuln":
                PlatformInvulnerable();
                Destroy(other.gameObject);
                break;
            case "HealItem":
                HealItem();
                Destroy(other.gameObject);
                break;
            case "StunRobotItem":
                canStunRobot = true;
                StartCoroutine(StunShooter());
                StartCoroutine(StunShooterDuration());
                Destroy(other.gameObject);
                break;
            default:
                //do nothing
                break;
        }
    }

    //Calls the functions that control the platform invulnerability logic
    private void PlatformInvulnerable()
    {
        GlobalManager.Instance.platformInvulnerable = true;
        StartCoroutine(PlatformInvulnDuration());
    }

    //Updates the currentHP value of the character and updates the game's UI to match
    private void HealItem()
    {
        if (currentHearts == maxHearts)
        {
            //do nothing, no heal received
        }
        else
        {
            RemainigHearts[currentHearts].SetActive(true);
            currentHearts++;
        }
    }

    //Pushes the character down when a platform is placed on top of girl character (meant to punish you for
    //trying to exploit the physics system as was previously possible)
    public void PushCharacterDown()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y - distPushedDown, transform.position.z);
    }

    //while the stun powerup is active, shoots a laser at the robot on a set interval 
    IEnumerator StunShooter()
    {
        while (canStunRobot)
        {
            var stunLaser = Instantiate(stunLaserPrefab, transform.position, Quaternion.identity);
            stunLaser.GetComponent<StunLaser>().Velocity = (robot.transform.position - transform.position) * stunLaserSpeed;
            yield return new WaitForSeconds(stunLaserShootSpeed);
        }
    }

    //Waits for length of stun powerup duration, then sets bool to false, ending the powerup
    IEnumerator StunShooterDuration()
    {
        yield return new WaitForSeconds(stunShooterDuration);
        canStunRobot = false;
    }


    //Waits for length of platform invulnerability powerup then sets the bool in GlobalManager to false
    IEnumerator PlatformInvulnDuration()
    {
        yield return new WaitForSeconds(platformInvulnDuration);
        GlobalManager.Instance.platformInvulnerable = false;
    }

}
