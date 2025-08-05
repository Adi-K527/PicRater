variable "app_name" {
  description = "The name of the application"
  type        = string
}

variable "replicas" {
  description = "The number of replicas for the deployment"
  type        = number
  default     = 1
}

variable "image" {
  description = "The Docker image to deploy"
  type        = string
}