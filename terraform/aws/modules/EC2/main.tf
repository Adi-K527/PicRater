resource "aws_security_group" "allow_ssh" {
    name = "allow_ssh"
    description = "Allow SSH traffic"
    vpc_id = "vpc-0af49312582e83e49"
    ingress {
        from_port   = 22
        to_port     = 22
        protocol    = "tcp"
        cidr_blocks = ["0.0.0.0/0"]
    }

    ingress {
        from_port   = 6443
        to_port     = 6443
        protocol    = "tcp"
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
  instance_type = "t3.micro"
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

    aws ecr get-login-password --region us-east-1 | docker login \
    --username AWS \
    --password-stdin 206479108282.dkr.ecr.us-east-1.amazonaws.com

    aws configure set aws_access_key_id ${var.access_key} && \
    aws configure set aws_secret_access_key ${var.secret_key} && \
    aws configure set region us-east-1 && \
    aws configure set output json

    ACCOUNT=$(aws sts get-caller-identity --query 'Account' --output text)
    REGION=us-east-1
    SECRET_NAME=ecr-secret
    EMAIL=abc@xyz.com
    TOKEN=$(aws ecr --region=$REGION get-authorization-token --output text --query authorizationData[].authorizationToken | base64 -d | cut -d: -f2)
                    
    kubectl create secret docker-registry $SECRET_NAME \
      --docker-server=https://$ACCOUNT.dkr.ecr.us-east-1.amazonaws.com \
      --docker-username=AWS \
      --docker-password="${TOKEN}" \
      --docker-email="${EMAIL}"

  EOF
  tags = {
    Name = var.name
  }
}


