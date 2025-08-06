resource "kubernetes_ingress_v1" "ingress" {
  metadata {
    name = "picrater-ingress"
  }

  spec {
    rule {
      http {
        path {
          path = "/user"
          path_type = "Prefix"

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
          path_type = "Prefix"

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