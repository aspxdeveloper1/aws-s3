using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AWS_Demo.Models
{
    public class S3ObjectModel
    {
        public string ObjectName { get; set; }
        public long ObjectSize { get; set; }
    }

    public class S3BucketModel
    {
        [Required(ErrorMessage = "Please bucket name"), MaxLength(30)]
        [RegularExpression(@"[a-z]{3,30}$", ErrorMessage = "Only lowercase Characters are allowed.")]
        public string BucketName { get; set; }
    }
}