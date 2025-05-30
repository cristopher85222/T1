using UnityEngine;

public class PuntosManager : MonoBehaviour
{
    [Header("Configuración de Puntos")]
    public Transform puntoA;
    public Transform puntoB;

    [Header("Creación Automática")]
    public bool crearPuntosAlInicio = true;
    public float distanciaEntrePuntos = 5f;
    public float alturaPuntos = 0f; // Para ajustar la altura de los puntos

    [Header("Configuración Visual")]
    public Color colorPuntoA = Color.green;
    public Color colorPuntoB = Color.red;
    public float tamanoPuntos = 0.5f;

    void Start()
    {
        if (crearPuntosAlInicio && (puntoA == null || puntoB == null))
        {
            CrearPuntosDePatrullaje();
        }

        AsignarPuntosAEnemigos();
    }

    [ContextMenu("Crear Puntos de Patrullaje")]
    public void CrearPuntosDePatrullaje()
    {
        // Crear punto A si no existe
        if (puntoA == null)
        {
            GameObject pA = new GameObject("PuntoA");
            pA.transform.position = transform.position + Vector3.left * (distanciaEntrePuntos / 2) + Vector3.up * alturaPuntos;
            pA.transform.parent = transform;
            puntoA = pA.transform;

            // Agregar componente visual
            var componenteA = pA.AddComponent<PuntoPatrullaje>();
            componenteA.nombrePunto = "A";
            componenteA.colorPunto = colorPuntoA;
            componenteA.tamanoPunto = tamanoPuntos;
        }

        // Crear punto B si no existe
        if (puntoB == null)
        {
            GameObject pB = new GameObject("PuntoB");
            pB.transform.position = transform.position + Vector3.right * (distanciaEntrePuntos / 2) + Vector3.up * alturaPuntos;
            pB.transform.parent = transform;
            puntoB = pB.transform;

            // Agregar componente visual
            var componenteB = pB.AddComponent<PuntoPatrullaje>();
            componenteB.nombrePunto = "B";
            componenteB.colorPunto = colorPuntoB;
            componenteB.tamanoPunto = tamanoPuntos;
        }

        Debug.Log("Puntos de patrullaje creados en " + gameObject.name);
        AsignarPuntosAEnemigos();
    }

    [ContextMenu("Eliminar Puntos de Patrullaje")]
    public void EliminarPuntosDePatrullaje()
    {
        if (puntoA != null)
        {
            if (Application.isPlaying)
                Destroy(puntoA.gameObject);
            else
                DestroyImmediate(puntoA.gameObject);
            puntoA = null;
        }

        if (puntoB != null)
        {
            if (Application.isPlaying)
                Destroy(puntoB.gameObject);
            else
                DestroyImmediate(puntoB.gameObject);
            puntoB = null;
        }

        Debug.Log("Puntos de patrullaje eliminados de " + gameObject.name);
    }

    [ContextMenu("Asignar Puntos a Enemigos")]
    public void AsignarPuntosAEnemigos()
    {
        if (puntoA == null || puntoB == null)
        {
            Debug.LogWarning("No hay puntos para asignar en " + gameObject.name);
            return;
        }

        // Buscar enemigos en el objeto padre
        Transform contenedorPrincipal = transform.parent;
        if (contenedorPrincipal != null)
        {
            EnemigosPatrullaje[] enemigos = contenedorPrincipal.GetComponentsInChildren<EnemigosPatrullaje>();

            foreach (EnemigosPatrullaje enemigo in enemigos)
            {
                enemigo.puntoA = puntoA;
                enemigo.puntoB = puntoB;
                Debug.Log("Puntos asignados a " + enemigo.gameObject.name);
            }

            if (enemigos.Length == 0)
            {
                Debug.LogWarning("No se encontraron enemigos con EnemigosPatrullaje en " + contenedorPrincipal.name);
            }
        }
    }

    [ContextMenu("Reorganizar Puntos")]
    public void ReorganizarPuntos()
    {
        if (puntoA != null && puntoB != null)
        {
            puntoA.position = transform.position + Vector3.left * (distanciaEntrePuntos / 2) + Vector3.up * alturaPuntos;
            puntoB.position = transform.position + Vector3.right * (distanciaEntrePuntos / 2) + Vector3.up * alturaPuntos;
            Debug.Log("Puntos reorganizados en " + gameObject.name);
        }
    }

    // Métodos para cambiar configuración en tiempo de ejecución
    public void CambiarDistanciaEntrePuntos(float nuevaDistancia)
    {
        distanciaEntrePuntos = nuevaDistancia;
        ReorganizarPuntos();
    }

    public void CambiarAlturaPuntos(float nuevaAltura)
    {
        alturaPuntos = nuevaAltura;
        ReorganizarPuntos();
    }

    // Métodos de utilidad
    public Vector3 ObtenerPosicionPuntoA()
    {
        return puntoA != null ? puntoA.position : Vector3.zero;
    }

    public Vector3 ObtenerPosicionPuntoB()
    {
        return puntoB != null ? puntoB.position : Vector3.zero;
    }

    public float ObtenerDistanciaEntrePuntos()
    {
        if (puntoA != null && puntoB != null)
            return Vector3.Distance(puntoA.position, puntoB.position);
        return 0f;
    }

    // Visualización en el editor
    void OnDrawGizmos()
    {
        // Dibujar conexión entre puntos
        if (puntoA != null && puntoB != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(puntoA.position, puntoB.position);

            // Dibujar centro del manager
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 0.2f);
        }

        // Mostrar área de influencia
        Gizmos.color = new Color(0, 1, 1, 0.1f);
        Gizmos.DrawSphere(transform.position, distanciaEntrePuntos);
    }

    void OnDrawGizmosSelected()
    {
        // Información adicional cuando está seleccionado
        if (puntoA != null && puntoB != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, puntoA.position);
            Gizmos.DrawLine(transform.position, puntoB.position);
        }
    }
}

// Clase para los puntos individuales de patrullaje
public class PuntoPatrullaje : MonoBehaviour
{
    [Header("Configuración del Punto")]
    public string nombrePunto = "Punto";
    public Color colorPunto = Color.red;
    public float tamanoPunto = 0.5f;

    [Header("Información del Punto")]
    public bool mostrarInformacion = true;
    public bool esPuntoSeguro = true; // Para mecánicas adicionales

    void OnDrawGizmos()
    {
        // Dibujar el punto
        Gizmos.color = colorPunto;
        Gizmos.DrawWireSphere(transform.position, tamanoPunto);

        // Rellenar el punto si está seleccionado
#if UNITY_EDITOR
        if (UnityEditor.Selection.activeTransform == transform)
        {
            Gizmos.color = new Color(colorPunto.r, colorPunto.g, colorPunto.b, 0.3f);
            Gizmos.DrawSphere(transform.position, tamanoPunto);
        }
#endif

        // Mostrar dirección hacia arriba
        Gizmos.color = Color.white;
        Gizmos.DrawRay(transform.position, Vector3.up * 0.5f);
    }

    void OnDrawGizmosSelected()
    {
        // Información adicional cuando está seleccionado
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, tamanoPunto * 1.5f);
    }

    // Métodos para interacciones
    public void MarcarComoVisitado()
    {
        Debug.Log("Punto " + nombrePunto + " visitado");
    }

    public bool EsPuntoSeguro()
    {
        return esPuntoSeguro;
    }
}