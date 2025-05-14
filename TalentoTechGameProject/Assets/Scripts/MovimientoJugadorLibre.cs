using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(SpriteRenderer))]
public class MovimientoJugadorLibre : MonoBehaviour
{
    [SerializeField] private float velocidad = 5f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector2 movimiento;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        float movimientoX = Input.GetAxisRaw("Horizontal");
        float movimientoY = Input.GetAxisRaw("Vertical");

        movimiento = new Vector2(movimientoX, movimientoY).normalized;

        animator.SetBool("isWalking", movimiento.magnitude > 0);

        // 🔁 Espejar sprite según dirección horizontal
        if (movimientoX != 0)
            spriteRenderer.flipX = movimientoX < 0;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = movimiento * velocidad;
    }
}
