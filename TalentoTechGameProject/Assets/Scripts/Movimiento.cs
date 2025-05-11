using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(BoxCollider2D))]
public class Movimiento : MonoBehaviour
{
    [Header("Configuración Básica")]
    [SerializeField] private float velocidad = 5f;
    [SerializeField] private float fuerzaSalto = 12f;
    [SerializeField] private float distanciaSuelo = 0.2f;
    [SerializeField] private Transform verificadorSuelo;
    [SerializeField] private string tagSuelo = "Suelo"; // Tag para reconocer el suelo

    [Header("Configuración de Animación")]
    [SerializeField] private string parametroMovimiento = "Velocidad";
    [SerializeField] private string parametroEnSuelo = "EnSuelo";

    private Rigidbody2D rb;
    private Animator animator;
    private bool mirandoDerecha = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (verificadorSuelo == null)
        {
            GameObject check = new GameObject("VerificadorSuelo");
            check.transform.SetParent(transform);
            check.transform.localPosition = Vector3.down * 0.5f;
            verificadorSuelo = check.transform;
            Debug.LogWarning("Se creó automáticamente un verificador de suelo");
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump") && EstaEnSuelo())
        {
            Saltar();
        }
    }

    private void FixedUpdate()
    {
        float movimiento = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(movimiento * velocidad, rb.linearVelocity.y);

        animator.SetFloat(parametroMovimiento, Mathf.Abs(movimiento));
        animator.SetBool(parametroEnSuelo, EstaEnSuelo());

        if ((movimiento > 0 && !mirandoDerecha) || (movimiento < 0 && mirandoDerecha))
        {
            Voltear();
        }
    }

    private bool EstaEnSuelo()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(verificadorSuelo.position, distanciaSuelo);
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject != gameObject && collider.CompareTag(tagSuelo))
            {
                return true;
            }
        }
        return false;
    }

    private void Saltar()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
    }

    private void Voltear()
    {
        mirandoDerecha = !mirandoDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

    private void OnDrawGizmosSelected()
    {
        if (verificadorSuelo != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(verificadorSuelo.position, distanciaSuelo);
        }
    }
}