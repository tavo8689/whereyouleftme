using UnityEngine;
// Interfaz que implementa cualquier objeto con el que el jugador pueda interactuar
public interface IInteractuable
{
    void Interactuar(GameObject jugador);
    string ObtenerTextoInteraccion();
}