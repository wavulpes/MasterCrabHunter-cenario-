using UnityEngine;
using UnityEngine.InputSystem;

public class BancadaInteracao : MonoBehaviour
{
    [Tooltip("Tecla para interagir com a bancada")]
    public Key teclaInteracao = Key.E;

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

        bool teclaE = Keyboard.current[teclaInteracao].wasPressedThisFrame;
        bool espaco = Keyboard.current.spaceKey.wasPressedThisFrame;

        if (teclaE || espaco)
            FishSystemManager.Instance?.AbrirPopupEscolhaPeixe();
    }
}
