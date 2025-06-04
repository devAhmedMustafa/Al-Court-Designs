using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

public class S3Service
{
    private readonly AmazonS3Client _s3Client;

    public S3Service(IConfiguration configuration)
    {
        var awsAccessKey = configuration["AWS:AccessKey"];
        var awsSecretKey = configuration["AWS:SecretKey"];
        var awsRegion = Amazon.RegionEndpoint.GetBySystemName(configuration["AWS:Region"]);

        var credentials = new BasicAWSCredentials(awsAccessKey, awsSecretKey);
        _s3Client = new AmazonS3Client(credentials, awsRegion);
    }

    public string GeneratePresignedUrl(string bucketName, string key, int durationInMinutes, HttpVerb httpVerb, string? contentType = null)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = bucketName,
            Key = key,
            Verb = httpVerb,
            Expires = DateTime.UtcNow.AddMinutes(durationInMinutes),
            ContentType = contentType
        }; 

        return _s3Client.GetPreSignedURL(request);
    }
}