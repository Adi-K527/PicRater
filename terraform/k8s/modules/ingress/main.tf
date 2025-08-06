resource "kubernetes_ingress" "ingress" {
  metadata {
    name = "picrater-ingress"
  }

  spec {
    rule {
      http {
        path {
          path = "/user"

          backend {
            service_name = "user-service-service"
            service_port = 80
          }
        }

        path {
          path = "/pic"

          backend {
            service_name = "pic-service-service"
            service_port = 80
          }
        }
      }
    }
  }
}