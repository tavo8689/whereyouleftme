using UnityEngine;

public class EstadoJugador : MonoBehaviour
{
    public bool estaEscondido { get; private set; } = false;
    public void Esconderse()
    {
        estaEscondido = true;
    }
    public void SalirDeEsconderse()
    {
        estaEscondido = false;
    }
}
