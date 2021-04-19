namespace Zebble.Billing
{
    using Amazon;
    using Amazon.DynamoDBv2;
    using Olive;

    public class DynamoDbOptions
    {
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string RegionName { get; set; }

        RegionEndpoint Region => RegionName.HasValue() ? RegionEndpoint.GetBySystemName(RegionName) : RegionEndpoint.EUWest2;

        internal IAmazonDynamoDB CreateClient() => new AmazonDynamoDBClient(AccessKey, SecretKey, Region);
    }
}
