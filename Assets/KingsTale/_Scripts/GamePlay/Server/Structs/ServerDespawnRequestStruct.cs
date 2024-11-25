    using Unity.Netcode;

    public struct ServerDespawnRequestStruct : INetworkSerializable
    {
        public ulong Id;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Id);
        }
    }