resource "kubernetes_horizontal_pod_autoscaler_v2" "auto_scaler" {
    metadata {
        name = "${var.app_name}-hpa"
        labels = {
            app = var.app_name
        }
    }
    
    spec {
        scale_target_ref {
            api_version = "apps/v1"
            kind        = "Deployment"
            name        = "${var.app_name}-deployment"
        }
    
        min_replicas = 1
        max_replicas = 5
    
        metric {
            type = "Resource"
        
            resource {
                name   = "cpu"
                target {
                    type  = "Utilization"
                    average_utilization = 50
                }
            }
        }
    }
}