public struct InputManagerResponseStruct
{
    public string Answer { get; private set; }
    public bool IsValidate { get; private set; }

    public InputManagerResponseStruct(string answer, bool isValidate)
    {
        Answer = answer;
        IsValidate = isValidate;
    }
}