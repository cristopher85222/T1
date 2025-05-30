using UnityEngine;

public class EnemigosPatrullaje : MonoBehaviour
{
    [Header("Puntos de Patrullaje")]
    public Transform puntoA;
    public Transform puntoB;

    [Header("Configuración de Movimiento")]
    public float velocidad = 2f;
    public float tiempoEspera = 1f; // Tiempo que espera en cada punto
    public float distanciaDeteccion = 0.1f; // Distancia mínima para considerar que llegó al punto

    [Header("Configuración Visual")]
    public bool mostrarGizmos = true;
    public Color colorLinea = Color.red;

    private Transform objetivo;
    private bool moviendose = true;
    private float tiempoEsperaActual = 0f;
    private Vector3 posicionInicial;

    void Start()
    {
        posicionInicial = transform.position;

        // Validar que los puntos estén asignados
        if (puntoA == null || puntoB == null)
        {
            Debug.LogError("Los puntos A y B deben estar asignados en " + gameObject.name);
            BuscarPuntosAutomaticamente();
        }

        if (puntoA != null && puntoB != null)
        {
            // Determinar el punto más cercano como objetivo inicial
            float distanciaA = Vector3.Distance(transform.position, puntoA.position);
            float distanciaB = Vector3.Distance(transform.position, puntoB.position);

            objetivo = (distanciaA <= distanciaB) ? puntoA : puntoB;
        }
    }

    void BuscarPuntosAutomaticamente()
    {
        // Buscar puntos en el objeto padre o en el mismo nivel
        Transform contenedorPuntos = transform.parent;
        if (contenedorPuntos != null)
        {
            // Buscar en hermanos del enemigo
            foreach (Transform hermano in contenedorPuntos)
            {
                if (hermano.name.Contains("Puntos") || hermano.name.Contains("puntos"))
                {
                    Transform[] puntosEncontrados = hermano.GetComponentsInChildren<Transform>();

                    foreach (Transform punto in puntosEncontrados)
                    {
                        if (punto.name.Contains("PuntoA") || punto.name.Contains("Punto_A"))
                            puntoA = punto;
                        else if (punto.name.Contains("PuntoB") || punto.name.Contains("Punto_B"))
                            puntoB = punto;
                    }
                    break;
                }
            }
        }

        if (puntoA == null || puntoB == null)
        {
            Debug.LogWarning("No se pudieron encontrar los puntos automáticamente. Asígnalos manualmente en " + gameObject.name);
        }
    }

    void Update()
    {
        if (puntoA == null || puntoB == null) return;

        if (moviendose)
        {
            MoverHaciaObjetivo();
        }
        else
        {
            Esperar();
        }
    }

    void MoverHaciaObjetivo()
    {
        // Calcular dirección hacia el objetivo
        Vector3 direccion = (objetivo.position - transform.position).normalized;

        // Mover el enemigo
        transform.position += direccion * velocidad * Time.deltaTime;

        // Rotar hacia la dirección de movimiento (opcional para 2D)
        if (direccion != Vector3.zero)
        {
            // Para juegos 2D, solo rotar en Z
            if (Mathf.Abs(direccion.z) < 0.1f) // Es movimiento 2D
            {
                float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angulo, Vector3.forward);
            }
            else // Movimiento 3D
            {
                transform.rotation = Quaternion.LookRotation(direccion);
            }
        }

        // Verificar si llegó al punto objetivo
        float distancia = Vector3.Distance(transform.position, objetivo.position);
        if (distancia <= distanciaDeteccion)
        {
            LlegoAlPunto();
        }
    }

    void LlegoAlPunto()
    {
        moviendose = false;
        tiempoEsperaActual = 0f;

        // Cambiar al siguiente punto
        objetivo = (objetivo == puntoA) ? puntoB : puntoA;

        Debug.Log(gameObject.name + " llegó al punto " + objetivo.name);
    }

    void Esperar()
    {
        tiempoEsperaActual += Time.deltaTime;

        if (tiempoEsperaActual >= tiempoEspera)
        {
            moviendose = true;
            Debug.Log(gameObject.name + " reanuda movimiento hacia " + objetivo.name);
        }
    }

    // Métodos públicos para control externo
    public void DetenerPatrullaje()
    {
        moviendose = false;
        Debug.Log("Patrullaje detenido para " + gameObject.name);
    }

    public void ReanudarPatrullaje()
    {
        moviendose = true;
        Debug.Log("Patrullaje reanudado para " + gameObject.name);
    }

    public void CambiarVelocidad(float nuevaVelocidad)
    {
        velocidad = nuevaVelocidad;
    }

    public void CambiarPuntoA(Transform nuevoPunto)
    {
        puntoA = nuevoPunto;
    }

    public void CambiarPuntoB(Transform nuevoPunto)
    {
        puntoB = nuevoPunto;
    }

    public void ReiniciarPosicion()
    {
        transform.position = posicionInicial;
        if (puntoA != null && puntoB != null)
        {
            float distanciaA = Vector3.Distance(transform.position, puntoA.position);
            float distanciaB = Vector3.Distance(transform.position, puntoB.position);
            objetivo = (distanciaA <= distanciaB) ? puntoA : puntoB;
        }
        moviendose = true;
    }

    // Estados del enemigo
    public bool EstaMoviendose()
    {
        return moviendose;
    }

    public Transform ObjetivoActual()
    {
        return objetivo;
    }

    // Visualización en el editor
    void OnDrawGizmos()
    {
        if (!mostrarGizmos) return;

        // Dibujar línea entre puntos A y B
        if (puntoA != null && puntoB != null)
        {
            Gizmos.color = colorLinea;
            Gizmos.DrawLine(puntoA.position, puntoB.position);

            // Dibujar esferas en los puntos
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(puntoA.position, 0.3f);
            Gizmos.DrawWireSphere(puntoB.position, 0.3f);

            // Etiquetas de los puntos
            Gizmos.color = Color.white;
            Gizmos.DrawRay(puntoA.position, Vector3.up * 0.5f);
            Gizmos.DrawRay(puntoB.position, Vector3.up * 0.5f);

            // Mostrar objetivo actual
            if (Application.isPlaying && objetivo != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(objetivo.position, 0.5f);

                // Línea hacia el objetivo
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, objetivo.position);
            }
        }

        // Mostrar rango de detección
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccion);

        // Mostrar dirección de movimiento
        if (Application.isPlaying && moviendose && objetivo != null)
        {
            Vector3 direccion = (objetivo.position - transform.position).normalized;
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(transform.position, direccion);
        }
    }
}