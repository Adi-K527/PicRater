resource "aws_security_group" "allow_ssh" {
    name = "allow_ssh"
    description = "Allow SSH traffic"
    vpc_id = "vpc-0af49312582e83e49"
    ingress {
        from_port = 22
        to_port = 22
        protocol = "tcp"
        cidr_blocks = ["0.0.0.0/0"]
    }
    egress {
        from_port = 0
        to_port = 0
        protocol = "-1"
        cidr_blocks = ["0.0.0.0/0"]
    }
}

resource "aws_instance" "instance" {
  ami = "ami-0c02fb55956c7d316"
  instance_type = "t3.small"
  key_name = "terraform-key"
  subnet_id = "subnet-0d98153aba45505e1"
  vpc_security_group_ids = [aws_security_group.allow_ssh.id]

  associate_public_ip_address = true

  user_data = <<-EOF
    #!/bin/bash
    sudo yum update -y
    sudo yum install -y docker

    sudo amazon-linux-extras enable selinux-ng
    sudo yum clean metadata
    sudo yum install -y selinux-policy-targeted

    curl -sfL https://get.k3s.io | sh -

    sudo chmod 777 /etc/rancher/k3s/k3s.yaml
  EOF
  tags = {
    Name = var.name
  }
}
