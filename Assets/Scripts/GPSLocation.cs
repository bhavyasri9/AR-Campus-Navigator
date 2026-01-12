using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPSLocation : MonoBehaviour
{
    public static Vector2 userLatLon;

    IEnumerator Start()
    {
        if (!Input.location.isEnabledByUser)
            yield break;

        Input.location.Start();

        while (Input.location.status == LocationServiceStatus.Initializing)
            yield return new WaitForSeconds(1);

        if (Input.location.status == LocationServiceStatus.Running)
        {
            userLatLon = new Vector2(
                Input.location.lastData.latitude,
                Input.location.lastData.longitude
            );
        }
    }
}
