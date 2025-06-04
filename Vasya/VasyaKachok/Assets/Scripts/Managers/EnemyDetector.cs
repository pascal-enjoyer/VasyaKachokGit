using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private SpriteRenderer markerPrefab; // Префаб спрайта (красный круг)
    [SerializeField] private CharacterCombat characterCombat;
    [SerializeField] private string sortingLayerName = "TargetIndicator"; // Слой для спрайта
    [SerializeField] private int sortingOrder = 5; // Порядок в слое (над землей)
    [SerializeField] private float baseHeightOffset = 0.1f; // Отступ от земли
    [SerializeField] private float baseScale = 1.5f; // Базовый размер круга
    [SerializeField] private float screenScale = 0.05f; // Масштаб для экранного размера
    
    private SpriteRenderer currentMarker;
    private Camera mainCamera;

    private void Start()
    {
        if (characterCombat == null)
        {
            Debug.LogError("CharacterCombat not assigned!");
            enabled = false;
            return;
        }
        if (markerPrefab == null)
        {
            Debug.LogError("MarkerPrefab not assigned!");
            enabled = false;
            return;
        }
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found!");
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        GameObject closestEnemy = FindClosestEnemy(transform.position);

        if (closestEnemy != null && currentMarker == null)
        {
            characterCombat.SetTarget(closestEnemy);
            ShowMarkerBelow(closestEnemy);
        }
        else if (closestEnemy == null && currentMarker !=null)
        {
            characterCombat.SetTarget(null);
            RemoveMarker();
        }
    }

    private GameObject FindClosestEnemy(Vector3 origin)
    {
        Collider[] hits = Physics.OverlapSphere(origin, detectionRadius, enemyLayer);
        GameObject closest = null;
        float minDist = float.MaxValue;

        foreach (var hit in hits)
        {
            float dist = Vector3.Distance(origin, hit.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = hit.gameObject;
            }
        }

        return closest;
    }

    private void ShowMarkerBelow(GameObject target)
    {
        if (markerPrefab == null || target == null) return;

        // Удаляем старый маркер
        RemoveMarker();

        // Вычисляем позицию у основания врага
        float heightOffset = baseHeightOffset;
        Renderer targetRenderer = target.GetComponentInChildren<Renderer>();
        if (targetRenderer != null)
        {
            heightOffset += //targetRenderer.bounds.min.y - 
                target.transform.position.y;
        }
        else
        {
            Collider targetCollider = target.GetComponent<Collider>();
            if (targetCollider != null)
                heightOffset += //targetCollider.bounds.min.y 
                    - target.transform.position.y;
        }

        // Создаем новый маркер
        currentMarker = Instantiate(markerPrefab, target.transform);
        currentMarker.transform.localPosition = new Vector3(0, heightOffset, 0);
        currentMarker.transform.rotation = Quaternion.Euler(90f, 0f, 0f); // Лежит на земле
        currentMarker.sortingLayerName = sortingLayerName;
        currentMarker.sortingOrder = sortingOrder;

        // Настраиваем материал
        if (currentMarker.material == null)
            currentMarker.material = new Material(Shader.Find("Sprites/Default"));

        // Масштаб для экранного размера
        float distance = Vector3.Distance(mainCamera.transform.position, currentMarker.transform.position);
        float scale = distance * screenScale + baseScale;
        currentMarker.transform.localScale = new Vector3(scale, scale, scale);
    }

    private void RemoveMarker()
    {
        if (currentMarker != null)
        {
            Destroy(currentMarker.gameObject);
            currentMarker = null;
        }
    }
}