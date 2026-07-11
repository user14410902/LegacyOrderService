build:
	dotnet build
clean:
	dotnet clean
	rm -rf OrderService.Data.Tests/bin/
	rm -rf OrderService.Data.Tests/obj/
restore:
	dotnet restore
test:
	dotnet test
watch:
	dotnet watch --project OrderService.ConsoleApp/OrderService.csproj run
run:
	dotnet run --project OrderService.ConsoleApp/OrderService.csproj
