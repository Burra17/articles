namespace Blocks.Core.FluenValidation;

public static class ValidationMessages
{
    public static readonly string InvalidId = "The {0} should be greater than 0.";
    public static readonly string MaxLengthExceeded = "The {0} should not exceed {1} characters.";
    public static readonly string NullOrEmptyValue = "{0} is required.";
}
