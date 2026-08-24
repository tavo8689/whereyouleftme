using UnityEngine;
using UnityEngine.AI;

public class EnemyChaser : MonoBehaviour
{
    public enum Estado { Deambular, Perseguir, Buscar }
    [Header("Referencias")]
    public Transform jugador;
    public Transform[] puntosDePatrulla;
    public Renderer rendererEnemigo; // el Mesh Renderer del modelo, para cambiar de color
    [Header("Visión")]
    public float radioDeVision = 15f;
    [Range(0, 360)]
    public float anguloDeVision = 90f;
    public LayerMask mascaraObstaculos;   // paredes, puertas, etc.
    public LayerMask mascaraJugador;      // capa del jugador
    [Header("Movimiento")]
    public float velocidadDeambular = 2f;
    public float velocidadPerseguir = 4.5f;
    public float tiempoDeBusqueda = 5f;   // segundos buscando antes de rendirse
    [Header("Colores por estado")]
    public Color colorDeambular = Color.green;
    public Color colorPerseguir = Color.red;
    public Color colorBuscar = Color.yellow;
    private NavMeshAgent agente;
    private Estado estadoActual = Estado.Deambular;
    private int indicePatrullaActual = 0;
    private Vector3 ultimaPosicionConocida;
    private float temporizadorBusqueda;

    public Animator componenteAnimator;


    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        agente.speed = velocidadDeambular;
        // Busca el Renderer en el objeto o en cualquiera de sus hijos
        if (rendererEnemigo == null)
            rendererEnemigo = GetComponentInChildren<Renderer>();
        if (puntosDePatrulla.Length > 0)
            agente.SetDestination(puntosDePatrulla[0].position);
        ActualizarColor();
    }
    void Update()
    {
        bool puedeVerJugador = PuedeVerJugador();
        switch (estadoActual)
        {
            case Estado.Deambular:
                Deambular();
                if (puedeVerJugador)
                    EntrarEnPerseguir();
                break;
            case Estado.Perseguir:
                if (puedeVerJugador)
                {
                    ultimaPosicionConocida = jugador.position;
                    agente.SetDestination(jugador.position);
                }
                else
                {
                    EntrarEnBuscar();
                }
                break;
            case Estado.Buscar:
                agente.SetDestination(ultimaPosicionConocida);
                if (puedeVerJugador)
                {
                    EntrarEnPerseguir();
                }
                else if (Vector3.Distance(transform.position, ultimaPosicionConocida) < 1f)
                {
                    temporizadorBusqueda -= Time.deltaTime;
                    if (temporizadorBusqueda <= 0f)
                        EntrarEnDeambular();
                }
                break;
        }
    }
    bool PuedeVerJugador()
    {
        if (jugador == null) return false;
        Vector3 direccionAlJugador = (jugador.position - transform.position);
        float distanciaAlJugador = direccionAlJugador.magnitude;
        if (distanciaAlJugador > radioDeVision) return false;
        float angulo = Vector3.Angle(transform.forward, direccionAlJugador);
        if (angulo > anguloDeVision / 2f) return false;
        // Chequeo de obstáculos entre el enemigo y el jugador
        if (Physics.Raycast(transform.position + Vector3.up, direccionAlJugador.normalized,
            out RaycastHit impacto, distanciaAlJugador, mascaraObstaculos | mascaraJugador))
        {
            if (((1 << impacto.collider.gameObject.layer) & mascaraJugador) != 0)
                return true;
        }
        return false;
    }
    void Deambular()
    {
        componenteAnimator.SetInteger("Momento", 1);
        if (puntosDePatrulla.Length == 0) return;
        if (agente.remainingDistance < 0.5f && !agente.pathPending)
        {
            indicePatrullaActual = (indicePatrullaActual + 1) % puntosDePatrulla.Length;
            agente.SetDestination(puntosDePatrulla[indicePatrullaActual].position);
        }
    }
    void EntrarEnPerseguir()
    {
        componenteAnimator.SetInteger("Momento", 2);
        estadoActual = Estado.Perseguir;
        agente.speed = velocidadPerseguir;
        ActualizarColor();
    }
    void EntrarEnBuscar()
    {
        componenteAnimator.SetInteger("Momento", 3);
        estadoActual = Estado.Buscar;
        agente.speed = velocidadPerseguir;
        ultimaPosicionConocida = jugador.position;
        temporizadorBusqueda = tiempoDeBusqueda;
        ActualizarColor();
    }
    void EntrarEnDeambular()
    {
        componenteAnimator.SetInteger("Momento", 1);
        estadoActual = Estado.Deambular;
        agente.speed = velocidadDeambular;
        if (puntosDePatrulla.Length > 0)
            agente.SetDestination(puntosDePatrulla[indicePatrullaActual].position);
        ActualizarColor();
    }
    void ActualizarColor()
    {
        if (rendererEnemigo == null) return;
        switch (estadoActual)
        {
            case Estado.Deambular:
                rendererEnemigo.material.color = colorDeambular;
                break;
            case Estado.Perseguir:
                rendererEnemigo.material.color = colorPerseguir;
                break;
            case Estado.Buscar:
                rendererEnemigo.material.color = colorBuscar;
                break;
        }
    }
    // Para visualizar el cono de visión en el editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radioDeVision);
        Vector3 limiteIzquierdo = DireccionDesdeAngulo(-anguloDeVision / 2f);
        Vector3 limiteDerecho = DireccionDesdeAngulo(anguloDeVision / 2f);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + limiteIzquierdo * radioDeVision);
        Gizmos.DrawLine(transform.position, transform.position + limiteDerecho * radioDeVision);
    }
    Vector3 DireccionDesdeAngulo(float anguloEnGrados)
    {
        anguloEnGrados += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(anguloEnGrados * Mathf.Deg2Rad), 0, Mathf.Cos(anguloEnGrados * Mathf.Deg2Rad));
    }
}