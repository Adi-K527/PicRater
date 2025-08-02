resource "aws_instance" "instance" {
  ami = "ami-0c02fb55956c7d316"
  instance_type = "t3.small"
  key_name = "terraform-key"
  vpc_security_group_ids = [aws_security_group.allow_ssh.id]
  subnet_id = aws_subnet.private_subnet.id
  associate_public_ip_address = true
  tags = {
    Name = var.name
  }
}