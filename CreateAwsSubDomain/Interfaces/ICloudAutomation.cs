using System;
using System.Collections.Generic;
using System.Text;

namespace CreateAwsSubDomain.Interfaces
{
    public interface ICloudAutomation
    {
        bool CreateS3WebsiteBucketWithIndexFile(string bucketName, string IndexFile);
        bool CreateHostedZone(string zoneName);
        bool CreateParentNsRecord(string fullName, string parentName);

    }
}
