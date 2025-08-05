variable "name" {
    description = "The name of the instance"
    type = string
}

variable "access_key" {
  default = "your_access_key"
  description = "AWS Access Key"
}

variable "secret_key" {
  default = "your_secret_key"
  description = "AWS Secret Key"
}