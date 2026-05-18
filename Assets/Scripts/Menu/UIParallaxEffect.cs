using UnityEngine;

public class UIParallaxEffect : MonoBehaviour
{
    [System.Serializable]
    public struct ParallaxLayer
    {
        public RectTransform rectTransform;
        [Tooltip("A mayor valor, más se moverá esta capa. Valores negativos invierten el sentido.")]
        public float speedMultiplier;
    }

    [Header("Capas de la UI")]
    [SerializeField] private ParallaxLayer[] layers;

    [Header("Configuración del Efecto Movimiento")]
    [SerializeField] private float maxMoveDistance = 50f;
    [SerializeField] private float smoothTime = 5f;

    [Header("Configuración del Brillo (Estrellas)")]
    public CanvasGroup canvasGroup;
    [Tooltip("Velocidad a la que pulsa/brilla el CanvasGroup.")]
    public float alphaIntensity = 2f; 
    [Range(0f, 1f)] [SerializeField] private float minAlpha = 0.3f;
    [Range(0f, 1f)] [SerializeField] private float maxAlpha = 1f;

    private Vector2[] startPositions;

    void Start()
    {
        if (layers == null || layers.Length == 0)
        {
            Debug.LogWarning("No has asignado capas en el script de Parallax.");
            return;
        }

        startPositions = new Vector2[layers.Length];
        for (int i = 0; i < layers.Length; i++)
        {
            if (layers[i].rectTransform != null)
            {
                startPositions[i] = layers[i].rectTransform.anchoredPosition;
            }
        }
    }

    void Update()
    {
        if (Time.timeScale != 1)
            Time.timeScale = 1;

        if (layers == null || layers.Length == 0) return;

        // 1. Obtener posición normalizada del ratón (-1 a 1)
        float mouseXNormalized = (Input.mousePosition.x / Screen.width) * 2f - 1f;
        float mouseYNormalized = (Input.mousePosition.y / Screen.height) * 2f - 1f;

        Vector2 targetMouseOffset = new Vector2(mouseXNormalized, mouseYNormalized);

        // 2. Lógica del Movimiento Parallax
        for (int i = 0; i < layers.Length; i++)
        {
            if (layers[i].rectTransform != null)
            {
                Vector2 targetOffset = targetMouseOffset * layers[i].speedMultiplier * maxMoveDistance;

                targetOffset.x = Mathf.Clamp(targetOffset.x, -maxMoveDistance, maxMoveDistance);
                targetOffset.y = Mathf.Clamp(targetOffset.y, -maxMoveDistance, maxMoveDistance);

                Vector2 targetPosition = startPositions[i] + targetOffset;

                layers[i].rectTransform.anchoredPosition = Vector2.Lerp(
                    layers[i].rectTransform.anchoredPosition,
                    targetPosition,
                    Time.deltaTime * smoothTime
                );
            }
        }

        // 3. Lógica del Brillo Autónomo (Independiente del ratón)
        if (canvasGroup != null)
        {
            // Mathf.Sin nos da un valor entre -1 y 1 que oscila constantemente en el tiempo.
            // Al sumarle 1 y dividir entre 2, convertimos ese rango de 0 a 1 de forma matemática suave.
            float pulseSin = (Mathf.Sin(Time.time * alphaIntensity) + 1f) / 2f;

            // Interpolamos el alpha directamente entre el mínimo y máximo usando la oscilación del seno
            float targetAlpha = Mathf.Lerp(minAlpha, maxAlpha, pulseSin);

            // Aplicamos el alpha de forma suavizada
            canvasGroup.alpha = Mathf.Lerp(
                canvasGroup.alpha,
                targetAlpha,
                Time.deltaTime * smoothTime
            );
        }
    }
}