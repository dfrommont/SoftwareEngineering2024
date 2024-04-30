public enum RiskCardType
{
    Infantry,
    Cavalry,
    Artillery,
    Wild
}

public class RiskCard
{
    private RiskCardType type;
    private int countryID;

    public RiskCard(RiskCardType type, int countryID)
    {
        this.type = type;
        this.countryID = countryID;
    }

    public override string ToString()
    {
        return $"countryID: {countryID} type: {type} Card";
    }

    public int getCountryID()
    {
        return countryID;
    }

    public RiskCardType getRiskCardType()
    {
        return type;
    }
}