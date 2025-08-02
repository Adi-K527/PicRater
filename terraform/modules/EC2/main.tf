data "aws_vpc" "default" {
    default = true
}

data "aws_subnet" "default" {
    vpc_id = data.aws_vpc.default.id
}

resource "aws_security_group" "allow_ssh" {
    name = "allow_ssh"
    description = "Allow SSH traffic"
    vpc_id = data.aws_vpc.default.id
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
  subnet_id = data.aws_subnet.default.id
  vpc_security_group_ids = [aws_security_group.allow_ssh.id]

  associate_public_ip_address = true
  tags = {
    Name = var.name
  }
}
