provider "kubernetes" {
  config_path = "${path.module}/kubeconfig.yaml"
}

resource "kubernetes_deployment" "dummy" {
  metadata {
    name = "dummy-deployment"
    labels = {
      app = "dummy"
    }
  }

  spec {
    selector {
      match_labels = {
        app = "dummy"
      }
    }

    template {
      metadata {
        labels = {
          app = "dummy"
        }
      }

      spec {
        container {
          name  = "pause"
          image = "k8s.gcr.io/pause:3.9"
        }
      }
    }
  }
}
