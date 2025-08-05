variable "image_tag" {
  description = "The tag of the Docker image to deploy"
  type        = string  
}

variable "account_id" {
  description = "The AWS account ID"
  type        = string
}

variable "token" {
  description = "The token for the Docker registry"
  type        = string
}

variable "db_connection" {
  description = "db connection string"
  type        = string
}

variable "jwt_secret" {
  description = "JWT secret"
  type        = string
}