using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("R�f�rences")]
    public Transform player;
    public Transform cameraTarget; // Un GameObject vide enfant du joueur pour la position de la cam�ra

    [Header("Param�tres de la cam�ra")]
    public float distance = 5f;
    public float height = 2f;
    public float mouseSensitivity = 2f;
    public float smoothSpeed = 10f;

    [Header("Limites de rotation")]
    public float minVerticalAngle = -30f;
    public float maxVerticalAngle = 60f;

    private float horizontalAngle = 0f;
    private float verticalAngle = 0f;
    private Vector3 targetPosition;

    [Header("Contr�le du curseur")]
    public bool lockCursor = true;

    void Start()
    {
        // Verrouiller le curseur au centre de l'�cran
        SetCursorLock(true);

        // Initialiser les angles bas�s sur la rotation actuelle
        Vector3 angles = transform.eulerAngles;
        horizontalAngle = angles.y;
        verticalAngle = angles.x;
    }

    void LateUpdate()
    {
        if (player == null) return;

        HandleCameraRotation();
        UpdateCameraPosition();
    }

    void HandleCameraRotation()
    {
        // R�cup�rer les entr�es de la souris
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Mettre � jour les angles
        horizontalAngle += mouseX;
        verticalAngle -= mouseY; // Inverser pour un contr�le naturel

        // Limiter l'angle vertical
        verticalAngle = Mathf.Clamp(verticalAngle, minVerticalAngle, maxVerticalAngle);
    }

    void UpdateCameraPosition()
    {
        // Calculer la position cible bas�e sur les angles
        Quaternion rotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0);
        Vector3 direction = rotation * Vector3.back;

        // Position cible de la cam�ra
        Vector3 playerPos = cameraTarget != null ? cameraTarget.position : player.position + Vector3.up * height;
        targetPosition = playerPos + direction * distance;

        // Appliquer la position et rotation avec smoothing
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        transform.LookAt(playerPos);
    }

    // M�thode publique pour obtenir la direction de la cam�ra (utilis�e par le PlayerController)
    public Vector3 GetCameraForward()
    {
        Vector3 forward = transform.forward;
        forward.y = 0; // Garder seulement la composante horizontale
        return forward.normalized;
    }

    public Vector3 GetCameraRight()
    {
        Vector3 right = transform.right;
        right.y = 0; // Garder seulement la composante horizontale
        return right.normalized;
    }

    public void SetCursorLock(bool locked)
    {
        lockCursor = locked;
        if (locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

    }
        public void ToggleCursorLock()
        {
            SetCursorLock(!lockCursor);
        }

        public bool IsCursorLocked()
        {
            return lockCursor;
        }
}