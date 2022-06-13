using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
  // Private Variables
  private Rigidbody m_Rigidbody;
  private Vector3 playerRespawnPos;
  private AudioSource _audioSrc;
  private GameController _gameCtrl;
  private int _percentChunk = 0;
  private float _currXZScale;
  private float _currMass;
  private float _currDrag;
  private float _currStamina;

  private float buttonCooler = 0.2f;
  private int buttonCount = 0;
  private bool leftMode = false;
  private bool rightMode = false;
  private bool doubleTap = false;
  private bool hasJump = true;

  // player can only pick up one item box
  private bool canDash = true;
  private float breakTimeLimit = 0f;
  private float dashTimer = 0f;

  [SerializeField] private bool isController = false;
  [SerializeField] private int PlayerNum;
  [SerializeField] private KeyCode UpKey;
  [SerializeField] private KeyCode DownKey;
  [SerializeField] private KeyCode LeftKey;
  [SerializeField] private KeyCode RightKey;
  [SerializeField] private KeyCode ActionKey;

  [SerializeField] private AudioClip JumpAudio;
  [SerializeField] private AudioClip DashAudio;
  [SerializeField] private AudioClip CollisionAudio;
  [SerializeField] private AudioClip BoxAudio;

  // Public Variables
  public float m_Thrust = 50f;
  public float m_InitMass = 10f;
  public float m_InitDrag = 0f;
  public float maxSpeed = 4f;
  public float InitXZScale = 0.5f;
  public float YScale = 1f;
  public int IncPercentChunk = 8;
  public int DecPercentChunk = 8;
  public float maxStamina = 1f;
  public float recoverRate = 0.005f;

  // text to show item obtained
  // public GameObject StaminaBar;

  // Start is called before the first frame update
  void Start()
  {
    m_Rigidbody = GetComponent<Rigidbody>();
    _audioSrc = GetComponent<AudioSource>();
    _gameCtrl = GameObject.Find("GameController").GetComponent<GameController>();
    _currXZScale = InitXZScale;
    _currMass = m_InitMass;
    _currStamina = maxStamina;
    playerRespawnPos = transform.position;
    UpdateChunk();
  }

  void Update()
  {
    if ((!isController && Input.anyKeyDown) || (isController && Input.GetAxis("CHorizontal") > 1))
    {

      if (buttonCooler > 0 && buttonCount == 1)
      {

        doubleTap = true;
      }
      else
      {
        if ((!isController &&Input.GetKeyDown(RightKey)) || (isController && Input.GetAxis("CHorizontal") > 0.99f))
        {
          rightMode = true;
          leftMode = false;
        }
        else if ((!isController &&Input.GetKeyDown(LeftKey)) || (isController && Input.GetAxis("CHorizontal") < -0.99f))
        {
          leftMode = true;
          rightMode = false;
        }
        else
        {
          return;
        }
        buttonCooler = 0.2f;
        buttonCount += 1;

      }

    }

  }
  void FixedUpdate()
  {
    float DistanceToTheGround = GetComponent<Collider>().bounds.extents.y;
    bool IsGrounded = Physics.Raycast(transform.position, Vector3.down, DistanceToTheGround + 0.01f);
    bool usedStamina = false;
    // bool IsGrounded = Physics.BoxCast(transform.position, transform.lossyScale / 2, Vector3.down, out RaycastHit hit, transform.rotation, 0.05f);

    if (!hasJump && IsGrounded)
    {
      hasJump = true;
    }

    if ((!isController && Input.GetKey(RightKey)) || (isController && Input.GetAxis("CHorizontal") > 0))
    {
      m_Rigidbody.AddForce(transform.right * m_Thrust * _currXZScale);

      if (m_Rigidbody.velocity.x > maxSpeed)
      {
        m_Rigidbody.velocity = new Vector3(maxSpeed, m_Rigidbody.velocity.y, m_Rigidbody.velocity.z);
      }

      if (doubleTap && rightMode && canDash)
      {
        breakTimeLimit = 0.1f;
        maxSpeed *= 2;
        canDash = false;
        dashTimer = 5f;
        m_Rigidbody.velocity = new Vector3(Mathf.Abs(m_Rigidbody.velocity.x), 0, 0);
        m_Rigidbody.AddForce(transform.right * m_Thrust * _currXZScale * 3, ForceMode.Impulse);
        rightMode = false;
        _audioSrc.clip = DashAudio;
        _audioSrc.Play();
        _currStamina = Mathf.Max(_currStamina - 0.5f, 0f);
        usedStamina = true;
      }
    }

    if ((!isController && Input.GetKey(LeftKey)) || (isController && Input.GetAxis("CHorizontal") < 0))
    {
      m_Rigidbody.AddForce(transform.right * -1 * m_Thrust * _currXZScale);

      if (m_Rigidbody.velocity.x < -1 * maxSpeed)
      {
        m_Rigidbody.velocity = new Vector3(-1 * maxSpeed, m_Rigidbody.velocity.y, m_Rigidbody.velocity.z);
      }

      if (doubleTap && leftMode && canDash)
      {
        breakTimeLimit = 0.1f;
        maxSpeed *= 2;
        canDash = false;
        dashTimer = 5f;
        m_Rigidbody.velocity = new Vector3(Mathf.Abs(m_Rigidbody.velocity.x) * -1, 0, 0);
        m_Rigidbody.AddForce(transform.right * m_Thrust * _currXZScale * -3, ForceMode.Impulse);
        leftMode = false;
        _audioSrc.clip = DashAudio;
        _audioSrc.Play();
        _currStamina = Mathf.Max(_currStamina - 0.5f, 0f);
        usedStamina = true;
      }
    }

    if (hasJump && ((!isController && (Input.GetKey(UpKey)) || (isController && Input.GetAxis("CVertical") > 0))))
    {
      hasJump = false;
      m_Rigidbody.AddForce(transform.up * m_Thrust * 25f * _currXZScale);
      _audioSrc.clip = JumpAudio;
      _audioSrc.Play();
    }

    if ((!isController && Input.GetKey(DownKey)) || (isController && Input.GetAxis("CVertical") < 0))
    {
      m_Rigidbody.AddForce(transform.up * m_Thrust * -0.05f, ForceMode.Impulse);
    }

    if (Input.GetKey(ActionKey))
    {
      if (_currStamina > 0f)
      {
        // inflates
        _percentChunk = Mathf.Min(40, _percentChunk + IncPercentChunk);
        _currStamina = Mathf.Max(_currStamina - 0.02f, 0f);
      }
      else {
        // deflates
        _percentChunk = Mathf.Max(0, _percentChunk - DecPercentChunk);
      }
      usedStamina = true;
    }
    else if (_currXZScale != InitXZScale)
    {
      // deflates
      _percentChunk = Mathf.Max(0, _percentChunk - DecPercentChunk);
    }

    if (buttonCooler > 0)
    {
      buttonCooler -= 1 * Time.deltaTime;
    }
    else
    {
      buttonCount = 0;
      doubleTap = false;
      leftMode = false;
      rightMode = false;
    }

    if (breakTimeLimit > 0f)
    {
      breakTimeLimit -= 1 * Time.deltaTime;
      if (breakTimeLimit <= 0f)
      {
        maxSpeed /= 2;
      }
    }

    if (dashTimer > 0f)
    {

      dashTimer -= 1 * Time.deltaTime;
      if (dashTimer <= 0f)
      {
        canDash = true;
      }
    }
    UpdateChunk();
    UpdateStamina(usedStamina);
  }

  void UpdateChunk()
  {
    _currXZScale = InitXZScale + _percentChunk / 100f * 0.5f;
    _currMass = m_InitMass + _percentChunk / 100f * 5;
    _currDrag = m_InitDrag + _percentChunk / 100f * 5;
    transform.localScale = new Vector3(_currXZScale, YScale, _currXZScale);
    m_Rigidbody.mass = _currMass;
    m_Rigidbody.drag = _currDrag;
  }

  void UpdateStamina(bool usedStamina)
  {
    if (_currStamina < maxStamina && !usedStamina) {
      _currStamina = Mathf.Min(_currStamina + recoverRate, maxStamina);
    }
    // StaminaBar.transform.localScale = new Vector3(_currStamina, 0.1f, 0.1f);
  }

  void OnTriggerEnter(Collider other)
  {
    if(other.gameObject.name == "Scream") {
      Debug.Log("YAY");
        m_Rigidbody.AddForce((new Vector3(1, transform.position.y-other.transform.position.y, 0))*100f, ForceMode.Impulse);
    }
    if (other.gameObject.CompareTag("Item"))
    {
      _currStamina = maxStamina;
      other.gameObject.SetActive(false);
      StartCoroutine(RespawnObj(other.gameObject, 5f));
      
      maxStamina = Mathf.Min(maxStamina + 0.5f, 2f);
      _audioSrc.clip = BoxAudio;
      _audioSrc.Play();
    } else if (other.gameObject.CompareTag("Platform")) {
      transform.parent = other.transform;
    }else if (other.gameObject.CompareTag("Trap"))
    {
      _gameCtrl.PlayerGG(PlayerNum);
    }
  }

  void OnTriggerExit(Collider other) {
    // in-progress currently not working :(
    Debug.Log("ONTRIGGEREXIT");
    if (other.gameObject.CompareTag("Platform")) {
      transform.parent = null;
    }
    
  }

  void OnCollisionEnter(Collision collision)
  {
    if (collision.gameObject.tag == "Player")
    {
      _audioSrc.clip = CollisionAudio;
      _audioSrc.Play();
    }
    else if (collision.gameObject.tag == "Trap")
    {
      // _gameCtrl.RespawnPlayer(PlayerNum);
    }
  }

  public void ResetPlayer() {
    m_Rigidbody.velocity = new Vector3(0, 0, 0);
    transform.position = playerRespawnPos;
  }
  
  IEnumerator RespawnObj(GameObject obj, float spawnDelay)
    {
      yield return new WaitForSeconds(spawnDelay);
      obj.SetActive(true);
    }
}
