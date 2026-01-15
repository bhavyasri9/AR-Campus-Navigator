using UnityEngine;
using System.Collections.Generic;

public class CampusNavigationManager : MonoBehaviour
{
    public static CampusNavigationManager Instance { get; private set; }

    [System.Serializable]
    public class NavigationState
    {
        public CampusLocationData selectedDestination;
        public float distanceToDestination; // in meters
        public float bearingToDestination; // in degrees (0-360)
        public float userHeading; // device heading
        public bool isNavigating;
    }

    private NavigationState currentNavState = new NavigationState();
    private List<CampusLocationData> availableCampusLocations = new List<CampusLocationData>();
    private float updateInterval = 1f; // Update navigation every 1 second

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        LoadCampusLocations();
        GPSLocationTracker.OnLocationUpdated += OnUserLocationUpdated;
        InvokeRepeating("UpdateNavigation", 0f, updateInterval);
    }

    private async void LoadCampusLocations()
    {
        Debug.Log("Loading campus locations from Firebase...");
        await FirebaseManager.Instance.LoadCampusLocationsAsync((locations) =>
        {
            availableCampusLocations = new List<CampusLocationData>(locations);
            Debug.Log($"Loaded {locations.Length} campus locations");
            foreach (var loc in locations)
            {
                Debug.Log($"  - {loc.name}: ({loc.latitude}, {loc.longitude})");
            }
        });
    }

    public void StartNavigation(CampusLocationData destination)
    {
        currentNavState.selectedDestination = destination;
        currentNavState.isNavigating = true;
        Debug.Log($"Started navigation to {destination.name}");
    }

    public void StopNavigation()
    {
        currentNavState.isNavigating = false;
        currentNavState.selectedDestination = null;
        Debug.Log("Navigation stopped");
    }

    private void OnUserLocationUpdated(Vector2 newLocation)
    {
        UpdateNavigation();
    }

    private void UpdateNavigation()
    {
        if (!currentNavState.isNavigating || currentNavState.selectedDestination == null)
            return;

        if (!GPSLocationTracker.isGPSReady)
            return;

        // Calculate distance and bearing
        float distance = CalculateDistance(
            GPSLocationTracker.userLatLon.x,
            GPSLocationTracker.userLatLon.y,
            currentNavState.selectedDestination.latitude,
            currentNavState.selectedDestination.longitude
        );

        float bearing = CalculateBearing(
            GPSLocationTracker.userLatLon.x,
            GPSLocationTracker.userLatLon.y,
            currentNavState.selectedDestination.latitude,
            currentNavState.selectedDestination.longitude
        );

        // Get device heading from compass
        currentNavState.userHeading = Input.compass.trueHeading;

        currentNavState.distanceToDestination = distance;
        currentNavState.bearingToDestination = bearing;

        Debug.Log($"Distance: {distance:F1}m, Bearing: {bearing:F1}°, Your Heading: {currentNavState.userHeading:F1}°");
    }

    /// <summary>
    /// Calculate distance between two GPS coordinates in meters (Haversine formula)
    /// </summary>
    private float CalculateDistance(float lat1, float lon1, float lat2, float lon2)
    {
        const float R = 6371000f; // Earth radius in meters

        float dLat = Mathf.Deg2Rad * (lat2 - lat1);
        float dLon = Mathf.Deg2Rad * (lon2 - lon1);

        float a = Mathf.Sin(dLat / 2f) * Mathf.Sin(dLat / 2f) +
                  Mathf.Cos(Mathf.Deg2Rad * lat1) * Mathf.Cos(Mathf.Deg2Rad * lat2) *
                  Mathf.Sin(dLon / 2f) * Mathf.Sin(dLon / 2f);

        float c = 2f * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1f - a));
        float distance = R * c;

        return distance;
    }

    /// <summary>
    /// Calculate bearing (direction) from point A to point B
    /// </summary>
    private float CalculateBearing(float lat1, float lon1, float lat2, float lon2)
    {
        float dLon = Mathf.Deg2Rad * (lon2 - lon1);
        float lat1Rad = Mathf.Deg2Rad * lat1;
        float lat2Rad = Mathf.Deg2Rad * lat2;

        float y = Mathf.Sin(dLon) * Mathf.Cos(lat2Rad);
        float x = Mathf.Cos(lat1Rad) * Mathf.Sin(lat2Rad) -
                  Mathf.Sin(lat1Rad) * Mathf.Cos(lat2Rad) * Mathf.Cos(dLon);

        float bearing = Mathf.Rad2Deg * Mathf.Atan2(y, x);
        bearing = (bearing + 360f) % 360f; // Normalize to 0-360

        return bearing;
    }

    public NavigationState GetNavigationState()
    {
        return currentNavState;
    }

    public List<CampusLocationData> GetAvailableCampusLocations()
    {
        return availableCampusLocations;
    }

    void OnDestroy()
    {
        GPSLocationTracker.OnLocationUpdated -= OnUserLocationUpdated;
    }
}
