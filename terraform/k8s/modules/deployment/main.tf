locals {
  app_name_parts = split("-", var.app_name)
}


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
                        container_port = 8080
                    }

                    env {
                        name  = "Jwt__Secret"
                        value = var.jwt_secret
                    }

                    env {
                        name  = "Db__Connection"
                        value = "${var.db_connection}"
                    }

                    liveness_probe {
                        http_get {
                            path = "/api/${local.app_name_parts[0]}/health"
                            port = 8080
                        }
                        initial_delay_seconds = 30
                        period_seconds        = 10
                    }

                    volume_mount {
                        name       = "logs-volume"
                        mount_path = "/mnt/logs"
                    }
                }

                volume {
                    name = "logs-volume"

                    persistent_volume_claim {
                        claim_name = "picrater-pvc"
                    }
                }

                image_pull_secrets {
                  name = "ecr-secret"
                }
            }
        }
    }
}