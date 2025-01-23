public static class Converters
{
    public static int PowerToAttackPower(float power)
    {
        if (power < 0.3f)
        {
            return 0;
        }
        else if(power < 0.6f)
        {
            return 1;
        }
        else if (power < 0.9f)
        {
            return 2;
        }
        else
        {
            return 3;
        }
    }
}
