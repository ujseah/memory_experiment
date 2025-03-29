using System.IO;
using System.Linq;
using UnityEngine;

public class GazeSpatialLogger : MonoBehaviour
{
    public Transform headset;

    private StreamWriter writer;
    private string currentObject = "None";
    private float startTime = 0f;
    private Vector3 lastHitPoint;

    // List of objects to ignore during gaze logging
    private readonly string[] ignoredObjects = {
        "Left Wall", "Right Wall", "Front Wall", "Back Wall",
        "Ceiling", "Floor", "DoorTrigger", "PlayerMover"
    };

    void Start()
    {
        string folder = Application.persistentDataPath;
        string baseFileName = "SpatialMemoryLog";
        string extension = ".csv";

        int fileIndex = 0;
        string fullPath;

        // Find an unused filename
        do
        {
            string numberedName = fileIndex == 0 ? baseFileName : $"{baseFileName}({fileIndex})";
            fullPath = Path.Combine(folder, numberedName + extension);
            fileIndex++;
        } while (File.Exists(fullPath));

        writer = new StreamWriter(fullPath, false);
        writer.WriteLine("ObjectLookedAt,StartTime,EndTime,DwellTime,HitX,HitY,HitZ,HeadX,HeadY,HeadZ");
    }

    void Update()
    {
        string lookedAt = "None";
        Vector3 hitPoint = Vector3.zero;

        // Cast a ray from the headset's forward direction
        if (Physics.Raycast(headset.position, headset.forward, out RaycastHit hit, 100f))
        {
            string hitName = hit.collider.gameObject.name;

            // Skip logging if the object is in the ignore list
            if (!ignoredObjects.Contains(hitName))
            {
                lookedAt = hitName;
                hitPoint = hit.point;
            }
        }

        // If the gaze target changed, log the previous one
        if (lookedAt != currentObject)
        {
            float endTime = Time.time;
            float dwellTime = endTime - startTime;

            if (startTime > 0f && currentObject != "None")
            {
                Vector3 headPos = headset.position;

                writer.WriteLine($"{currentObject},{startTime:F2},{endTime:F2},{dwellTime:F2}," +
                                 $"{lastHitPoint.x:F2},{lastHitPoint.y:F2},{lastHitPoint.z:F2}," +
                                 $"{headPos.x:F2},{headPos.y:F2},{headPos.z:F2}");
            }

            currentObject = lookedAt;
            lastHitPoint = hitPoint;
            startTime = Time.time;
        }
    }

    void OnApplicationQuit()
    {
        // Log the final gaze before quitting
        float endTime = Time.time;
        float dwellTime = endTime - startTime;
        if (currentObject != "None")
        {
            Vector3 headPos = headset.position;

            writer.WriteLine($"{currentObject},{startTime:F2},{endTime:F2},{dwellTime:F2}," +
                             $"{lastHitPoint.x:F2},{lastHitPoint.y:F2},{lastHitPoint.z:F2}," +
                             $"{headPos.x:F2},{headPos.y:F2},{headPos.z:F2}");
        }

        writer.Close();
    }
}
