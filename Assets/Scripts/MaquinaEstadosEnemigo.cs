using System;

public class MaquinaEstadosEnemigo
{
    public enum Estado { Deambular, Alertar, Perseguir, Buscar }

    public Estado estadoActual { get; private set; } = Estado.Deambular;

    public event Action<Estado> AlCambiarEstado;

    public void CambiarEstado(Estado nuevoEstado)
    {
        if (estadoActual == nuevoEstado) return;
        estadoActual = nuevoEstado;
        AlCambiarEstado?.Invoke(estadoActual);
    }
}