using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using TMPro;

enum Direction {Left, Right, None};
public class PlayerController1 : MonoBehaviour
{
    //Private Player State Variables
    private Rigidbody rb;
    private Vector3 movement;
    private Direction dirFacing;

    private Vector3 initialPos;
    private float distToGround;
    private float _currStamina;
    private float inputCooldown;
    private bool usedStamina;
    private bool knocked;
    
    //Public Player State Variables
    public AbilityDelegate ability;
    public int hitBy;
    public bool isDashing;

    //Player State Constants
    public int PlayerNum; 
    public float maxStamina = 1f;
    public float recoverRate = 0.001f;
    
    // Input Control Variables
    [SerializeField] private string VerticalAxis;
    [SerializeField] private string HorizontalAxis;
    [SerializeField] private KeyCode ActionKey;
    [SerializeField] private TextMeshProUGUI itemText;

    //Physics Movement Variables
    public float fallMultiplier =2.5f;
    public float lowJumpMultiplier = 2f;
    [Range(1, 30)]
    public float jumpVelocity;
    public float bounceBoost = 1f;

    //Sound Variables
    [SerializeField] 
    private AudioClip BoxAudio;
    private AudioSource _audioSrc;

    //Other Variables
    private GameController _gameCtrl;
    public delegate IEnumerator AbilityDelegate();

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        _audioSrc = GetComponent<AudioSource>();
        _gameCtrl = GameObject.Find("GameController").GetComponent<GameController>();
        _currStamina = maxStamina;
        inputCooldown = 0;
        distToGround = GetComponent<Collider>().bounds.extents.y;
        hitBy = -1;
        // ✯
        itemText.text = "";

