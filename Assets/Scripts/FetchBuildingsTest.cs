using UnityEngine;
using Firebase.Firestore;
using System.Collections.Generic;

public class FetchBuildingsTest : MonoBehaviour
{
    private FirebaseFirestore db;

    void Start()
    {
        // Initialize Firestore
        db = FirebaseFirestore.DefaultInstance;
        Debug.Log("FetchBuildingsTest initialized");
    }

    // Fetch all buildings
    public void FetchAllBuildings()
    {
        Debug.Log("\n=== Fetching ALL BUILDINGS ===");
        
        if (db == null)
        {
            db = FirebaseFirestore.DefaultInstance;
            Debug.Log("Firestore reinitialized");
        }
        
        db.Collection("buildings").GetSnapshotAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("✗ Firestore Error: " + task.Exception);
                return;
            }
            
            if (task.IsCompleted)
            {
                QuerySnapshot snapshot = task.Result;
                
                if (snapshot.Count > 0)
                {
                    Debug.Log($"✓ Found {snapshot.Count} buildings:\n");
                    
                    int count = 0;
                    foreach (DocumentSnapshot doc in snapshot.Documents)
                    {
                        count++;
                        Dictionary<string, object> data = doc.ToDictionary();
                        
                        string buildingId = data.ContainsKey("building_id") ? data["building_id"].ToString() : "N/A";
                        string name = data.ContainsKey("name") ? data["name"].ToString() : "N/A";
                        string category = data.ContainsKey("category") ? data["category"].ToString() : "N/A";
                        
                        Debug.Log($"{count}. [{buildingId}] {name} ({category})");
                    }
                }
                else
                {
                    Debug.Log("✗ No buildings found");
                }
            }
        });
    }

    // Fetch all path nodes
    public void FetchAllPathNodes()
    {
        Debug.Log("\n=== Fetching ALL PATH NODES ===");
        
        if (db == null)
        {
            db = FirebaseFirestore.DefaultInstance;
            Debug.Log("Firestore reinitialized");
        }
        
        db.Collection("path_nodes").GetSnapshotAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("✗ Firestore Error: " + task.Exception);
                return;
            }
            
            if (task.IsCompleted)
            {
                QuerySnapshot snapshot = task.Result;
                
                if (snapshot.Count > 0)
                {
                    Debug.Log($"✓ Found {snapshot.Count} path nodes:\n");
                    
                    int count = 0;
                    foreach (DocumentSnapshot doc in snapshot.Documents)
                    {
                        count++;
                        Dictionary<string, object> data = doc.ToDictionary();
                        
                        string nodeId = data.ContainsKey("nodeId") ? data["nodeId"].ToString() : "N/A";
                        string routeId = data.ContainsKey("routeId") ? data["routeId"].ToString() : "N/A";
                        string landmark = data.ContainsKey("landmarkName") ? data["landmarkName"].ToString() : "N/A";
                        
                        Debug.Log($"{count}. [{nodeId}] Route: {routeId}, Landmark: {landmark}");
                    }
                }
                else
                {
                    Debug.Log("✗ No path nodes found");
                }
            }
        });
    }

    // Fetch all routes
    public void FetchAllRoutes()
    {
        Debug.Log("\n=== Fetching ALL ROUTES ===");
        
        if (db == null)
        {
            db = FirebaseFirestore.DefaultInstance;
            Debug.Log("Firestore reinitialized");
        }
        
        db.Collection("routes").GetSnapshotAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("✗ Firestore Error: " + task.Exception);
                return;
            }
            
            if (task.IsCompleted)
            {
                QuerySnapshot snapshot = task.Result;
                
                if (snapshot.Count > 0)
                {
                    Debug.Log($"✓ Found {snapshot.Count} routes:\n");
                    
                    int count = 0;
                    foreach (DocumentSnapshot doc in snapshot.Documents)
                    {
                        count++;
                        Dictionary<string, object> data = doc.ToDictionary();
                        
                        string routeId = data.ContainsKey("routeId") ? data["routeId"].ToString() : "N/A";
                        string purpose = data.ContainsKey("routePurpose") ? data["routePurpose"].ToString() : "N/A";
                        
                        Debug.Log($"{count}. Route {routeId}: {purpose}");
                    }
                }
                else
                {
                    Debug.Log("✗ No routes found");
                }
            }
        });
    }

    // Fetch everything
    public void FetchEverything()
    {
        Debug.Log("\n╔════════════════════════════════════════╗");
        Debug.Log("║  FETCHING ALL CAMPUS NAVIGATION DATA  ║");
        Debug.Log("╚════════════════════════════════════════╝");
        
        FetchAllBuildings();
        FetchAllPathNodes();
        FetchAllRoutes();
    }
}


