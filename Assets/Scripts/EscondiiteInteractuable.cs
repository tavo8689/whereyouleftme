using UnityEngine;

public class EscondiiteInteractuable : MonoBehaviour, IInteractuable
{
    [Header("Referencias")]
    public Transform puntoDeEscondite;

    private bool jugadorDentro = false;

    public void Interactuar(GameObject jugador)
    {
        EstadoJugador estado = jugador.GetComponent<EstadoJugador>();
        if (estado == null) return;

        var controller = jugador.GetComponent<CharacterController>();

        if (!jugadorDentro)
        {
            jugador.transform.position = puntoDeEscondite.position;
            estado.Esconderse();
            jugadorDentro = true;
            if (controller != null) controller.enabled = false;
        }
        else
        {
            estado.SalirDeEsconderse();
            jugadorDentro = false;
            if (controller != null) controller.enabled = true;
        }
    }

    public string ObtenerTextoInteraccion()
    {
        return jugadorDentro ? "Presiona E para salir" : "Presiona E para esconderte";
    }
}