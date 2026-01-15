using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DestinationSelector : MonoBehaviour
{
    [SerializeField] private Button showDestinationsButton;
    [SerializeField] private GameObject destinationListPanel;
    [SerializeField] private Transform destinationListContent;
    [SerializeField] private GameObject destinationButtonPrefab;

    void Start()
    {
        if (showDestinationsButton != null)
            showDestinationsButton.onClick.AddListener(OnShowDestinationsClick);

        // Hide panel initially
        if (destinationListPanel != null)
            destinationListPanel.SetActive(false);

        // Load and display destinations
        RefreshDestinationList();
    }

    private void OnShowDestinationsClick()
    {
        if (destinationListPanel != null)
        {
            bool isActive = destinationListPanel.activeSelf;
            destinationListPanel.SetActive(!isActive);
        }
    }

    public void RefreshDestinationList()
    {
        // Clear existing buttons
        foreach (Transform child in destinationListContent)
        {
            Destroy(child.gameObject);
        }

        // Get available campus locations
        var locations = CampusNavigationManager.Instance.GetAvailableCampusLocations();

        if (locations.Count == 0)
        {
            Debug.Log("No campus locations available");
            return;
        }

        // Create button for each location
        foreach (var location in locations)
        {
            GameObject buttonObj = Instantiate(destinationButtonPrefab, destinationListContent);
            Button btn = buttonObj.GetComponent<Button>();
            TextMeshProUGUI btnText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

            if (btnText != null)
                btnText.text = location.name + "\n" + location.description;

            // Capture location in closure
            CampusLocationData locData = location;
            if (btn != null)
            {
                btn.onClick.AddListener(() => SelectDestination(locData));
            }
        }

        Debug.Log($"Created {locations.Count} destination buttons");
    }

    private void SelectDestination(CampusLocationData location)
    {
        Debug.Log($"Selected destination: {location.name}");
        CampusNavigationManager.Instance.StartNavigation(location);

        // Hide panel after selection
        if (destinationListPanel != null)
            destinationListPanel.SetActive(false);
    }

    public void StopNavigation()
    {
        CampusNavigationManager.Instance.StopNavigation();
        Debug.Log("Navigation stopped");
    }
}
