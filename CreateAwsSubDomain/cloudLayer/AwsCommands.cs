using System;
using CreateAwsSubDomain.Interfaces;
using Amazon.Runtime;

namespace CreateAwsSubDomain.cloudLayer
{
    public class AwsCommands : ICloudAutomation
    {
        private ILogger logger;
        private AWSCredentials awsCredentials;
        private AwsProfile awsProfile;

        public AwsCommands(ILogger logger)
        {
            this.logger = logger;

            awsProfile = new AwsProfile(this.logger);

           // awsCredentials = awsProfile.TheAwsProfile();  //remove after refactor
        }

        public bool CreateS3WebsiteBucketWithIndexFile(string bucketName, string indexFile)
        {
            try
            {
                AwsS3Commands s3 = new AwsS3Commands(logger, awsProfile);

                s3.CreateS3Bucket(bucketName);

                s3.makeS3BucketWebsite(bucketName);

                s3.uploadIndexHtmlFileToS3Bucket(bucketName, indexFile);

                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Failed to create S3 bucket.\n{0}\n{1}", ex.Message, ex.StackTrace));
                return false;
            }
        }

        public bool CreateHostedZone(string zoneName)
        {            
            try
            {
                AwsRoute53Commands r53 = new AwsRoute53Commands(logger, awsProfile);

                HostedZoneInformation zoneInfo = r53.createHostedZoneAsync(zoneName);

                r53.create_A_ResourceRecords(zoneInfo.Id, zoneInfo.Name);
                
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Failed to create hosted zone.\n{0}\n{1}", ex.Message, ex.StackTrace));
                return false;
            }

        }

        public bool CreateParentNsRecord(string fullName, string parentName)
        {
            try
            {
                AwsRoute53Commands r53 = new AwsRoute53Commands(logger, awsProfile);

                r53.copyNsRecordsFromSubDomainToParentDomain(fullName, parentName);
                
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Failed to create parent NS record.\n{0}\n{1}", ex.Message, ex.StackTrace));
                return false;
            }
        }
        
    }
}
