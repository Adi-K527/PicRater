resource "kubernetes_persistent_volume" "volume" {
    metadata {
        name = "picrater-pv"
    }
    
    spec {
        persistent_volume_source {
            host_path {
                path = "/mnt/logs"
            }
        }
        capacity = {
            storage = "50Mi"
        }
        access_modes = ["ReadWriteOnce"]
        persistent_volume_reclaim_policy = "Retain"
        storage_class_name = "manual"
    }
}


resource "kubernetes_persistent_volume_claim" "volume_claim" {
    metadata {
        name = "picrater-pvc"
    }
    
    spec {
        access_modes = ["ReadWriteOnce"]
        resources {
            requests = {
                storage = "50Mi"
            }
        }
        storage_class_name = "manual"
    }
}