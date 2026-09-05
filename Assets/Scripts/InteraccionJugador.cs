using UnityEngine;
using UnityEngine.InputSystem;

public class InteraccionJugador : MonoBehaviour
{
    [Header("Referencias")]
    public Transform camaraJugador;
    public LayerMask capaInteractuable;

    [Header("Configuración")]
    public float distanciaInteraccion = 3f;

    private IInteractuable objetivoActual;

    void Update()
    {
        DetectarInteractuable();

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (objetivoActual != null)
                objetivoActual.Interactuar(gameObject);
        }
    }

    void DetectarInteractuable()
    {
        objetivoActual = null;

        if (Physics.Raycast(camaraJugador.position, camaraJugador.forward,
            out RaycastHit impacto, distanciaInteraccion, capaInteractuable))
        {
            objetivoActual = impacto.collider.GetComponent<IInteractuable>();
        }
    }

    public string ObtenerTextoActual()
    {
        return objetivoActual != null ? objetivoActual.ObtenerTextoInteraccion() : "";
    }
}