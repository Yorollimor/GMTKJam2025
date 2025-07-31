using UnityEngine;
using UnityEngine.UI;

public class ArrowVisualizer : MonoBehaviour
{
    [Header("Arrow Settings")]
    public GameObject arrowPrefab;  // A prefab with a SpriteRenderer for the arrow.
    public float arrowScale = 1f;   // Scale the arrow size by this factor.
    public float maxArrowLength = 1f; // Max length of arrow for visualization (normalization).

    private Solver fluid;

    void Start()
    {
        fluid = FindObjectOfType<Solver>();
        // Create arrows for each cell in the fluid simulation
        CreateArrowField();
    }

    void Update()
    {
        // Update the velocity field (and arrows) every frame
        VisualizeArrows();
    }

    void CreateArrowField()
    {
        // Loop through each grid cell and instantiate an arrow object at each position
        int fluidGridSize = fluid.N + 2;
        for (int i = 0; i < fluidGridSize; i++)
        {
            for (int j = 0; j < fluidGridSize; j++)
            {
                Vector2 position = new Vector2(i * fluidGridSize, j * fluidGridSize);

                // Create an arrow for each grid cell, and set its position
                GameObject arrow = Instantiate(arrowPrefab, new Vector3(0,0,0), Quaternion.identity, transform);
                arrow.name = $"Arrow_{i}_{j}";
                arrow.SetActive(true);
            }
        }
    }

    void VisualizeArrows()
    {
        int n = fluid.N + 2;

        for (int i = 1; i < n - 1; i++)
        {
            for (int j = 1; j < n- 1; j++)
            {
                int idx = i * n + j;
                Vector2 vel = fluid.GetVelocityAtGridPoint(i, j);
                float u = vel.x;      // Get the u component (horizontal)
                float v = vel.y;  // Get the v component (vertical)

                // Find the arrow GameObject corresponding to the current grid cell
                Transform arrow = transform.GetChild(idx);

                // Calculate the length and angle of the arrow
                float length = Mathf.Min(maxArrowLength, Mathf.Sqrt(u * u + v * v)) * arrowScale;
                float angle = Mathf.Atan2(v, u) * -Mathf.Rad2Deg + 90;

                // Set the position and rotation of the arrow
                arrow.GetComponent<RectTransform>().anchoredPosition = new Vector2(i , j );
                arrow.rotation = Quaternion.Euler(0, 0, angle);
                arrow.localScale = new Vector3(1f, length, 1f);
                float angle01 = (angle + 180) / 360;
                arrow.GetComponentInChildren<Image>().color =
                    new Color(angle01/3, angle01/3*2, angle01);
            }
        }
    }
}
