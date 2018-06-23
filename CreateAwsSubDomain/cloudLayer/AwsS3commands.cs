using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CreateAwsSubDomain.Interfaces;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

namespace CreateAwsSubDomain.cloudLayer
{
    class AwsS3Commands
    {
        private ILogger logger;
        private AmazonS3Client client;
        private RegionEndpoint regionEndPoint;
        private AWSCredentials awsProfileCredentials;

        public AwsS3Commands(ILogger logger, AwsProfile awsProfile)
        {
            this.logger = logger;
            awsProfileCredentials = awsProfile.TheAwsProfile();
            regionEndPoint = RegionEndpoint.USEast1;
        }

        public AwsS3Commands(ILogger logger, AwsProfile awsProfile, RegionEndpoint regionEndPoint)
        {
            this.logger = logger;
            awsProfileCredentials = awsProfile.TheAwsProfile();
            this.regionEndPoint = regionEndPoint;
        }

        private AmazonS3Client TheS3Client()
        {

            if (client == null)
            {
                logger.LogInformation("Creating S3 client...");
                client = new AmazonS3Client(awsProfileCredentials, regionEndPoint);
            }

            logger.LogInformation("Returning S3 client...");
            return client;
        }

        public void CreateS3Bucket(string bucketName)
        {
            Task<PutBucketResponse> S3response = createAwsS3Bucket(bucketName);
            S3response.Wait();
        }

        //bucket will be created in US-East-1 region
        //If you specify us-east-1, you will get an error.  
        //If you don't specify a region, it will use us-east-1
        //If you specify another region, it works
        //not worth my time to code for any region.
        private async Task<PutBucketResponse> createAwsS3Bucket(string bucketName)
        {
            logger.LogInformation($"Creating S3 bucket '{ bucketName }'...");

            PutBucketRequest request = new PutBucketRequest
            {
                BucketName = bucketName,
                CannedACL = S3CannedACL.PublicRead  // make bucket publicly readable                
            };

            PutBucketResponse bucket = await TheS3Client().PutBucketAsync(request);
            return bucket;

        }

        public void makeS3BucketWebsite(string bucketName)
        {
            Task<PutBucketWebsiteResponse> S3WebsiteResponse = makeAwsS3BucketWebsite(bucketName);
            S3WebsiteResponse.Wait();
        }

        private async Task<PutBucketWebsiteResponse> makeAwsS3BucketWebsite(string bucketName)
        {
            logger.LogInformation($"Turning S3 bucket '{ bucketName }' into website...");

            PutBucketWebsiteRequest request = new PutBucketWebsiteRequest
            {
                BucketName = bucketName,
                WebsiteConfiguration = new WebsiteConfiguration
                {
                    IndexDocumentSuffix = "index.html",
                    ErrorDocument = "index.html"
                }
                //raises error if you specify us-east-1 region, but goes there if you don't specify it.
            };

            PutBucketWebsiteResponse bucket = await TheS3Client().PutBucketWebsiteAsync(request);
            return bucket;

        }

        public void uploadIndexHtmlFileToS3Bucket(string bucketName, string indexFile)
        {
            Task<PutObjectResponse> S3ObjectResponse = uploadIndexHtmlFileToAwsS3bucket(bucketName, indexFile);
            S3ObjectResponse.Wait();
        }

        private async Task<PutObjectResponse> uploadIndexHtmlFileToAwsS3bucket(string bucketName, string indexFile)
        {
            logger.LogInformation($"Uploading { indexFile }' to { bucketName } bucket...");

            PutObjectRequest request = new PutObjectRequest
            {
                BucketName = bucketName,
                FilePath = indexFile,
                CannedACL = S3CannedACL.PublicRead
            };

            PutObjectResponse bucket = await TheS3Client().PutObjectAsync(request);
            return bucket;

        }

        private async Task<ListBucketsResponse> getBuckets(AmazonS3Client client)
        {
            var myList = await client.ListBucketsAsync();
            return myList;

        }

        private async Task<GetBucketWebsiteResponse> getBucketWebsite(AmazonS3Client client, GetBucketWebsiteRequest request)
        {
            GetBucketWebsiteResponse response = await client.GetBucketWebsiteAsync(request);
            return response;

        }

    }
}
