resource "kubernetes_secret" "ecr_secret" {
  metadata {
    name = "ecr-secret"
  }

  type = "kubernetes.io/dockerconfigjson"

  data = {
    ".dockerconfigjson" = base64encode(jsonencode({
      auths = {
        "https://${var.account_id}.dkr.ecr.us-east-1.amazonaws.com" = {
          auth = base64encode("AWS:${var.token}")
        }
      }
    }))
  }
}
