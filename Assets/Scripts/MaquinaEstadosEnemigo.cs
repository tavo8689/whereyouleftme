using System;

public class MaquinaEstadosEnemigo
{
    public enum Estado { Deambular, Perseguir, Buscar }
    public Estado estadoActual { get; private set; } = Estado.Deambular;
    // Evento que se dispara cada vez que cambia de estado
    public event Action<Estado> AlCambiarEstado;
    public void CambiarEstado(Estado nuevoEstado)
    {
        if (estadoActual == nuevoEstado) return; // evita disparar el evento si no cambió nada
        estadoActual = nuevoEstado;
        AlCambiarEstado?.Invoke(estadoActual);
    }
}
