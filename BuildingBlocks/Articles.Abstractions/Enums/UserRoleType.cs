using System.ComponentModel;

namespace Articles.Abstractions.Enums;

public enum UserRoleType
{
    // Cross domain: 1-9
    [Description("Editorial Office")]
    EOF = 1,

    // Submission domain: 11-19
    [Description("Author")]
    AUT = 11,
    [Description("Corresponding Author")]
    CORAUT = 12,

    [Description("Review Editor")]
    REVED = 21,

    [Description("User Admin")]
    USERADMIN = 91,

}

