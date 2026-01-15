using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NavigationDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI destinationNameText;
    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private TextMeshProUGUI directionText;
    [SerializeField] private TextMeshProUGUI bearingText;
    [SerializeField] private TextMeshProUGUI coordsText;
    [SerializeField] private Image directionArrow;
    [SerializeField] private GameObject navigationPanel;
    [SerializeField] private Button stopNavigationButton;

    private float updateInterval = 0.5f;

    void Start()
    {
        if (navigationPanel != null)
            navigationPanel.SetActive(false);

        if (stopNavigationButton != null)
            stopNavigationButton.onClick.AddListener(OnStopNavigationClick);

        InvokeRepeating("UpdateDisplay", 0f, updateInterval);
    }

    private void UpdateDisplay()
    {
        var navState = CampusNavigationManager.Instance.GetNavigationState();

        if (!navState.isNavigating || navState.selectedDestination == null)
        {
            if (navigationPanel != null)
                navigationPanel.SetActive(false);
            return;
        }

        if (navigationPanel != null)
            navigationPanel.SetActive(true);

        // Update destination name
        if (destinationNameText != null)
            destinationNameText.text = navState.selectedDestination.name;

        // Update distance
        if (distanceText != null)
        {
            if (navState.distanceToDestination < 1000)
                distanceText.text = $"Distance: {navState.distanceToDestination:F0} m";
            else
                distanceText.text = $"Distance: {navState.distanceToDestination / 1000f:F2} km";
        }

        // Update direction
        if (directionText != null)
        {
            string directionStr = GetCardinalDirection(navState.bearingToDestination);
            directionText.text = $"Direction: {directionStr}";
        }

        // Update bearing (absolute direction to destination)
        if (bearingText != null)
            bearingText.text = $"Bearing: {navState.bearingToDestination:F0}°";

        // Update user coordinates
        if (coordsText != null)
        {
            coordsText.text = $"You: {GPSLocationTracker.userLatLon.x:F6}, {GPSLocationTracker.userLatLon.y:F6}\n" +
                            $"Dest: {navState.selectedDestination.latitude:F6}, {navState.selectedDestination.longitude:F6}";
        }

        // Rotate arrow to point toward destination
        if (directionArrow != null)
        {
            float relativeAngle = navState.bearingToDestination - navState.userHeading;
            directionArrow.transform.rotation = Quaternion.Euler(0, 0, -relativeAngle);
        }
    }

    private string GetCardinalDirection(float bearing)
    {
        // Normalize bearing to 0-360
        bearing = (bearing + 360) % 360;

        if (bearing >= 337.5 || bearing < 22.5)
            return "North ↑";
        else if (bearing >= 22.5 && bearing < 67.5)
            return "Northeast ↗";
        else if (bearing >= 67.5 && bearing < 112.5)
            return "East →";
        else if (bearing >= 112.5 && bearing < 157.5)
            return "Southeast ↘";
        else if (bearing >= 157.5 && bearing < 202.5)
            return "South ↓";
        else if (bearing >= 202.5 && bearing < 247.5)
            return "Southwest ↙";
        else if (bearing >= 247.5 && bearing < 292.5)
            return "West ←";
        else
            return "Northwest ↖";
    }

    private void OnStopNavigationClick()
    {
        CampusNavigationManager.Instance.StopNavigation();
        if (navigationPanel != null)
            navigationPanel.SetActive(false);
        Debug.Log("Navigation stopped");
    }
}
