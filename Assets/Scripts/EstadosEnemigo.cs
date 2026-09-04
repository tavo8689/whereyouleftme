using UnityEngine;
using UnityEngine.AI;
public class EstadoEnemigo : MonoBehaviour
{
    [Header("Referencias")]
    public Transform jugador;
    public EstadoJugador estadoJugador;
    public Transform[] puntosDePatrulla;
    public Renderer rendererEnemigo;
    public Animator animador;
    [Header("Visión")]
    public float radioDeVision = 15f;
    [Range(0, 360)]
    public float anguloDeVision = 90f;
    public LayerMask mascaraObstaculos;
    public LayerMask mascaraJugador;
    [Header("Movimiento")]
    public float velocidadDeambular = 2f;
    public float velocidadPerseguir = 4.5f;
    public float tiempoDeBusqueda = 5f;
    [Header("Colores por estado")]
    public Color colorDeambular = Color.green;
    public Color colorPerseguir = Color.red;
    public Color colorBuscar = Color.yellow;
    private NavMeshAgent agente;
    private MaquinaEstadosEnemigo maquinaEstados;
    private int indicePatrullaActual = 0;
    private Vector3 ultimaPosicionConocida;
    private float temporizadorBusqueda;
    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        if (animador == null)
            animador = GetComponentInChildren<Animator>();
        if (rendererEnemigo == null)
            rendererEnemigo = GetComponentInChildren<Renderer>();
        if (puntosDePatrulla == null || puntosDePatrulla.Length == 0)
            puntosDePatrulla = BuscarPuntosDePatrullaPorTag();
        maquinaEstados = new MaquinaEstadosEnemigo();
        maquinaEstados.AlCambiarEstado += ManejarCambioDeEstado;
        agente.speed = velocidadDeambular;
        if (puntosDePatrulla.Length > 0)
            agente.SetDestination(puntosDePatrulla[0].position);
        ActualizarColor(MaquinaEstadosEnemigo.Estado.Deambular);
    }
    Transform[] BuscarPuntosDePatrullaPorTag()
    {
        GameObject[] objetosConTag = GameObject.FindGameObjectsWithTag("PuntoDePatrulla");
        // Ordenados por nombre, para que el orden sea predecible (ej: PuntoDePatrulla_1, _2, _3...)
        System.Array.Sort(objetosConTag, (a, b) => string.Compare(a.name, b.name));
        Transform[] puntos = new Transform[objetosConTag.Length];
        for (int i = 0; i < objetosConTag.Length; i++)
            puntos[i] = objetosConTag[i].transform;
        return puntos;
    }
    void Update()
    {
        bool puedeVerJugador = PuedeVerJugador();
        switch (maquinaEstados.estadoActual)
        {
            case MaquinaEstadosEnemigo.Estado.Deambular:
                Deambular();
                if (puedeVerJugador)
                    maquinaEstados.CambiarEstado(MaquinaEstadosEnemigo.Estado.Perseguir);
                break;
            case MaquinaEstadosEnemigo.Estado.Perseguir:
                if (puedeVerJugador)
                {
                    ultimaPosicionConocida = jugador.position;
                    agente.SetDestination(jugador.position);
                }
                else
                {
                    ultimaPosicionConocida = jugador.position;
                    temporizadorBusqueda = tiempoDeBusqueda;
                    maquinaEstados.CambiarEstado(MaquinaEstadosEnemigo.Estado.Buscar);
                }
                break;
            case MaquinaEstadosEnemigo.Estado.Buscar:
                agente.SetDestination(ultimaPosicionConocida);
                if (puedeVerJugador)
                {
                    maquinaEstados.CambiarEstado(MaquinaEstadosEnemigo.Estado.Perseguir);
                }
                else if (Vector3.Distance(transform.position, ultimaPosicionConocida) < 1f)
                {
                    temporizadorBusqueda -= Time.deltaTime;
                    if (temporizadorBusqueda <= 0f)
                        maquinaEstados.CambiarEstado(MaquinaEstadosEnemigo.Estado.Deambular);
                }
                break;
        }
        if (animador != null)
            animador.SetFloat("Velocidad", agente.velocity.magnitude);
    }
    // Se ejecuta automáticamente cada vez que la máquina de estados cambia de estado
    void ManejarCambioDeEstado(MaquinaEstadosEnemigo.Estado nuevoEstado)
    {
        switch (nuevoEstado)
        {
            case MaquinaEstadosEnemigo.Estado.Deambular:
                agente.speed = velocidadDeambular;
                if (puntosDePatrulla.Length > 0)
                    agente.SetDestination(puntosDePatrulla[indicePatrullaActual].position);
                break;
            case MaquinaEstadosEnemigo.Estado.Perseguir:
            case MaquinaEstadosEnemigo.Estado.Buscar:
                agente.speed = velocidadPerseguir;
                break;
        }
        ActualizarColor(nuevoEstado);
    }
    bool PuedeVerJugador()
    {
        if (jugador == null) return false;
        if (estadoJugador != null && estadoJugador.estaEscondido) return false;
        Vector3 direccionAlJugador = (jugador.position - transform.position);
        float distanciaAlJugador = direccionAlJugador.magnitude;
        if (distanciaAlJugador > radioDeVision) return false;
        float angulo = Vector3.Angle(transform.forward, direccionAlJugador);
        if (angulo > anguloDeVision / 2f) return false;
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
        if (puntosDePatrulla.Length == 0) return;
        if (agente.remainingDistance < 0.5f && !agente.pathPending)
        {
            indicePatrullaActual = (indicePatrullaActual + 1) % puntosDePatrulla.Length;
            agente.SetDestination(puntosDePatrulla[indicePatrullaActual].position);
        }
    }
    void ActualizarColor(MaquinaEstadosEnemigo.Estado estado)
    {
        if (rendererEnemigo == null) return;
        switch (estado)
        {
            case MaquinaEstadosEnemigo.Estado.Deambular:
                rendererEnemigo.material.color = colorDeambular;
                break;
            case MaquinaEstadosEnemigo.Estado.Perseguir:
                rendererEnemigo.material.color = colorPerseguir;
                break;
            case MaquinaEstadosEnemigo.Estado.Buscar:
                rendererEnemigo.material.color = colorBuscar;
                break;
        }
    }
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
