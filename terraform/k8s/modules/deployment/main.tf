resource "kubernetes_deployment" "deployment" {
    metadata {
        name   = "${var.app_name}-deployment"
        labels = {
            app = var.app_name
        }
    }
    
    spec {
        replicas = var.replicas
    
        selector {
            match_labels = {
                app = var.app_name
            }
        }
    
        template {
            metadata {
                labels = {
                    app = var.app_name
                }
            }
    
            spec {
                container {
                    name  = "${var.app_name}-container"
                    image = var.image
            
                    port {
                        container_port = 80
                    }
                }

                image_pull_secrets {
                  name = module.ecr_secret.secret_name
                }
                
                restart_policy = "never"
            }
        }
    }
}