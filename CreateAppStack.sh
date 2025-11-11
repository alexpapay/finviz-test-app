#!/bin/bash

source ./docker-functions.sh

remove_container_and_image_if_exists "finviz-test-app-api" "finviz-test-app-api"
remove_container_and_image_if_exists "finviz-test-app-web" "finviz-test-app-web"

echo "Pull containers..."
docker-compose pull

echo "Build containers..."
docker-compose build --no-cache

echo "Start-Up containers..."
docker-compose up -d