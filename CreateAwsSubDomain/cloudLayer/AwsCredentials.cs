using System;
using System.Collections.Generic;
using System.Text;
using CreateAwsSubDomain.Interfaces;
using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;

namespace CreateAwsSubDomain.cloudLayer
{
    class AwsCredentials
    {

        private AWSCredentials awsCredentials;
        private ILogger logger;

        public AwsCredentials(ILogger logger)
        {
            this.logger = logger;
        }

        public AWSCredentials TheAwsCredentials()
        {
            if (awsCredentials == null)
            {
                try
                {
                    awsCredentials = LoadDefaultProfile();
                }
                catch (Exception ex)
                {
                    logger.LogError(string.Format("Failed to load profile. \n{0}\n{1}", ex.Message, ex.StackTrace));
                }
            }

            return awsCredentials;
        }

        // This will load the access key and secret key, which must be set up
        // in a credentials file. You will need to create a new user in AWS IAM with programatic 
        // access and permission to S3 and Route53
        private AWSCredentials LoadDefaultProfile()
        {
            CredentialProfile defaultProfile;
            AWSCredentials awsCredentials;

            logger.LogInformation("Loading the default AWS profile...");

            SharedCredentialsFile sharedFile = new SharedCredentialsFile();

            sharedFile.TryGetProfile("default", out defaultProfile);
            AWSCredentialsFactory.TryGetAWSCredentials(defaultProfile, sharedFile, out awsCredentials);
            
            logger.LogInformation("Profile loaded successfully");

            return awsCredentials;
        }
    }
}
