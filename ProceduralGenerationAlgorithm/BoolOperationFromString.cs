/// <summary>
///static class to do a simple boolean operation using operator that comes from a string
/// </summary>
public static class BoolOperationFromString
{
    /// <summary>
    /// Run comparison using opertaor from a string. Usage: BoolOperationFromString.Compare(1,2"==")
    /// </summary>
    public static bool Compare(float a, float b, string operand)
    {
        switch (operand)
        {
            case "<":
                if (a < b)
                {
                    return true;
                }
                break;
            case "<=":
                if (a <= b)
                {
                    return true;
                }
                break;
            case ">":
                if (a > b)
                {
                    return true;
                }
                break;
            case ">=":
                if (a >= b)
                {
                    return true;
                }
                break;
            case "==":
                if (a == b)
                {
                    return true;
                }
                break;
            case "!=":
                if (a != b)
                {
                    return true;
                }
                break;
        }
        return false;
    }
}
