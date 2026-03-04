@echo off
echo ============================================
echo  Восстановление пакетов NuGet и сборка
echo ============================================

where dotnet >nul 2>nul
IF %ERRORLEVEL% NEQ 0 (
    echo [ОШИБКА] .NET SDK не найден!
    echo Скачайте с https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

echo [1/2] Восстановление пакетов NuGet...
dotnet restore BookshopApp.csproj
IF %ERRORLEVEL% NEQ 0 (
    echo [ОШИБКА] Ошибка при восстановлении пакетов.
    pause
    exit /b 1
)

echo [2/2] Сборка проекта...
dotnet build BookshopApp.csproj --configuration Debug
IF %ERRORLEVEL% NEQ 0 (
    echo [ОШИБКА] Ошибка при сборке.
    pause
    exit /b 1
)

echo.
echo [OK] Проект успешно собран.
echo Для запуска: dotnet run --project BookshopApp.csproj
pause
