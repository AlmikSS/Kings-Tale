using Unity.Collections;
using Unity.Netcode;

public struct ValidateResponseStruct : INetworkSerializable
{
    public bool IsValidate;
    public FixedString4096Bytes Message;

    public ValidateResponseStruct(bool isValidate, string message)
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