export KVSERVER_BASE_PATH=/home/azureuser/resilientdb
sudo rm -rf ./publish
sudo dotnet publish "ContractPortal.csproj" -c Release -o ./publish
cd ./publish
sudo dotnet ContractPortal.dll --urls http://*:80