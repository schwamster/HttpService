# docker-compose -f docker-compose-build.yml up

docker build --build-arg BuildNumber=local -t httpservice-build .
docker create --name httpservice-build httpservice-build
docker cp httpservice-build:/package ./package
docker rm httpservice-build