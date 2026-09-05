using UnityEngine;

public class ObjetoRecogible : MonoBehaviour, IInteractuable
{
    public string nombreItem = "Palanca";

    public void Interactuar(GameObject jugador)
    {
        InventarioJugador inventario = jugador.GetComponent<InventarioJugador>();
        if (inventario != null)
        {
            inventario.AgregarItem(nombreItem);
            Destroy(gameObject);
        }
    }

    public string ObtenerTextoInteraccion()
    {
        return "Presiona E para recoger " + nombreItem;
    }
}