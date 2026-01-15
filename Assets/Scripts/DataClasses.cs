using System;
using System.Collections.Generic;

/// <summary>
/// Firestore data classes for campus navigation
/// </summary>
/// 
[System.Serializable]
public class BuildingData
{
    public string building_id;
    public string name;
    public string category;
    public int floors;
    public GPSCoordinates gps;
    public List<Entrance> entrances;
}

[System.Serializable]
public class GPSCoordinates
{
    public double latitude;
    public double longitude;
}

[System.Serializable]
public class Entrance
{
    public string entrance_id;
    public string name;
    public double latitude;
    public double longitude;
}

[System.Serializable]
public class PathNodeData
{
    public string nodeId;
    public string routeId;
    public double latitude;
    public double longitude;
    public string nodeType;
    public int order;
    public string movementInstruction;
    public string turnDirection;
    public string landmarkName;
    public List<string> connectedTo;
}

[System.Serializable]
public class RouteData
{
    public int routeId;
    public string startNodeId;
    public string direction;
    public List<int> destinationBuildings;
    public string routePurpose;
    public string pathType;
}

[System.Serializable]
public class DestinationData
{
    public string name;
    public float latitude;
    public float longitude;
    public string timestamp;
}

[System.Serializable]
public class CampusLocationData
{
    public string name;
    public float latitude;
    public float longitude;
    public string description;
}
