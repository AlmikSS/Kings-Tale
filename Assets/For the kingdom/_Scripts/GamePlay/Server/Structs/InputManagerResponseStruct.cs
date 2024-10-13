public struct InputManagerResponseStruct
{
    public bool IsValidate { get; private set; }
    public string Message { get; private set; }

    public InputManagerResponseStruct(bool isValidate, string message)
    {
        IsValidate = isValidate;
        Message = message;
    }
}