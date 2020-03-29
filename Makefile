.PHONY: dotnet/generate dotnet/test serve/basic-website

## Generate generated code
dotnet/generate:
	srgen -c Carbonfrost.Commons.Hxl.Integration.AspNetCore.Resources.SR \
		-r Carbonfrost.Commons.Hxl.Integration.AspNetCore.Automation.SR \
		--resx \
		dotnet/src/Carbonfrost.Commons.Hxl.Integration.AspNetCore/Automation/SR.properties

serve/basic-website:
	@ cd dotnet/test/WebSites/BasicWebsite; \
		dotnet run

## Execute dotnet unit tests
dotnet/test: dotnet/publish -dotnet/test

PUBLISH_DIR=dotnet/test/Carbonfrost.UnitTests.Hxl.Integration.AspNetCore/bin/$(CONFIGURATION)/netcoreapp3.0/publish
NUGET_DIR?=$(HOME)/.nuget/packages

-dotnet/test:
	fspec -i dotnet/test/Carbonfrost.UnitTests.Hxl/Content \
		$(NUGET_DIR)/System.Memory/4.5.2/lib/netstandard2.0/System.Memory.dll \
		$(NUGET_DIR)/System.Text.Encoding.CodePages/4.5.1/lib/netstandard2.0/System.Text.Encoding.CodePages.dll \
		$(NUGET_DIR)/System.Runtime.CompilerServices.Unsafe/4.5.2/lib/netstandard2.0/System.Runtime.CompilerServices.Unsafe.dll \
		$(NUGET_DIR)/System.Threading.Tasks.Extensions/4.5.2/lib/netstandard2.0/System.Threading.Tasks.Extensions.dll \
		$(PUBLISH_DIR)/Carbonfrost.UnitTests.Hxl.Integration.AspNetCore.dll

-include eng/.mk/*.mk
