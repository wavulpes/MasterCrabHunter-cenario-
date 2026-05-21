using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float velocidade = 5f;
    private Rigidbody2D rb;
    private Vector2 movimento;
    
    // VARIÁVEL ADICIONADA: Cria a caixinha para guardar o Animator
    private Animator meuAnimator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // CÓDIGO ADICIONADO: Diz para a Unity buscar o Animator que está no Player
        meuAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        Vector2 input = Vector2.zero;

        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
            input.y = 1;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
            input.y = -1;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            input.x = -1;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            input.x = 1;

        movimento = input;

        // LÓGICA ADICIONADA: Controla a transição das animações
        if (movimento != Vector2.zero)
        {
            // Se estiver se movendo, avisa o Animator para tocar a caminhada
            meuAnimator.SetBool("isWalking", true);
        }
        else
        {
            // Se o movimento for zero, volta para o estado parado (Idle)
            meuAnimator.SetBool("isWalking", false);
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movimento.normalized * velocidade * Time.fixedDeltaTime);
    }
}