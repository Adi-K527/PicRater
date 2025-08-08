output "public_ip" {
  value = module.backend_server.public_ip
}

output "distribution" {
  value = module.pic_bucket.distribution
}