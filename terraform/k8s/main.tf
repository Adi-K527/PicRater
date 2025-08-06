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
    key     = "global/k3s/terraform.tfstate"
    region  = "us-east-1"
    encrypt = true
  }
}


provider "kubernetes" {
  config_path = "${path.module}/../../kubeconfig.yaml"
  insecure    = true
}

module "volume" {
  source = "./modules/volume"
}

module "user_service" {
  source = "./modules/deployment"
  app_name = "user-service"
  image    = "${var.account_id}.dkr.ecr.us-east-1.amazonaws.com/user-service-repo:${var.image_tag}"
  jwt_secret = var.jwt_secret
  db_connection = var.db_connection
  depends_on = [ module.volume ]
}

module "pic_service" {
  source = "./modules/deployment"
  app_name = "pic-service"
  image    = "${var.account_id}.dkr.ecr.us-east-1.amazonaws.com/pic-service-repo:${var.image_tag}"
  jwt_secret = var.jwt_secret
  db_connection = var.db_connection
  depends_on = [ module.volume ]
}

module "user_service_nodeport" {
  source = "./modules/service"
  app_name = "user-service"
  node_port = 30080
  depends_on = [ module.user_service ]
}

module "pic_service_nodeport" {
  source = "./modules/service"
  app_name = "pic-service"
  node_port = 30081
  depends_on = [ module.pic_service ]
}

module "user_service_hpa" {
  source = "./modules/horizontal-auto-scaler"
  app_name = "user-service"
  depends_on = [ module.user_service ]
}

module "pic_service_hpa" {
  source = "./modules/horizontal-auto-scaler"
  app_name = "pic-service"
  depends_on = [ module.pic_service ]
}