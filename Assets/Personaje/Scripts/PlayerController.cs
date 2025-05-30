using UnityEngine;
using UnityEngine.UI; // Esto es necesario para trabajar con UI

public class NewEmptyCSharpScript : MonoBehaviour
{

    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator animator;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float jumpForce = 10.4f;
    [SerializeField] private float verticalSpeed = 10f;

    [Header("Combat")]
    [SerializeField] private bool estaAtacando = false;
    [SerializeField] private float attackCooldown = 0.5f;
    private float lastAttackTime;

    [Header("Movement Mechanics")]
    private bool puedeMoverseVerticalMente = false;
    private float defaultGravityScale = 1f;
    private bool puedeSaltar = true;



    // Variables de audio y movimiento
    private bool wasMoving = false;
    private bool isMoving = false;


    //vida del jugador
    public int vidaMaxima = 3;
    private int vidaActual;


    //UI vida controller del jugador
    public VidaUIController vidaUI;

    //Cantidad de enemigos destruidos   
    private int enemigosDestruidos = 0;
    public Text enemigosText; // Asignarás esto desde el inspector




    void Start()
    {
        Debug.Log("Iniciando PlayerController");
        // Obtener componentes
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // Configura la gravedad por defecto
        defaultGravityScale = rb.gravityScale;


        // Inicializa la vida actual al máximo
        vidaActual = vidaMaxima; 
        vidaUI.ActualizarVida(vidaActual, vidaMaxima);

        
        if (enemigosText == null)
        {
            Debug.LogError("ERROR: enemigosText no está asignado en el Inspector.");
        }
        enemigosDestruidos = 99;  // prueba forzada
        enemigosText.text = "Enemigos destruidos: " + enemigosDestruidos;
    }

  

    void Update()
    {
        HandleGravity();
        HandleMovement();
        HandleAudio();


    }

    void HandleGravity()
    {
        // Aplica gravedad manual cuando no puede moverse verticalmente
        if (!puedeMoverseVerticalMente)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x,
                rb.linearVelocity.y + (Physics2D.gravity.y * defaultGravityScale * Time.deltaTime));
        }
    }

    void HandleMovement()
    {
        // Detectar si se está moviendo (para audio)
        wasMoving = isMoving;
        isMoving = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow);

        SetupMoverseHorizontal();
        SetupMoverseVertical();
        SetupSalto();
        SetupAtacar();
    }

    //Sonido de pasos
    void HandleAudio()
    {
        // Sonidos de pasos - cuando empieza a caminar
        if (isMoving && puedeSaltar && !wasMoving && !estaAtacando)
        {
            PlayWalkSound();
        }
    }

    #region Collision Events
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Suelo"))
        {
            puedeSaltar = true;
        }

        // Colisión con el enemigo 
        if (collision.gameObject.CompareTag("Enemigo"))
        {
            HandleEnemyCollision(collision.gameObject);
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
    #endregion


    void SetupMoverseHorizontal()
    {
        float velocidadX = 0;
        animator.SetInteger("Estado", 0); // Idle por defecto

        if (Input.GetKey(KeyCode.RightArrow))
        {
            velocidadX = moveSpeed;
            sr.flipX = false;
            animator.SetInteger("Estado", 1); // Caminando
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            velocidadX = -moveSpeed;
            sr.flipX = true;
            animator.SetInteger("Estado", 1); // Caminando
        }

        // Solo aplicar velocidad horizontal si no está atacando
        if (!estaAtacando)
        {
            rb.linearVelocity = new Vector2(velocidadX, rb.linearVelocity.y);
        }
        else
        {
            // Detener movimiento durante ataque
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }



    void SetupMoverseVertical()
    {
        if (!puedeMoverseVerticalMente) return;

        // Reinicia la velocidad vertical
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);

        if (Input.GetKey(KeyCode.UpArrow))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, verticalSpeed);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -verticalSpeed);
        }
    }

    
    



    //REFACTORIZACION 
    void SetupAtacar()
    {
        // Iniciar ataque
        if (Input.GetKeyDown(KeyCode.A) && !estaAtacando &&
            Time.time >= lastAttackTime + attackCooldown)
        {
            StartAttack();
        }

        // Terminar ataque
        if (Input.GetKeyUp(KeyCode.A) && estaAtacando)
        {
            EndAttack();
        }
    }

    //Reproducir sonido atake
    void StartAttack()
    {
        estaAtacando = true;
        lastAttackTime = Time.time;
        animator.SetInteger("Estado", 2); // Atacando

        // Reproducir sonido de ataque
        PlayAttackSound();
    }

    void EndAttack()
    {
        estaAtacando = false;
        animator.SetInteger("Estado", 0); // Volver a Idle
    }


    //REFACTORIZADO
    void SetupSalto()
    {
        if (!puedeSaltar) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            puedeSaltar = false;

            // Reproducir sonido de salto
            PlayJumpSound();
        }
    }

    #region Audio Methods
    void PlayAttackSound()
    {
        // Reproducir sonido de ataque
        if (AudioManager.instance != null)
            AudioManager.instance.PlayAttackSound();
    }

    // Reproducir sonido de salto
    void PlayJumpSound()
    {
        if (AudioManager.instance != null)
            AudioManager.instance.PlayJumpSound();
    }

    // Reproducir sonido de caminar
    void PlayWalkSound()
    {
        if (AudioManager.instance != null)
            AudioManager.instance.PlayWalkSound();
    }

    // Reproducir sonido de daño
    void PlayDamageSound()
    {
        if (AudioManager.instance != null)
            AudioManager.instance.PlayDamageSound();
    }

    // Reproducir sonido de recolección de items
    void PlayCollectSound()
    {
        if (AudioManager.instance != null)
            AudioManager.instance.PlayCollectSound();
    }
    #endregion

    #region Combat Methods
    void HandleEnemyCollision(GameObject enemy)
    {
        DemonController demon = enemy.GetComponent<DemonController>();
        if (demon != null)
        {
            Debug.Log($"Colisión con Enemigo: {demon.puntosVida}");

            // Reproducir sonido de daño
            PlayDamageSound();

            
            // Aplicar daño al jugador
            TakeDamage(1);

            // Destruir el enemigo
            Destroy(enemy);
            enemigosDestruidos++;
            Debug.Log("Enemigos destruidos: " + enemigosDestruidos);

            // ACTUALIZAR UI
            enemigosText.text = "Enemigos destruidos: " + enemigosDestruidos;
            Debug.Log("Texto actualizado: " + enemigosText.text);

        }
    }

   


    // Método público para recibir daño desde otros scripts
    public void TakeDamage(int damage)
    {
        

        // Aquí puedes agregar lógica de vida, etc.
        vidaActual-= damage;
        Debug.Log($"Player recibio: {vidaActual} de daño. vida actual");

        vidaUI.ActualizarVida(vidaActual, vidaMaxima);

        if (vidaActual<=0)
        {
            Debug.Log("El jugador ha muerto.");
        }
    }

    // Método público para recolectar items
    public void CollectItem()
    {
        Debug.Log("Item recolectado!");
        PlayCollectSound();
    }
    #endregion

    #region Debug => informacion en pantalla 
    // Para debugging - mostrar información en pantalla
    void OnDrawGizmosSelected()
    {
        // Mostrar información de estado
        if (Application.isPlaying)
        {
            Gizmos.color = puedeSaltar ? Color.green : Color.red;
            Gizmos.DrawWireSphere(transform.position + Vector3.down * 0.5f, 0.2f);
        }
    }
    #endregion

   



}