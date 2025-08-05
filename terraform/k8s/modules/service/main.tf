resource "kubernetes_service" "service" {
  metadata {
    name = "${var.app_name}-service"
    labels = {
      app = var.app_name
    }
  }

  spec {
    selector = {
      app = var.app_name
    }

    port {
      port        = 80
      target_port = 8080
      node_port   = var.node_port
    }

    type = "NodePort"
  }
  
}