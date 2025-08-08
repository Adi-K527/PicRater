terraform {
  required_version = ">= 1.0.0, < 2.0.0"

  required_providers {
    aws = {
        source  = "hashicorp/aws"
        version = "~>5.0"
    }
  }
  backend "s3" {
    bucket  = "picrater-state-bucket-1615"
    key     = "global/s3/terraform.tfstate"
    region  = "us-east-1"
    encrypt = true
  }
}

provider "aws" {
  region     = "us-east-1"
  access_key = var.access_key
  secret_key = var.secret_access_key
}

resource "aws_ecr_repository" "user_service_repo" {
  name = "user-service-repo"
}

resource "aws_ecr_repository" "pic_service_repo" {
  name = "pic-service-repo"
}

module "frontend_bucket" {
  source     = "./modules/S3"
  bucket_name = "picrater-frontend-8164"
}

module "pic_bucket" {
  source     = "./modules/S3-CloudFront"
  bucket_name = "picrater-pics-8164"
}

module "backend_server" {
  source = "./modules/EC2"
  name = "backend-server"
  access_key = var.access_key
  secret_key = var.secret_access_key
}