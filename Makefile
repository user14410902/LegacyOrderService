build:
	dotnet build
clean:
	dotnet clean
	rm -rf OrderService.Data.Tests/bin/
	rm -rf OrderService.Data.Tests/obj/
	rm -rf OrderService.Services.Tests/bin/
	rm -rf OrderService.Services.Tests/obj/	
restore:
	dotnet restore
test:
	dotnet test
watch:
	dotnet watch --project OrderService.ConsoleApp/OrderService.csproj run
addOrder_Interactive:
	dotnet run --project OrderService.ConsoleApp/OrderService.csproj addOrderInteractive
addOrder_CommandLine:
	dotnet run --project OrderService.ConsoleApp/OrderService.csproj addOrder CustomerName Gadget 2
showAllOrders:
	dotnet run --project OrderService.ConsoleApp/OrderService.csproj showAllOrders
api:
	dotnet run --project OrderService.API/OrderService.API.csproj
api_watch:
	dotnet watch --project OrderService.API/OrderService.API.csproj
api_getAllOrders:
	curl -X GET http://localhost:8080/api/order/getall --verbose
apu_postOrder:
	curl -X POST --header "Accept: application/json" --header "Content-Type: application/json" --data '{"customerName":"Customer Name","productName":"Gadget","quantity": 5}' http://localhost:8080/api/order/create --verbose