        // if(PlayerNum == 0) {
        //     ability = Dash;
        // }else if(PlayerNum == 2){
        //     ability = Scream;
        // }else {
        //     ability = Gun;
        // }
        knocked = false;
        initialPos = transform.position;
        usedStamina = false;
        isDashing = false;
    }

    // Update is called once per frame
    void Update()
    {
        usedStamina = false;

        if (inputCooldown > 0) {
            inputCooldown -= Time.deltaTime;
        } else {
            // Jump + DownJump
            if (!knocked && !isDashing) {
                // jump from the ground
                if (Input.GetAxisRaw(VerticalAxis) > 0 && IsGrounded()) {
                    rb.velocity = Vector3.up * jumpVelocity;
                // falling
                } else if (Input.GetAxisRaw(VerticalAxis) < 0) { 
                    rb.velocity = Vector3.up * -2 * jumpVelocity;
                }
            }

            // Movement in the Air is unrestricted and clamped
            float clamped_movement = ClampMovement(Input.GetAxisRaw(HorizontalAxis)*8f);
            movement = new Vector3(clamped_movement, rb.velocity.y, 0);
        
            RotatePlayer(Input.GetAxisRaw(HorizontalAxis));

            if (rb.velocity.y < 0) {
                movement += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            } else if (rb.velocity.y > 0 && !Input.GetButton("Jump")) {
                movement += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
            }
        
        }

        // Use Ability
        if (Input.GetKeyDown(ActionKey) && _currStamina >= 0.5f 
                                        && ability != null) {
            _currStamina -= 0.5f;
            StartCoroutine(ability());
            usedStamina = true; 
        }

        UpdateStamina(usedStamina);

    }

    void FixedUpdate() {
        rb.velocity = movement;
        
    }

    void OnCollisionEnter(Collision other) {
        if(other.gameObject.tag == "Player") {

            inputCooldown = 0.2f;

            float this_v = Mathf.Pow(movement.x, 2) + Mathf.Pow(movement.y, 2);
            float other_v = Mathf.Pow(other.gameObject.GetComponent<PlayerController1>().movement.x, 2) + 
                            Mathf.Pow(other.gameObject.GetComponent<PlayerController1>().movement.y, 2);
            float sum_v = this_v + other_v;
            
            float this_new_vx = 0, other_new_vx = 0;
            if (sum_v > 0) {
                this_new_vx = -movement.x * other_v / sum_v * 2;
                other_new_vx = -other.gameObject.GetComponent<PlayerController1>().movement.x * this_v / sum_v * 2;
            }
            StartCoroutine(Knocked(0.5f)); // freeze 0.5 second
            movement = new Vector3(this_new_vx, bounceBoost, 0);
            other.gameObject.GetComponent<PlayerController1>().movement = new Vector3(other_new_vx, bounceBoost, 0);
        }
        if(other.gameObject.tag == "Player" && other.gameObject.GetComponent<PlayerController1>().isDashing) {
            StartCoroutine(HitCooldown(other.gameObject.GetComponent<PlayerController1>().PlayerNum));

        }
    }
   
    void OnTriggerEnter(Collider other)
    {
        // Hit by a bullet
        if (other.gameObject.tag == "Bullet") {
            bullet = other.gameObject.GetComponent<Bullet>();
            if (bullet.playerNum != PlayerNum) { // hit by enemy's bullet 
                Vector3 knockBack;
                StartCoroutine(HitCooldown(bullet.playerNum));
                Destroy(other.gameObject);      // destroy bullet

                // Tune for a minor shift on x and y axis
                //   could be subsituted by any small number 
                Vector3 VelChange 
                    = new Vector3(1, (transform.position.y - other.transform.position.y + 0.5f), 0)
                                                                                * jumpVelocity * 0.2f;
                
                // Player is attached from the left, positive increment
                if (transform.position.x - other.transform.position.x > 0) {
                    knockBack = VelChange;
                } else {                        // negative increment
                    knockBack = -VelChange;
                }

                StartCoroutine(Knocked(0.1f));  // Attacked for 0.1 second
                rb.velocity += knockBack;       // Apply the velocity change
            }
        } 
    
        // Attack by a scream
        if (other.gameObject.tag == "Scream") {
            Vector3 VelChange;
            StartCoroutine(HitCooldown(other.gameObject.GetComponentInParent<PlayerController1>().PlayerNum));
            
            // Tune for a minor shift on x and y axis
            //   could be subsituted by any small number 
            Vector3 baseSpeed 
                = new Vector3(1, (transform.position.y - other.transform.position.y + 0.5f), 0) 
                                                                              * jumpVelocity * 0.5f;
            
            // player is attached from the left, positive increment
            if (transform.position.x -other.transform.position.x > 0) {
                knockBack = VelChange;
            } else { // negative increment
                knockBack = -VelChange;
            }

            StartCoroutine(Knocked(1f));
            rb.velocity += knockBack;
        }

        // ***Previous Controller Code***
        if (other.gameObject.CompareTag("Item"))
        {
            // _currStamina = maxStamina;
            // other.gameObject.SetActive(false);
            // StartCoroutine(RespawnObj(other.gameObject, 5f));
            
            // maxStamina = Mathf.Min(maxStamina + 0.5f, 2f);
            // numItems++;
            print(other.gameObject.name);
            if (other.gameObject.name == "Scream Item(Clone)") {
                ability = Scream;
                itemText.text = "Scream";
            } else if (other.gameObject.name == "Dash Item(Clone)") {
                ability = Dash;
                itemText.text = "Dash";
            } else if (other.gameObject.name == "Gun Item(Clone)") {
                ability = Gun;
                itemText.text = "Gun";
            } 

            other.gameObject.SetActive(false);
            _audioSrc.clip = BoxAudio;
            _audioSrc.Play();
            transform.parent = null;


        } else if (other.gameObject.CompareTag("Platform")) 
        {
            transform.parent = other.transform;
        }else if (other.gameObject.CompareTag("Trap"))
        {
            _gameCtrl.PlayerGG(PlayerNum);
            transform.parent = null;
        }
    }

   /* Allocate n seconds of attacking time */  
    IEnumerator Knocked(float seconds)
    {
        knocked = true;
        yield return new WaitForSeconds(seconds);
        knocked = false;
    }

    /* Add 5 seconds of safe buffer time when the player is killed */  
    IEnumerator HitCooldown(int player)
    {
        if (hitBy == player) { yield break;}
        hitBy = player;
        yield return new WaitForSeconds(5.0f);

        hitBy = -1; // remove the killer
    }

    /*
     * Ability Functions
     */

    IEnumerator Scream()
    {
        switch(dirFacing) { 
            case Direction.Right:
                transform.GetChild(0).gameObject.SetActive(true);
                break;
            case Direction.Left:
                transform.GetChild(1).gameObject.SetActive(true);
                break;
            default:
                transform.GetChild(1).gameObject.SetActive(true);
                transform.GetChild(0).gameObject.SetActive(true);
                break;
        }
        yield return new WaitForSeconds(0.5f);
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);

    }

    IEnumerator Dash()
    {
        switch(dirFacing) { 
            case Direction.Right:
                rb.velocity = Vector3.right * 2f * jumpVelocity;
                break;
            case Direction.Left:
                rb.velocity = Vector3.right * -2f * jumpVelocity;
                break;
            default:
                rb.velocity = Vector3.up * 4f * jumpVelocity;
                break;
        }
        movement = rb.velocity;
        rb.useGravity = false;
        rb.mass = 1000f;
        isDashing = true;
        yield return new WaitForSeconds(0.125f); // dashing 

        rb.mass = 10f;
        isDashing = false;
        rb.velocity = Vector3.zero;
        movement = rb.velocity;
        rb.useGravity = true;

    }

    IEnumerator Sword()
    {
        if(transform.GetChild(4).GetChild(1).gameObject.activeSelf) {
            transform.GetChild(4).GetChild(2).gameObject.SetActive(true);

            yield break;
        }else if(transform.GetChild(4).GetChild(0).gameObject.activeSelf) {
            transform.GetChild(4).GetChild(1).gameObject.SetActive(true);
            yield break;
        }else {
            transform.GetChild(4).GetChild(0).gameObject.SetActive(true);
        }
        
        yield return new WaitForSeconds(3f);
        transform.GetChild(4).GetChild(0).gameObject.SetActive(false);
        transform.GetChild(4).GetChild(1).gameObject.SetActive(false);
        transform.GetChild(4).GetChild(2).gameObject.SetActive(false);
    }

    IEnumerator Gun()
    {
        switch(dirFacing) { 
            case Direction.Right:
                transform.GetChild(5).eulerAngles = new Vector3(0,0,0);
                break;
            case Direction.Left:
                transform.GetChild(5).eulerAngles = new Vector3(0,0,180);
                break;
            default:
                transform.GetChild(5).eulerAngles = new Vector3(0,0,90);
                break;  
        }
        transform.GetChild(5).gameObject.SetActive(true);

        yield return new WaitForSeconds(3f);
        transform.GetChild(5).gameObject.SetActive(false);
    }

    /*
     * Support Functions
     */

    // ClampMovement - Clamps movement so that players can't move too fast
    float ClampMovement(float input) {
        float new_input = input;
        if (knocked || isDashing) return rb.velocity.x;
        if (rb.velocity.y != 0) {
            if(input > 0 && rb.velocity.x > input) {
                new_input = Mathf.Clamp(input + rb.velocity.x, -1000f, 
                                                                rb.velocity.x);
            } else if(input < 0 && rb.velocity.x < input) {
                new_input = Mathf.Clamp(input+rb.velocity.x, rb.velocity.x, 
                                                                        1000f);
            } else if (input == 0) {
                new_input = rb.velocity.x;
            }
        }

        return new_input;
    } 

    void RotatePlayer(float input) {
        if(Input.GetAxisRaw(HorizontalAxis) > 0) {
            transform.GetChild(2).eulerAngles = new Vector3(
                transform.GetChild(2).eulerAngles.x,
                -90f,
                transform.GetChild(2).eulerAngles.z
            );
            dirFacing = Direction.Right;
        } else if(Input.GetAxisRaw(HorizontalAxis) < 0) {
            transform.GetChild(2).eulerAngles = new Vector3(
                transform.GetChild(2).eulerAngles.x,
                90f,
                transform.GetChild(2).eulerAngles.z
            );
            dirFacing = Direction.Left;
        } else {
            transform.GetChild(2).eulerAngles = new Vector3(
                transform.GetChild(2).eulerAngles.x,
                0f,
                transform.GetChild(2).eulerAngles.z
            );
            dirFacing = Direction.None;
        }
    }

    // IsGrounded - Checks if Player can jump and is on the ground
    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, - Vector3.up, distToGround + 0.01f);
    }

    public void ResetPlayer(bool isMovingLevel) {
        rb.velocity = new Vector3(0, 0, 0);
        if(isMovingLevel) {
            Vector3 newRespawn = new Vector3(Camera.main.transform.position.x, 9, initialPos.z);
            transform.position = newRespawn;
        }else {
            transform.position = initialPos;
        }
        transform.GetChild(4).GetChild(0).gameObject.SetActive(false);
        transform.GetChild(4).GetChild(1).gameObject.SetActive(false);
        transform.GetChild(4).GetChild(2).gameObject.SetActive(false);
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        // numItems = 0;
        itemText.text = "";
        ability = null;
    }

    void UpdateStamina(bool usedStamina)
    {
        if (_currStamina < maxStamina && !usedStamina) {
            _currStamina = Mathf.Min(_currStamina + recoverRate, maxStamina);
        }
        transform.GetChild(3).localScale = new Vector3(_currStamina, 0.1f, 0.1f);
    }

    IEnumerator RespawnObj(GameObject obj, float spawnDelay)
    {
      yield return new WaitForSeconds(spawnDelay);
      obj.SetActive(true);
    }
}