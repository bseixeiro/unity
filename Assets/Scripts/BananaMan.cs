using UnityEngine;

public class BananaMan : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float airControlMultiplier = 0.5f; // Contrôle en l'air réduit
    [SerializeField] private float rotationSpeed = 10f; // Vitesse de rotation du personnage

    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundLayerMask = 1; // Layer du sol
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private Transform groundCheckPoint; // Point de vérification du sol (aux pieds)

    [Header("Physics Settings")]
    [SerializeField] private float gravity = 20f; // Gravité additionnelle
    [SerializeField] private float maxFallSpeed = 20f;

    [Header("Camera Settings")]
    [SerializeField] private ThirdPersonCamera cameraController;

    // Composants
    private Rigidbody rb;

    // Variables de mouvement
    private Vector3 moveDirection;
    private Vector3 worldMoveDirection; // Direction de mouvement dans l'espace monde
    private bool isGrounded;
    private bool isSprinting;
    private float currentSpeed;

    void Start()
    {
        // Récupérer le Rigidbody
        rb = GetComponent<Rigidbody>();

        // Configuration du Rigidbody pour de meilleurs contrôles
        rb.freezeRotation = true; // Empêche la rotation non désirée

        // Si pas de point de vérification au sol défini, en créer un
        if (groundCheckPoint == null)
        {
            GameObject groundCheck = new GameObject("GroundCheck");
            groundCheck.transform.SetParent(transform);
            groundCheck.transform.localPosition = new Vector3(0, -0.5f, 0);
            groundCheckPoint = groundCheck.transform;
        }

        // Trouver automatiquement la caméra si elle n'est pas assignée
        if (cameraController == null)
        {
            cameraController = FindObjectOfType<ThirdPersonCamera>();
        }
    }

    void Update()
    {
        // Vérifier si on est au sol
        CheckGrounded();

        // Gérer les inputs
        HandleInput();

        // Appliquer le mouvement
        MovePlayer();

        // Gérer la rotation basée sur la caméra
        HandleRotation();

        // Gérer le saut
        HandleJump();

        // Appliquer la gravité additionnelle
        ApplyGravity();
    }

    private void CheckGrounded()
    {
        // Utiliser un SphereCast pour une détection plus fiable
        isGrounded = Physics.SphereCast(transform.position, 0.3f, Vector3.down, out RaycastHit hit, groundCheckDistance + 0.3f, groundLayerMask);

        // Alternative avec Raycast multiple pour plus de fiabilité
        if (!isGrounded)
        {
            isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance + 0.5f, groundLayerMask) ||
                        Physics.Raycast(transform.position + Vector3.forward * 0.2f, Vector3.down, groundCheckDistance + 0.5f, groundLayerMask) ||
                        Physics.Raycast(transform.position + Vector3.back * 0.2f, Vector3.down, groundCheckDistance + 0.5f, groundLayerMask) ||
                        Physics.Raycast(transform.position + Vector3.right * 0.2f, Vector3.down, groundCheckDistance + 0.5f, groundLayerMask) ||
                        Physics.Raycast(transform.position + Vector3.left * 0.2f, Vector3.down, groundCheckDistance + 0.5f, groundLayerMask);
        }

        // Debug visuel
        Debug.DrawRay(transform.position, Vector3.down * (groundCheckDistance + 0.5f), isGrounded ? Color.green : Color.red);
    }

    private void HandleInput()
    {
        // Récupérer les inputs de mouvement horizontal
        float horizontal = 0f;
        float vertical = 0f;

        // WASD ou Flèches directionnelles
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            vertical = 1f;
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            vertical = -1f;

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            horizontal = 1f;
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            horizontal = -1f;

        // Calculer la direction de mouvement locale
        moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        // Convertir la direction basée sur l'orientation de la caméra
        if (moveDirection.magnitude > 0.1f && cameraController != null)
        {
            Vector3 cameraForward = cameraController.GetCameraForward();
            Vector3 cameraRight = cameraController.GetCameraRight();

            worldMoveDirection = (cameraForward * moveDirection.z + cameraRight * moveDirection.x).normalized;
        }
        else
        {
            worldMoveDirection = Vector3.zero;
        }

        // Vérifier si on sprint
        isSprinting = Input.GetKey(KeyCode.LeftShift);

        // Déterminer la vitesse actuelle
        currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
    }

    private void MovePlayer()
    {
        if (worldMoveDirection.magnitude > 0.1f)
        {
            // Calculer la vélocité de mouvement
            Vector3 targetVelocity = worldMoveDirection * currentSpeed;

            // Si on est en l'air, réduire le contrôle
            if (!isGrounded)
            {
                targetVelocity *= airControlMultiplier;
            }

            // Appliquer le mouvement horizontal en préservant la vélocité verticale
            rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);
        }
        else
        {
            // Arrêter le mouvement horizontal quand aucune touche n'est pressée
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        }
    }

    private void HandleRotation()
    {
        // Faire tourner le personnage vers la direction du mouvement dans l'espace monde
        if (worldMoveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(worldMoveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void HandleJump()
    {
        // Saut uniquement si on est au sol et qu'on appuie sur Espace
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            // Appliquer la force de saut
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        }
    }

    private void ApplyGravity()
    {
        // Appliquer une gravité additionnelle pour un saut plus naturel
        if (!isGrounded)
        {
            rb.linearVelocity += Vector3.down * gravity * Time.deltaTime;

            // Limiter la vitesse de chute
            if (rb.linearVelocity.y < -maxFallSpeed)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, -maxFallSpeed, rb.linearVelocity.z);
            }
        }
    }

    // Fonctions utilitaires publiques
    public bool IsGrounded()
    {
        return isGrounded;
    }

    public bool IsSprinting()
    {
        return isSprinting && isGrounded;
    }

    public float GetCurrentSpeed()
    {
        return Vector3.Magnitude(new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z));
    }

    // Fonction pour ajuster les paramètres depuis d'autres scripts
    public void SetMovementSpeed(float newWalkSpeed, float newSprintSpeed)
    {
        walkSpeed = newWalkSpeed;
        sprintSpeed = newSprintSpeed;
    }

    public void SetJumpForce(float newJumpForce)
    {
        jumpForce = newJumpForce;
    }

    // Gizmos pour visualiser le point de vérification du sol dans l'éditeur
    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheckPoint.position, 0.1f);
            Gizmos.DrawLine(groundCheckPoint.position, groundCheckPoint.position + Vector3.down * groundCheckDistance);
        }
    }
}

// Script optionnel pour la collecte de pièces
public class CoinCollector : MonoBehaviour
{
    [Header("Collecte")]
    public int coinsCollected = 0;
    public float collectionRange = 1.5f;

    [Header("UI")]
    public UnityEngine.UI.Text coinText; // Assignez un Text UI pour afficher le score

    void Update()
    {
        CheckForCoins();
        UpdateUI();
    }

    void CheckForCoins()
    {
        // Chercher les pièces dans un rayon autour du joueur
        Collider[] coins = Physics.OverlapSphere(transform.position, collectionRange);

        foreach (Collider coin in coins)
        {
            if (coin.CompareTag("Coin"))
            {
                CollectCoin(coin.gameObject);
            }
        }
    }

    void CollectCoin(GameObject coin)
    {
        coinsCollected++;

        // Effet sonore ou visuel optionnel
        // AudioSource.PlayClipAtPoint(coinSound, coin.transform.position);

        Destroy(coin);
    }

    void UpdateUI()
    {
        if (coinText != null)
        {
            coinText.text = "Pièces: " + coinsCollected;
        }
    }

    void OnDrawGizmosSelected()
    {
        // Visualiser le rayon de collecte dans l'éditeur
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, collectionRange);
    }
}