resource "aws_instance" "instance" {
  ami = "ami-0c02fb55956c7d316"
  instance_type = "t3.small"
  key_name = "terraform-key"
  tags = {
    Name = var.name
  }
}
