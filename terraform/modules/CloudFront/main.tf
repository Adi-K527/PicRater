resource "aws_cloudfront_distribution" "cloudfront_s3_distribution" {
  origin {
    domain_name = var.website_endpoint
    origin_id   = var.bucket
    custom_origin_config {
      http_port              = "80"
      https_port             = "443"
      origin_protocol_policy = "http-only"
      origin_ssl_protocols   = ["TLSv1", "TLSv1.1", "TLSv1.2"]
    }
  }

  default_cache_behavior {
    cache_policy_id         = "4135ea2d-6df8-44a3-9df3-4b5a84be39ad"
    cached_methods          = ["GET", "HEAD"]
    allowed_methods         = ["GET", "HEAD"]
    target_origin_id        = var.bucket
    viewer_protocol_policy  = "allow-all"
  }

  restrictions {
    geo_restriction {
      restriction_type = "none"
    }
  }

  viewer_certificate {
    cloudfront_default_certificate = true
  }

  enabled         = true
  is_ipv6_enabled = true
  price_class     = "PriceClass_100"

  depends_on      = [ aws_s3_bucket_policy.s3_public_access ] 
}