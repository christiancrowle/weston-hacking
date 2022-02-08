run-client: kill-weston start-weston
	dotnet run --project ImguiClient/ImguiClient.csproj
	$(MAKE) kill-weston

start-weston:
	(weston &)

kill-weston:
	-(kill -9 `pidof weston`)

build-lib:
	dotnet build