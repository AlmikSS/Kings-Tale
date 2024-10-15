using Unity.Netcode;

public struct InputManagerResponseStruct : INetworkSerializable
{
    public bool IsValidate;
    public string Message;

    public InputManagerResponseStruct(bool isValidate, string message)
    {
        IsValidate = isValidate;
        Message = message;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref IsValidate);
        serializer.SerializeValue(ref Message);
    }
}