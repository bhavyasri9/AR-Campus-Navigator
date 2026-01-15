using UnityEngine;
using System;
using System.Collections;

public class GPSLocationTracker : MonoBehaviour
{
    public static Vector2 userLatLon = Vector2.zero;
    public static float userAltitude = 0f;
    public static bool isGPSReady = false;
    public static Action<Vector2> OnLocationUpdated;

    private float updateInterval = 5f; // Update GPS every 5 seconds

    IEnumerator Start()
    {
        if (!Input.location.isEnabledByUser)
        {
            Debug.LogWarning("GPS is disabled by user");
            yield break;
        }

        Input.location.Start(100f, 100f); // accuracy: 100 meters

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait < 1)
        {
            Debug.LogError("GPS initialization timed out");
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Running)
        {
            isGPSReady = true;
            Debug.Log("GPS is ready");
            StartCoroutine(UpdateGPSLocation());
        }
        else
        {
            Debug.LogError("GPS failed or was denied permission");
        }
    }

    private IEnumerator UpdateGPSLocation()
    {
        while (isGPSReady)
        {
            if (Input.location.status == LocationServiceStatus.Running)
            {
                userLatLon = new Vector2(
                    Input.location.lastData.latitude,
                    Input.location.lastData.longitude
                );
                userAltitude = Input.location.lastData.altitude;

                OnLocationUpdated?.Invoke(userLatLon);
                
                Debug.Log($"GPS Updated: Lat={userLatLon.x:F6}, Lon={userLatLon.y:F6}, Alt={userAltitude:F2}m");
            }

            yield return new WaitForSeconds(updateInterval);
        }
    }

    void OnDestroy()
    {
        if (Input.location.isEnabledByUser)
            Input.location.Stop();
    }
}
