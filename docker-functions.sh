remove_container_and_image_if_exists() {
    local container_name=$1
    local image_name=$2

    if docker ps -a --format '{{.Names}}' | grep -q "^$container_name$"; then
        echo "Stopping and removing container: $container_name"

        docker stop "$container_name"
        docker rm "$container_name"
        
        echo "Removing image: $image_name"
        docker rmi "$image_name"
    else
        echo "Container $container_name does not exist, skipping."
    fi
}

remove_container_if_exists() {
    local container_name=$1

    if docker ps -a --format '{{.Names}}' | grep -q "^$container_name$"; then
        echo "Stopping and removing container: $container_name"

        docker stop "$container_name"
        docker rm "$container_name"
    else
        echo "Container $container_name does not exist, skipping."
    fi
}
