namespace DataProcessor.Domain;

public class DefaultDictionaryComparer
{
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }
}

public class MeasurementsDictionaryComparer: IEqualityComparer<object>
{
    public new bool Equals(object? x, object? y)
    {
        throw new NotImplementedException();
    }

    public int GetHashCode(object obj)
    {
        throw new NotImplementedException();
    }
}