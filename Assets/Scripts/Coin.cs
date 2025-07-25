using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Coin Settings")]
    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private float bounceHeight = 0.5f;
    [SerializeField] private float bounceSpeed = 2f;

    [Header("Effects")]
    [SerializeField] private bool destroyOnCollect = true;

    private Vector3 startPosition;
    private CoinGameManager gameManager;

    void Start()
    {
        // Enregistrer la position de d�part pour l'animation
        startPosition = transform.position;

        gameManager = FindObjectOfType<CoinGameManager>();

        if (gameManager == null)
        {
            Debug.LogError("CoinGameManager introuvable ! Assurez-vous qu'il y en a un dans la sc�ne.");
        }

        // S'assurer que la pi�ce a le bon tag
        if (!gameObject.CompareTag("Coin"))
        {
            gameObject.tag = "Coin";
        }
    }

    void Update()
    {
        // Animation de rotation
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // Animation de rebond
        float newY = startPosition.y + Mathf.Sin(Time.time * bounceSpeed) * bounceHeight;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CollectCoin();
        }
    }

    private void CollectCoin()
    {
        // Notifier le gestionnaire de jeu
        Debug.Log("Pi�ce collect�e !");
        Debug.Log(gameManager != null ? "Gestionnaire de jeu trouv�." : "Gestionnaire de jeu introuvable.");
        if (gameManager != null)
        {
            gameManager.CollectCoin();
        }

        // D�truire la pi�ce ou la d�sactiver
        if (destroyOnCollect)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
