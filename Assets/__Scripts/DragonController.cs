using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonController : MonoBehaviour
{
    public float speed; //speed
    public float jump; //jump
    public GameObject healthManager;
    private Rigidbody2D _dragon; //dragon player sprite
    private PolygonCollider2D basicAttack;
    private PolygonCollider2D strike;
    private bool _facingRight = true; //facing direction
    private bool _isOnHill = false;
    private bool _inPortal = false;
    public float _healthPoints;

    Animator dragonAnim; //animator for the dragon

    public void Start()
    {
        //get all the components
        _dragon = GetComponent<Rigidbody2D>();
        dragonAnim = GetComponent<Animator>();
        _healthPoints = 100f;
    }

    public void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        
    }

    //flip the direction of the dragon sprite
    private void Flip()
    {
        _facingRight = !_facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void Movement()
    {

        //flip the character based on which direction ur moving
        var move = Input.GetAxis("Horizontal");
        if (Input.GetAxis("Horizontal") > 0 && _facingRight == false)
        {
            Flip();
        }
        else if (Input.GetAxis("Horizontal") < 0 && _facingRight == true)
        {
            Flip();
        }

        //translate the position of the player
        transform.position += new Vector3(move, 0, 0) * Time.deltaTime * speed;

        //change animation when player is walking
        if (Input.GetAxis("Horizontal") != 0)
        {
            dragonAnim.SetBool("IsWalking", true);
        }
        else
        {
            dragonAnim.SetBool("IsWalking", false);
        }

        //Crouch animation
        if (Input.GetAxis("Vertical") < 0 && _inPortal == false && dragonAnim.GetBool("IsWalking") == false)
        {
            dragonAnim.SetBool("IsCrouch", true);
        }
        else
        {
            dragonAnim.SetBool("IsCrouch", false);
        }

        //get input for player to jump, add impulse force for jump
        if (Input.GetButtonDown("Jump") && Mathf.Abs(_dragon.velocity.y) < 0.001f)
        {
            if (_isOnHill)
            {
                Vector3 targetVelocity = new Vector2(move * 10f, _dragon.velocity.y);
                _dragon.AddForce(new Vector2(0, jump * _dragon.gravityScale / 2), ForceMode2D.Impulse);
            }
            else
            {
                _dragon.AddForce(new Vector2(0, jump), ForceMode2D.Impulse);
            }

        }
    }

    private void Attack()
    {

        //Attack 1 Animations
        if (Input.GetButtonDown("Attack1") && !dragonAnim.GetCurrentAnimatorStateInfo(0).IsName("Dragon_BasicAttack"))
        {
            dragonAnim.SetTrigger("BasicAttack");
            basicAttack.enabled = true;
            StartCoroutine(DisableBasicAttackCollider());
        }

        //Attack 2 Animations
        if (Input.GetButtonDown("Attack2") && !dragonAnim.GetCurrentAnimatorStateInfo(0).IsName("Dragon_FlyKick"))
        {
            dragonAnim.SetTrigger("Strike");
            strike.enabled = true;
            StartCoroutine(DisableStrikeCollider());
        }
    }

    private IEnumerator DisableStrikeCollider()
    {
        yield return new WaitForSeconds(0.03f);
        strike.enabled = false;
        StopCoroutine(DisableStrikeCollider());
    }
    private IEnumerator DisableBasicAttackCollider()
    {
        yield return new WaitForSeconds(0.04f);
        basicAttack.enabled = false;
        StopCoroutine(DisableBasicAttackCollider());
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Hill"))
        {
            _isOnHill = true;
            _dragon.gravityScale = 3;
        }
        else
        {
            _isOnHill = false;
            _dragon.gravityScale = 1.5f;
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("portal1")) //CHANGE TO PORTAL2 once created
        {
            _inPortal = false;
        }
    }

    public void takeDamage(float damage)
    {
        _healthPoints -= damage;
        healthManager.GetComponent<healthManager>().healthUpdate(_healthPoints);
    }


}