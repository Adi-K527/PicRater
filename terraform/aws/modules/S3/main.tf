resource "aws_s3_bucket" "s3_bucket" {
    bucket = var.bucket_name
}

resource "aws_s3_bucket_versioning" "s3_bucket_versioning" {
    bucket = aws_s3_bucket.s3_bucket.id

    versioning_configuration {
        status = "Enabled"
    }
}

# Public access block - allow public read access
resource "aws_s3_bucket_public_access_block" "s3_bucket_public_access" {
    bucket = aws_s3_bucket.s3_bucket.id

    block_public_acls       = false
    block_public_policy     = false
    ignore_public_acls      = false
    restrict_public_buckets = false
}

# Bucket policy to allow public read access
resource "aws_s3_bucket_policy" "s3_bucket_policy" {
    bucket = aws_s3_bucket.s3_bucket.id

    policy = jsonencode({
        Version = "2012-10-17"
        Statement = [
            {
                Sid       = "PublicReadGetObject"
                Effect    = "Allow"
                Principal = "*"
                Action    = "s3:GetObject"
                Resource  = "${aws_s3_bucket.s3_bucket.arn}/*"
            }
        ]
    })

    depends_on = [aws_s3_bucket_public_access_block.s3_bucket_public_access]
}

# CORS configuration to allow images to be displayed from frontend
resource "aws_s3_bucket_cors_configuration" "s3_bucket_cors" {
    bucket = aws_s3_bucket.s3_bucket.id

    cors_rule {
        allowed_headers = ["*"]
        allowed_methods = ["GET", "PUT", "POST", "DELETE"]
        allowed_origins = ["*"]
        expose_headers  = ["ETag"]
        max_age_seconds = 3000
    }
}

resource "aws_s3_bucket_website_configuration" "s3_static_hosting" {
  bucket = aws_s3_bucket.s3_bucket.id

  index_document {
    suffix = "index.html"
  }
}