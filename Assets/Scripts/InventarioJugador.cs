using System.Collections.Generic;
using UnityEngine;

public class InventarioJugador : MonoBehaviour
{
    private List<string> items = new List<string>();

    public void AgregarItem(string nombreItem)
    {
        items.Add(nombreItem);
        Debug.Log("Item agregado al inventario: " + nombreItem);
    }

    public bool TieneItem(string nombreItem)
    {
        return items.Contains(nombreItem);
    }

    public void UsarItem(string nombreItem)
    {
        if (items.Contains(nombreItem))
            items.Remove(nombreItem);
    }
}