using UnityEngine;

public class NewEmptyCSharpScript : MonoBehaviour
{

    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator animator;

    //Modificacion 
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

        // Configura la gravedad por defecto
        defaultGravityScale = rb.gravityScale;
    }

    void Update()
    {
        // Aplica gravedad manual
        if (!puedeMoverseVerticalMente)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y + (Physics2D.gravity.y * defaultGravityScale * Time.deltaTime));
        }

        SetupMoverseHorizontal();
        SetupMoverseVertical();
        SetupSalto();
        SetupAtacar();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Suelo"))
        {
            puedeSaltar = true;
        }

        if (collision.gameObject.CompareTag("Enemigo"))
        {
            DemonController demon = collision.gameObject.GetComponent<DemonController>();
            Debug.Log($"Colisión con Enemigo: {demon.puntosVida}");
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
        }
    }

    void SetupMoverseVertical()
    {
        if (!puedeMoverseVerticalMente) return;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // Reinicia la velocidad vertical

        if (Input.GetKey(KeyCode.UpArrow))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 10);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -10);
        }
    }

    void SetupMoverseHorizontal()
    {
        float velocidadX = 0;
        animator.SetInteger("Estado", 0);

        if (Input.GetKey(KeyCode.RightArrow))
        {
            velocidadX = 10;
            sr.flipX = false;
            animator.SetInteger("Estado", 1);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            velocidadX = -10;
            sr.flipX = true;
            animator.SetInteger("Estado", 1);
        }

        rb.linearVelocity = new Vector2(velocidadX, rb.linearVelocity.y);
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 10.4f);
            puedeSaltar = false;
        }
    }
}