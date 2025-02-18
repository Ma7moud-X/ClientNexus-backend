using System;
using Database.Models;

namespace Database.TypeExtensions;

public static class ProblemStatusExtensions
{
    public static char ToChar(this ProblemStatus status) {
        return (char)status;
    }

    public static ProblemStatus ToProblemStatus(this char status) {
        if (!Enum.IsDefined(typeof(ProblemStatus), (int)status))
        {
            throw new ArgumentException($"Invalid problem status character: {status}");
        }

        return (ProblemStatus)status;
    }
}
