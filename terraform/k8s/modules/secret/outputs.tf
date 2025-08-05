output "secret_name" {
  description = "The name of the Kubernetes secret created for ECR authentication"
  value       = kubernetes_secret.ecr_secret.metadata[0].name
}