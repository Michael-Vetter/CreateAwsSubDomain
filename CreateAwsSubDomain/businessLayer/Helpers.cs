using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace CreateAwsSubDomain.businessLayer
{
    public class DomainName
    {
        public string sub { get; set; }
        public string parent { get; set; }
        public string fullName { get; set; }
    }

    public static class Helpers
    {
        public static DomainName parseDomainName(string domainName)
        {
            // get rid of http://, https:// and a "/" and anything after it.
            string uri = Regex.Replace(domainName, "https*://", "");

            //check if "/" exists
            int slashLoc = uri.IndexOf('/');

            if (slashLoc >= 0)
                uri = uri.Substring(0, slashLoc);
            
            string[] parts = uri.Split('.');

            if (parts.Length <= 2)
                throw new System.FormatException("Invalid URL");

            string parentAndTopLevel = parts[parts.Length - 2] + "." + parts[parts.Length - 1];

            string subdomain = uri.Substring(0, uri.Length - parentAndTopLevel.Length - 1);

            return new DomainName
            {
                parent = parentAndTopLevel,
                sub = subdomain,
                fullName = uri
            };

        }

        public static string CreateIndexDotHtml(string TextToSayHelloTo)
        {
            try
            {
                string path = @"index.html";

                // Delete the file if it exists.
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                FileStream fs;
                // Create the file.
                using (fs = File.Create(path, 1024))
                {
                    Byte[] info = new UTF8Encoding(true).GetBytes("<html><h2>Hello " + TextToSayHelloTo + "!</h2></html>");
                    // Add some information to the file.
                    fs.Write(info, 0, info.Length);
                }

                return fs.Name;
            }
            catch
            {
                return string.Empty;
            }
            
        }
    }
}
