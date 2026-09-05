using UnityEngine;

public class ObstaculoDespejable : MonoBehaviour, IInteractuable
{
    public string itemNecesario = "Palanca";
    public bool consumirItemAlUsar = false;

    public void Interactuar(GameObject jugador)
    {
        InventarioJugador inventario = jugador.GetComponent<InventarioJugador>();
        if (inventario == null) return;

        if (inventario.TieneItem(itemNecesario))
        {
            if (consumirItemAlUsar)
                inventario.UsarItem(itemNecesario);

            gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Necesitás: " + itemNecesario);
        }
    }

    public string ObtenerTextoInteraccion()
    {
        return "Presiona E para despejar (necesitás " + itemNecesario + ")";
    }
}
