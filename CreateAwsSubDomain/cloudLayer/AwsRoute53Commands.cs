using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CreateAwsSubDomain.Interfaces;
using Amazon;
using Amazon.Runtime;
using Amazon.Route53;
using Amazon.Route53.Model;

namespace CreateAwsSubDomain.cloudLayer
{
    class AwsRoute53Commands
    {
        private ILogger logger;
        private AWSCredentials awsProfileCredentials;
        private AmazonRoute53Client client;
        private RegionEndpoint regionEndPoint;


        public AwsRoute53Commands(ILogger logger, AwsProfile awsProfile)
        {
            this.logger = logger;
            awsProfileCredentials = awsProfile.TheAwsProfile();
            regionEndPoint = RegionEndpoint.USEast1;
        }

        public AwsRoute53Commands(ILogger logger, AwsProfile awsProfile, RegionEndpoint regionEndPoint)
        {
            this.logger = logger;
            awsProfileCredentials = awsProfile.TheAwsProfile();
            this.regionEndPoint = regionEndPoint;
        }

        private AmazonRoute53Client TheRoute53Client()
        {

            if (client == null)
            {
                logger.LogInformation("Creating Route53 client...");
                client = new AmazonRoute53Client(awsProfileCredentials, regionEndPoint);
            }

            logger.LogInformation("Returning Route53 client...");
            return client;
        }

        public HostedZoneInformation createHostedZoneAsync(string zoneName)
        {
            Task<CreateHostedZoneResponse> R53response = createAwsHostedZoneAsync(zoneName);
            R53response.Wait();

            return new HostedZoneInformation() {
                Id = R53response.Result.HostedZone.Id,
                Name = R53response.Result.HostedZone.Name
            };
        }

        private async Task<CreateHostedZoneResponse> createAwsHostedZoneAsync(string zoneName)
        {
            logger.LogInformation($"Creating hosted zone '{ zoneName }'...");

            CreateHostedZoneRequest request = new CreateHostedZoneRequest
            {
                CallerReference = zoneName,
                Name = zoneName
            };

            CreateHostedZoneResponse zone = await TheRoute53Client().CreateHostedZoneAsync(request);
            return zone;

        }

        public void create_A_ResourceRecords(string hostedZoneId, string hostName)
        {
            Task<ChangeResourceRecordSetsResponse> ResourceResponse = createAws_A_ResourceRecords(hostedZoneId, hostName);
            ResourceResponse.Wait();
        }

        private async Task<ChangeResourceRecordSetsResponse> createAws_A_ResourceRecords(string hostedZoneId, string hostName)
        {

            ChangeBatch changes = new ChangeBatch();
            string newHostName = hostName.Substring(0, hostName.Length - 1);

            logger.LogInformation($"Creating Alias Record for hosted zone { newHostName }...");

            ResourceRecordSet aliasRs = new ResourceRecordSet()
            {
                AliasTarget = new AliasTarget()
                {
                    HostedZoneId = "Z3AQBSTGFYJSTF",  //use zone id from here: https://docs.aws.amazon.com/general/latest/gr/rande.html#s3_region
                    DNSName = "s3-website-us-east-1.amazonaws.com",
                    EvaluateTargetHealth = false
                },
                Type = RRType.A,
                Name = newHostName

            };
            Change AliasRecord = new Change(ChangeAction.CREATE, aliasRs);
            changes.Changes.Add(AliasRecord);

            ChangeResourceRecordSetsRequest request = new ChangeResourceRecordSetsRequest()
            {
                ChangeBatch = changes,
                HostedZoneId = hostedZoneId
            };

            ChangeResourceRecordSetsResponse response = await TheRoute53Client().ChangeResourceRecordSetsAsync(request);
            return response;
        }

        public void copyNsRecordsFromSubDomainToParentDomain(string fullSubDomainName, string parentName)
        {
            Task<ListHostedZonesResponse> ListOfHostedZones = listAwsHostedZonesAsync();
            ListOfHostedZones.Wait();

            HostedZone subZone = ListOfHostedZones.Result.HostedZones.Find(h => h.Name == $"{ fullSubDomainName }.");
            HostedZone parentZone = ListOfHostedZones.Result.HostedZones.Find(h => h.Name == $"{ parentName }.");

            Task<ListResourceRecordSetsResponse> getResourceRecords = getAwsResourceRecordsAsync(subZone.Id);
            getResourceRecords.Wait();
            ResourceRecordSet rrs = getResourceRecords.Result.ResourceRecordSets.Find(r => r.Name == $"{ fullSubDomainName }." && r.Type == RRType.NS);


            Task<ChangeResourceRecordSetsResponse> ResourceResponse = createAws_NS_ResourceRecords(parentZone.Id, fullSubDomainName, rrs);
            ResourceResponse.Wait();

        }
        
        

        private async Task<ListHostedZonesResponse> listAwsHostedZonesAsync()
        {
            logger.LogInformation($"Getting List of hosted zones...");

            ListHostedZonesRequest request = new ListHostedZonesRequest { };

            ListHostedZonesResponse zone = await TheRoute53Client().ListHostedZonesAsync(request);
            return zone;

        }

        private async Task<ListResourceRecordSetsResponse> getAwsResourceRecordsAsync(string hostedZoneId)
        {
            logger.LogInformation($"Getting List of Resource Records for hosted zone ID { hostedZoneId }...");

            ListResourceRecordSetsRequest request = new ListResourceRecordSetsRequest
            {
                HostedZoneId = hostedZoneId
            };

            ListResourceRecordSetsResponse records = await TheRoute53Client().ListResourceRecordSetsAsync(request);
            return records;

        }

        private async Task<ChangeResourceRecordSetsResponse> createAws_NS_ResourceRecords(string hostedZoneId, string fullName, ResourceRecordSet NsRecord)
        {
            logger.LogInformation($"Creating NS record for hosted zone ID { hostedZoneId }, { fullName }...");

            ChangeBatch changes = new ChangeBatch();

            Change change = new Change(ChangeAction.CREATE, NsRecord);
            changes.Changes.Add(change);

            ChangeResourceRecordSetsRequest request = new ChangeResourceRecordSetsRequest()
            {
                ChangeBatch = changes,
                HostedZoneId = hostedZoneId
            };

            ChangeResourceRecordSetsResponse response = await TheRoute53Client().ChangeResourceRecordSetsAsync(request);
            return response;
        }

    }

    public class HostedZoneInformation
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
