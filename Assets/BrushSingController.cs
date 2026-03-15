using UnityEngine;

public class BrushSingController : MonoBehaviour
{
    private Transform player;
    [SerializeField] private Transform brush;
    [SerializeField] private GameObject pointer;
    [SerializeField] private float showDistance = 5f;
    [SerializeField] private Vector3 offset;

    private void Start()
    {
        player = GameObject.Find("Player").transform;
        pointer.transform.parent = null;
    }

    private void LateUpdate()
    {
        pointer.transform.position = brush.position + offset;
        float yAngle = brush.eulerAngles.y;
        pointer.transform.rotation = Quaternion.Euler(0f, yAngle, 0f);
    }
    void Update()
    {
        float dist = Vector3.Distance(player.position, brush.position);
        bool shouldShow = dist > showDistance;

        if (pointer.activeSelf != shouldShow)
        {
            pointer.SetActive(shouldShow);
        }
    }
}
