provider "kubernetes" {
  config_path = "${path.module}/../../kubeconfig.yaml"
  insecure    = true
}

module "ecr_secret" {
  source     = "./modules/secret"
  account_id = var.account_id
  token      = var.token
}

module "user_service" {
  source = "./modules/deployment"
  app_name = "user-service"
  replicas = 2
  image    = "${var.account_id}.dkr.ecr.us-east-1.amazonaws.com/user-service-repo:${var.image_tag}"
  depends_on = [ module.ecr_secret ]
}

module "pic_service" {
  source = "./modules/deployment"
  app_name = "pic-service"
  replicas = 1
  image    = "${var.account_id}.dkr.ecr.us-east-1.amazonaws.com/pic-service-repo:${var.image_tag}"
  depends_on = [ module.ecr_secret ]
}