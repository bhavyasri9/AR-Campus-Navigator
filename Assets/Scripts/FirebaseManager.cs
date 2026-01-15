using System;
using System.Threading.Tasks;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Auth;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance { get; private set; }

    private FirebaseDatabase database;
    private FirebaseAuth auth;
    private FirebaseUser currentUser;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    async void Start()
    {
        await InitializeFirebase();
    }

    private async Task InitializeFirebase()
    {
        try
        {
            // Initialize Firebase
            var checkAndFixResult = await FirebaseApp.CheckAndFixDependenciesAsync();
            if (checkAndFixResult == DependencyStatus.Available)
            {
                // Set the database URL explicitly
                var app = FirebaseApp.DefaultInstance;
                database = FirebaseDatabase.GetInstance(app, "https://arcampusnavigator-986c9.firebasedatabase.app");
                auth = FirebaseAuth.DefaultInstance;
                
                Debug.Log("Firebase initialized successfully");
                
                // Enable offline persistence
                database.SetPersistenceEnabled(true);
            }
            else
            {
                Debug.LogError("Firebase dependencies not available: " + checkAndFixResult);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Firebase initialization failed: " + e.Message);
        }
    }

    // Login with email and password
    public async Task LoginAsync(string email, string password)
    {
        try
        {
            var result = await auth.SignInWithEmailAndPasswordAsync(email, password);
            currentUser = result.User;
            Debug.Log("Login successful: " + currentUser.DisplayName);
        }
        catch (FirebaseException e)
        {
            Debug.LogError("Login failed: " + e.Message);
        }
    }

    // Register new user
    public async Task RegisterAsync(string email, string password, string displayName)
    {
        try
        {
            var result = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            currentUser = result.User;

            // Set display name
            var userProfile = new UserProfile { DisplayName = displayName };
            await currentUser.UpdateUserProfileAsync(userProfile);

            Debug.Log("Registration successful: " + displayName);
        }
        catch (FirebaseException e)
        {
            Debug.LogError("Registration failed: " + e.Message);
        }
    }

    // Logout
    public void Logout()
    {
        auth.SignOut();
        currentUser = null;
        Debug.Log("Logged out");
    }

    // Get current user
    public FirebaseUser GetCurrentUser()
    {
        return currentUser;
    }

    // Save destination to Firebase
    public async Task SaveDestinationAsync(string destinationName, float latitude, float longitude)
    {
        if (currentUser == null)
        {
            Debug.LogWarning("User not logged in. Cannot save destination.");
            return;
        }

        try
        {
            var destination = new DestinationData
            {
                name = destinationName,
                latitude = latitude,
                longitude = longitude,
                timestamp = DateTime.Now.ToString()
            };

            string json = JsonUtility.ToJson(destination);
            await database.GetReference("users")
                .Child(currentUser.UserId)
                .Child("destinations")
                .Child(destinationName)
                .SetValueAsync(json);

            Debug.Log("Destination saved: " + destinationName);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save destination: " + e.Message);
        }
    }

    // Load destinations for current user
    public async Task LoadDestinationsAsync(Action<DestinationData[]> callback)
    {
        if (currentUser == null)
        {
            Debug.LogWarning("User not logged in. Cannot load destinations.");
            return;
        }

        try
        {
            var snapshot = await database.GetReference("users")
                .Child(currentUser.UserId)
                .Child("destinations")
                .GetValueAsync();

            if (snapshot.Exists)
            {
                var destinations = new System.Collections.Generic.List<DestinationData>();
                foreach (var child in snapshot.Children)
                {
                    var data = JsonUtility.FromJson<DestinationData>(child.Value.ToString());
                    destinations.Add(data);
                }
                callback?.Invoke(destinations.ToArray());
            }
            else
            {
                callback?.Invoke(new DestinationData[0]);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to load destinations: " + e.Message);
        }
    }

    // Save campus location (for admin)
    public async Task SaveCampusLocationAsync(string buildingName, float latitude, float longitude, string description)
    {
        try
        {
            var location = new CampusLocationData
            {
                name = buildingName,
                latitude = latitude,
                longitude = longitude,
                description = description
            };

            string json = JsonUtility.ToJson(location);
            await database.GetReference("campusLocations")
                .Child(buildingName)
                .SetValueAsync(json);

            Debug.Log("Campus location saved: " + buildingName);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save campus location: " + e.Message);
        }
    }

    // Load all campus locations
    public async Task LoadCampusLocationsAsync(Action<CampusLocationData[]> callback)
    {
        try
        {
            var snapshot = await database.GetReference("campusLocations").GetValueAsync();

            if (snapshot.Exists)
            {
                var locations = new System.Collections.Generic.List<CampusLocationData>();
                foreach (var child in snapshot.Children)
                {
                    var data = JsonUtility.FromJson<CampusLocationData>(child.Value.ToString());
                    locations.Add(data);
                }
                callback?.Invoke(locations.ToArray());
            }
            else
            {
                callback?.Invoke(new CampusLocationData[0]);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to load campus locations: " + e.Message);
        }
    }
}

