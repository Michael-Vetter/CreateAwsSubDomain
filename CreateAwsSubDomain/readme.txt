CreateAwsSubDomain

This console application will do the following:
1) wait for user input of a subdomain to create.  The parent domain must be included and already exist in your AWS account. Example input: subdomain.parentdomain.com  (parentdomain.com must already be configured in Route53 on your account).
2) Create an S3 bucket
3) put a simple "hello" index.html file in the bucket
4) configure the S3 bucket as a website and set read permissions to everyone
5) Create the Route53 Hosted Zone for the subdomain
6) Create the Route53 Alias record pointing to the S3 bucket created earlier
7) Copy the Route53 NS record from the subdomain to the parent domain.

Notes:
1) you must have an AWS account and have set up an account with programmatic access.  This will provide you with an Access Key and a Secret Key.
2) The account needs the following permissions: AmazonS3FullAccess and AmazonRoute53FullAccess
3) The program loads the "default" credentials, which for me are stored in c:\Users\{username}\.aws in a file called credentials (no extension).  This file already existed on my machine.  I believe it was created when I installed the AWS .NET toolkit for visual studio (https://aws.amazon.com/visualstudio/).  I manually modified the file with the access key and secret key.  You should never hard code these values in your code.
For more information about configuring credentials: https://docs.aws.amazon.com/sdk-for-net/v2/developer-guide/net-dg-config-creds.html

4) This application is meant as an example of how to automate tasks in AWS using .NET (or .NET Core in my case).  This is not meant for production.  There are many ways this code can be improved and made more bullet proof.
