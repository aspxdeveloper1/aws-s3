using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.Auth;
using Amazon.S3.Model;
using System.IO;
using System.Collections.Specialized;
using System.Configuration;
using AWS_Demo.Models;

namespace AWS_Demo.Helper
{
    public sealed class S3Helper
    {

        private static readonly Lazy<S3Helper> lazy = new Lazy<S3Helper>(() => new S3Helper());
        IAmazonS3 client = new AmazonS3Client(RegionEndpoint.APSouth1);
        public static S3Helper Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        string bucketName = "aspx-demo-bucket";
        private S3Helper()
        {
        }

        #region simple upload



        public List<string> ListingBuckets()
        {
            try
            {
                ListBucketsResponse response = client.ListBuckets();
                return response.Buckets.Select(x => x.BucketName).ToList();
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                return new List<string>();
            }
        }

        public string CreateBucket(string s3BucketName)
        {
            try
            {
                PutBucketRequest request = new PutBucketRequest();
                request.CannedACL = S3CannedACL.PublicReadWrite;
                request.BucketName = s3BucketName;
                client.PutBucket(request);

                return "Success";
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                return amazonS3Exception.Message;
            }
        }

        public void SimpleUploadFile(HttpPostedFileBase file)
        {
            try
            {
                string s3ObjectName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                // simple object put
                PutObjectRequest request = new PutObjectRequest()
                {
                    CannedACL = S3CannedACL.PublicRead,
                    InputStream = file.InputStream,
                    BucketName = bucketName,
                    Key = s3ObjectName
                };
                request.Metadata.Add("original-file-name", file.FileName);
                PutObjectResponse response = client.PutObject(request);
            }
            catch (AmazonS3Exception amazonS3Exception)
            {

            }
        }

        public string GetFile(string s3ObjectName, string destinationPath)
        {
            try
            {
                GetObjectRequest request = new GetObjectRequest()
                {
                    BucketName = bucketName,
                    Key = s3ObjectName
                };

                using (GetObjectResponse response = client.GetObject(request))
                {
                    string orignalFileName = response.Metadata["original-file-name"];
                    if (!string.IsNullOrEmpty(orignalFileName))
                    {
                        s3ObjectName = orignalFileName;
                    }
                    if (!File.Exists(destinationPath))
                    {
                        response.WriteResponseStreamToFile(destinationPath + s3ObjectName);
                    }
                }
                return s3ObjectName;
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                return "";
            }
        }

        public void DeletingAnObject(string s3objectName)
        {
            try
            {
                DeleteObjectRequest request = new DeleteObjectRequest()
                {
                    BucketName = bucketName,
                    Key = s3objectName
                };

                client.DeleteObject(request);
            }
            catch (AmazonS3Exception amazonS3Exception)
            {

            }
        }

        public List<S3ObjectModel> ListingObjects()
        {
            try
            {
                ListObjectsRequest request = new ListObjectsRequest();
                request.BucketName = bucketName;
                ListObjectsResponse response = client.ListObjects(request);


                return response.S3Objects.Select(x => new S3ObjectModel { ObjectName = x.Key, ObjectSize = x.Size }).ToList();


                // list only things starting with "cfg"
                //request.Prefix = "cfg";
                //response = client.ListObjects(request);
                //return response.S3Objects.Select(x => new S3ObjectModel { ObjectName = x.Key, ObjectSize = x.Size }).ToList();


                // list only things that come after "cfg" alphabetically
                //request.Prefix = null;
                //request.Marker = "cfg";
                //response = client.ListObjects(request);
                //return response.S3Objects.Select(x => new S3ObjectModel { ObjectName = x.Key, ObjectSize = x.Size }).ToList();

                // only list 3 things
                //request.Prefix = null;
                //request.Marker = null;
                //request.MaxKeys = 3;
                //response = client.ListObjects(request);
                //return response.S3Objects.Select(x => new S3ObjectModel { ObjectName = x.Key, ObjectSize = x.Size }).ToList();
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                return new List<S3ObjectModel>();
            }
        }

        #endregion

        #region Transfer utility

        public void Upload(HttpPostedFileBase file, string s3ObjectName)
        {
            TransferUtility utility = new TransferUtility(client);

            TransferUtilityUploadRequest request = new TransferUtilityUploadRequest
            {
                BucketName = bucketName,
                Key = s3ObjectName,
                //FilePath = file,// file path 
                CannedACL= S3CannedACL.PublicRead,
                InputStream= file.InputStream 
            };


            utility.Upload(request);
        }

        public void UploadDirectory(string uploadDirectory)
        {
            try
            {

                client.PutBucket(new PutBucketRequest() { BucketName = bucketName });

                TransferUtility transferUtility = new TransferUtility(RegionEndpoint.APSouth1);
                TransferUtilityUploadDirectoryRequest request = new TransferUtilityUploadDirectoryRequest()
                {
                    BucketName = bucketName,
                    Directory = uploadDirectory,
                    SearchOption = SearchOption.AllDirectories
                };
                transferUtility.UploadDirectory(request);
            }
            catch
            {

            }
        }

        #endregion
    }
}