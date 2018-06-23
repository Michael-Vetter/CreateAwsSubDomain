using System;
using System.Collections.Generic;
using System.Text;
using CreateAwsSubDomain.Interfaces;


namespace CreateAwsSubDomain.businessLayer
{
    public class CreateSubDomain

    {
        private ICloudAutomation cloudContext;
        private ILogger logger;

        public CreateSubDomain(ICloudAutomation cloudContext, ILogger logger)
        {
            this.cloudContext = cloudContext;
            this.logger = logger;
        }

        public bool execute(string subdomainName)
        {
            /*
             * Create index.html file with "hello subdomain"
             * Get parent domain and verify exists in Route53
             * Create S3 bucket with name of subdomain (and parent name)
             * Create Route 53 hosted zone for subdomain; get NS records; 
             * Create Alias record pointed to S3 bucket
             * Create NS records in Parent domain.
             * 
             */
             
            DomainName domainName = Helpers.parseDomainName(subdomainName.ToLower());  //AWS requires lower case

            string IndexFile = Helpers.CreateIndexDotHtml(domainName.sub);

            if (IndexFile.Length > 0)
                logger.LogInformation(IndexFile);
            else
            {
                logger.LogError("Index file not created");
                return false;
            }

            if (!cloudContext.CreateS3WebsiteBucketWithIndexFile(domainName.fullName, IndexFile))
                return false;

            if (!cloudContext.CreateHostedZone(domainName.fullName))
                return false;

            if (!cloudContext.CreateParentNsRecord(domainName.fullName, domainName.parent))
                return false;
            
            logger.LogInformation("complete");

            return true;
        }

        
    }
}
