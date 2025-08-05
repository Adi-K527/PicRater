output "secret_name" {
  description = "The name of the Kubernetes secret created for ECR authentication"
  value       = kubernetes_secret.example.metadata[0].name
}