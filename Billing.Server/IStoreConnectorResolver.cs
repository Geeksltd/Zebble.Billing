namespace Zebble.Billing
{
    public interface IStoreConnectorResolver
    {
        IStoreConnector Resolve(string storeName);
    }
}
