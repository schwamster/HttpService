docker build --build-arg BuildNumber=local -t httpservice-build .
docker create --name httpservice-build httpservice-build
docker cp httpservice-build:/package ./package
docker cp httpservice-build:/testresults/ ./testresults
docker rm httpservice-build