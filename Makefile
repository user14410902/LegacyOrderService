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
run:
	dotnet run --project OrderService.ConsoleApp/OrderService.csproj
