using UnityEngine;
using UnityEngine.InputSystem;

public class NpcDonoLojaMulta : MonoBehaviour
{
    [Tooltip("Tecla para entregar a multa ao NPC")]
    public Key teclaEntregar = Key.F;

    private bool playerPerto;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerPerto = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerPerto = false;
    }

    private void Update()
    {
        if (!playerPerto || Keyboard.current == null) return;

        if (Keyboard.current[teclaEntregar].wasPressedThisFrame)
            FishSystemManager.Instance?.EntregarMulta();
    }
}
