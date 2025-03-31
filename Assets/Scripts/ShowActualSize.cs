using UnityEngine;

[ExecuteAlways]
public class ShowActualSize : MonoBehaviour
{
    [Header("World Size of Object")]
    public Vector3 worldSize;

    void Update()
    {
        if (TryGetComponent<Renderer>(out var renderer))
        {
            worldSize = renderer.bounds.size;
        }
    }
}
