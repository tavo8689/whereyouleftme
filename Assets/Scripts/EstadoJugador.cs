using UnityEngine;

public class EstadoJugador : MonoBehaviour
{
    public bool estaEscondido { get; private set; } = false;
    public IInteractuable escondiiteActual { get; private set; }

    public void Esconderse(IInteractuable escondite)
    {
        estaEscondido = true;
        escondiiteActual = escondite;
    }

    public void SalirDeEsconderse()
    {
        estaEscondido = false;
        escondiiteActual = null;
    }
}