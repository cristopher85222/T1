using UnityEngine;

public class NewEmptyCSharpScript : MonoBehaviour
{

    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator animator;
    private bool puedeMoverseVerticalMente = false;
    private float defaultGravityScale = 1f;
    private bool puedeSaltar = true;
   
    private bool estaAtacando = false;


    void Start()
    {
        Debug.Log("Iniciando PlayerController");
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        defaultGravityScale = rb.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {

       
            SetupMoverseHorizontal();
            SetupMoverseVertical();
            SetupSalto(); 
            SetupAtacar();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemigo"))
        {
            DemonController demon = collision.gameObject.GetComponent < DemonController>();
            Debug.Log($"Colision con Enemigo: ${demon.puntosVida}");
            Destroy(collision.gameObject);
        }
    }
    void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log($"Trigger con: {other.gameObject.name}");
        if (other.gameObject.name == "Muro")
        {
            puedeMoverseVerticalMente = true;
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log($"Trigger con: {other.gameObject.name}");
        if (other.gameObject.name == "Muro")
        {
            puedeMoverseVerticalMente = false;
            rb.gravityScale = defaultGravityScale;
        }
    }
    void SetupMoverseVertical()
    {

        if (!puedeMoverseVerticalMente) return;
        rb.gravityScale = 0;
        rb.linearVelocityY = 0;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            rb.linearVelocityY = 10;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            rb.linearVelocityY = -10;
        }
    }

    void SetupMoverseHorizontal()
    {
        
        
            rb.linearVelocityX = 0;
            animator.SetInteger("Estado", 0);
        
            if (Input.GetKey(KeyCode.RightArrow))
            {
                rb.linearVelocityX = 10;
                sr.flipX = false;
                animator.SetInteger("Estado", 1);
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                rb.linearVelocityX = -10;
                sr.flipX = true;
                animator.SetInteger("Estado", 1);
            }
        
       
    }

    void SetupAtacar()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            estaAtacando = true;
            animator.SetInteger("Estado", 2); // Atacando
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            estaAtacando = false;
            animator.SetInteger("Estado", 0); // Volver a Idle
        }
    }
    void SetupSalto()
    {
        if (!puedeSaltar) return;
        if (Input.GetKeyUp(KeyCode.Space))
        {
            rb.linearVelocityY = 10.4f;
        }
    }
}