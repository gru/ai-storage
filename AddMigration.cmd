@echo off
setlocal enabledelayedexpansion

:: Prompt for variables
set /p migrationName=Enter the migration name: 
set projectPath=Src/AI.Storage.Entities/AI.Storage.Entities.csproj
set startupProjectPath=Src/AI.Storage.Migrations/AI.Storage.Migrations.csproj
set context=AI.Storage.Entities.StorageDbContext

:: Create the migration
set command=dotnet ef migrations add %migrationName% --project "%projectPath%" --startup-project "%startupProjectPath%" --context "%context%"

:: Output the command (optional, for verification)
echo Executing command: %command%

:: Execute the command
%command%

:: Check if the command was successful
if %ERRORLEVEL% equ 0 (
    echo Migration '%migrationName%' created successfully.
) else (
    echo Failed to create migration '%migrationName%'. Please check the error messages above.
)

endlocal