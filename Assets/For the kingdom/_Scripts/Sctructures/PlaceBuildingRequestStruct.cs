using UnityEngine;

[System.Serializable]
public struct PlaceBuildingRequestStruct
{
    public ushort PlayerId { get; private set; }
    public ushort BuildingId { get; private set; }
    public Vector3 Position { get; private set; }

    public PlaceBuildingRequestStruct(ushort playerId, ushort buildingId, Vector3 position)
    {
        PlayerId = playerId;
        BuildingId = buildingId;
        Position = position;
    }
}