using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DestinationLoader : MonoBehaviour
{
    [SerializeField] private Button loadDestinationsButton;
    [SerializeField] private Transform destinationListParent;
    [SerializeField] private GameObject destinationItemPrefab;

    void Start()
    {
        if (loadDestinationsButton != null)
            loadDestinationsButton.onClick.AddListener(OnLoadDestinationsClick);
    }

    private async void OnLoadDestinationsClick()
    {
        if (FirebaseManager.Instance.GetCurrentUser() == null)
        {
            Debug.LogWarning("User not logged in");
            return;
        }

        Debug.Log("Loading destinations from Firebase...");
        
        await FirebaseManager.Instance.LoadDestinationsAsync((destinations) =>
        {
            // Clear existing items
            foreach (Transform child in destinationListParent)
                Destroy(child.gameObject);

            // Create item for each destination
            foreach (var dest in destinations)
            {
                GameObject item = Instantiate(destinationItemPrefab, destinationListParent);
                Text text = item.GetComponentInChildren<Text>();
                if (text != null)
                    text.text = dest.name + " (" + dest.latitude + ", " + dest.longitude + ")";
                
                Debug.Log("Loaded destination: " + dest.name);
            }
        });
    }
}
