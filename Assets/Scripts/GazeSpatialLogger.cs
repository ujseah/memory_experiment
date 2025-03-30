using System.IO;
using System.Linq;
using UnityEngine;

public class GazeSpatialLogger : MonoBehaviour
{
    public Transform headset;
    public float forgivenessDuration = 0.15f;
    public float forcedLogInterval = 0.05f;

    private StreamWriter writer;
    private float playStartTime;

    private string currentObject = "None";
    private string stableObject = "None";
    private float stableStartTime = 0f;
    private float timeLastSeen = 0f;
    private Vector3 lastHitPoint;
    private Vector3 lastHeadPosition;

    private readonly string[] ignoredObjects = {
        "Left Wall", "Right Wall", "Front Wall", "Back Wall",
        "Ceiling", "Floor", "DoorTrigger", "PlayerMover"
    };

    void Start()
    {
        playStartTime = Time.time;

        string folder = Application.persistentDataPath;
        string baseFileName = "SpatialMemoryLog";
        string extension = ".csv";

        int fileIndex = 0;
        string fullPath;
        do
        {
            string numbered = fileIndex == 0 ? baseFileName : $"{baseFileName}({fileIndex})";
            fullPath = Path.Combine(folder, numbered + extension);
            fileIndex++;
        } while (File.Exists(fullPath));

        writer = new StreamWriter(fullPath, false);
        writer.WriteLine("ObjectLookedAt,StartTime,EndTime,DwellTime,HitX,HitY,HitZ,HeadX,HeadY,HeadZ");
    }

    void Update()
    {
        string hitName = "None";
        Vector3 hitPoint = Vector3.zero;

        if (Physics.Raycast(headset.position, headset.forward, out RaycastHit hit, 100f))
        {
            string objName = hit.collider.gameObject.name;
            if (!ignoredObjects.Contains(objName))
            {
                hitName = objName;
                hitPoint = hit.point;
            }
        }

        // ✅ Skip if no valid object is being looked at
        if (hitName == "None") return;

        float currentTime = Time.time;

        if (hitName == currentObject)
        {
            timeLastSeen = currentTime;

            // Forced log for long gaze
            if (stableObject == hitName && (currentTime - stableStartTime >= forcedLogInterval))
            {
                float dwellTime = currentTime - stableStartTime;
                writer.WriteLine($"{stableObject},{stableStartTime:F2},{currentTime:F2},{dwellTime:F2}," +
                                 $"{lastHitPoint.x:F2},{lastHitPoint.y:F2},{lastHitPoint.z:F2}," +
                                 $"{lastHeadPosition.x:F2},{lastHeadPosition.y:F2},{lastHeadPosition.z:F2}");

                stableStartTime = currentTime;
                timeLastSeen = currentTime;
            }
        }
        else if (hitName == stableObject && (currentTime - timeLastSeen <= forgivenessDuration))
        {
            timeLastSeen = currentTime;
        }
        else
        {
            if (stableObject != "None")
            {
                float dwellTime = currentTime - stableStartTime;
                writer.WriteLine($"{stableObject},{stableStartTime:F2},{currentTime:F2},{dwellTime:F2}," +
                                 $"{lastHitPoint.x:F2},{lastHitPoint.y:F2},{lastHitPoint.z:F2}," +
                                 $"{lastHeadPosition.x:F2},{lastHeadPosition.y:F2},{lastHeadPosition.z:F2}");
            }

            stableObject = hitName;
            stableStartTime = currentTime;
            timeLastSeen = currentTime;
            lastHitPoint = hitPoint;
            lastHeadPosition = headset.position;
        }

        currentObject = hitName;
    }

    public void LogPlaytimeAndClose()
    {
        float playDuration = Time.time - playStartTime;
        writer.WriteLine($"TOTAL_PLAYTIME,,,{playDuration:F2},,,,,,");
        writer.Close();
        Debug.Log($"Playtime logged and file closed. Total playtime: {playDuration:F2} seconds.");
    }

    void OnApplicationQuit()
    {
        float endTime = Time.time;
        float dwellTime = endTime - stableStartTime;

        if (stableObject != "None")
        {
            writer.WriteLine($"{stableObject},{stableStartTime:F2},{endTime:F2},{dwellTime:F2}," +
                             $"{lastHitPoint.x:F2},{lastHitPoint.y:F2},{lastHitPoint.z:F2}," +
                             $"{lastHeadPosition.x:F2},{lastHeadPosition.y:F2},{lastHeadPosition.z:F2}");
        }

        writer.Close();
    }
}
