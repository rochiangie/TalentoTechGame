using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class MovimientoJugadorLibre : MonoBehaviour
{
    public float velocidadMovimiento = 5f;
    private Rigidbody2D rb;
    private Animator animator;

    private Vector2 inputMovimiento;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        inputMovimiento.x = Input.GetAxisRaw("Horizontal");
        inputMovimiento.y = Input.GetAxisRaw("Vertical");

        animator.SetBool("isWalking", inputMovimiento != Vector2.zero);

        if (inputMovimiento.x != 0)
        {
            Vector3 escala = transform.localScale;
            float escalaOriginal = Mathf.Abs(escala.x);
            escala.x = escalaOriginal * Mathf.Sign(inputMovimiento.x);
            transform.localScale = escala;
        }

    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + inputMovimiento.normalized * velocidadMovimiento * Time.fixedDeltaTime);
    }
}
