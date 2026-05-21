using UnityEngine;
using Unity.Cinemachine;

public class TrocaSala : MonoBehaviour
{
    public CinemachineCamera camSala1;
    public CinemachineCamera camSala2;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (other.transform.position.x < transform.position.x)
        {
            // Player veio da esquerda, indo para sala 2
            camSala1.Priority = 0;
            camSala2.Priority = 10;
        }
        else
        {
            // Player veio da direita, voltando para sala 1
            camSala1.Priority = 10;
            camSala2.Priority = 0;
        }
    }
}