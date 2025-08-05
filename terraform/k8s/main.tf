provider "kubernetes" {
  config_path = "${path.module}/../../kubeconfig.yaml"
  insecure    = true
}

module "user_service" {
  source = "./modules/deployment"
  app_name = "user-service"
  replicas = 2
  image    = "${var.account_id}.dkr.ecr.us-east-1.amazonaws.com/user-service-repo:${var.image_tag}"
  jwt_secret = var.jwt_secret
  db_connection = var.db_connection
}

module "pic_service" {
  source = "./modules/deployment"
  app_name = "pic-service"
  replicas = 1
  image    = "${var.account_id}.dkr.ecr.us-east-1.amazonaws.com/pic-service-repo:${var.image_tag}"
  jwt_secret = var.jwt_secret
  db_connection = var.db_connection
}

module "user_service_nodeport" {
  source = "./modules/service"
  app_name = "user-service"
  node_port = 30080
}

module "pic_service_nodeport" {
  source = "./modules/service"
  app_name = "pic-service"
  node_port = 30081
}