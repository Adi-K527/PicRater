resource "kubernetes_ingress_v1" "ingress" {
  metadata {
    name = "picrater-ingress"
  }

  spec {
    rule {
      http {
        path {
          path = "/user"

          backend {
            service {
                name = "user-service-service"
                port {
                    number = 80
                }
            }
          }
        }

        path {
          path = "/pic"

          backend {
            service {
                name = "pic-service-service"
                port {
                    number = 80
                }
            }
          }
        }
      }
    }
  }
}