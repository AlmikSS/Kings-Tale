public struct ValidateResponseStruct
{
    public bool IsValidate;
    public string Message;

    public ValidateResponseStruct(bool isValidate, string message)
    {
        IsValidate = isValidate;
        Message = message;
    }
}