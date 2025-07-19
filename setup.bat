@echo off
REM Talabeyah Order Management System - Setup Script for Windows
REM This script helps new users set up the project after cloning

echo 🚀 Setting up Talabeyah Order Management System...

REM Check if Node.js is installed
node --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ Node.js is not installed. Please install Node.js v18 or higher.
    echo    Download from: https://nodejs.org/
    pause
    exit /b 1
)

REM Check if Docker is installed
docker --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ Docker is not installed. Please install Docker Desktop.
    echo    Download from: https://www.docker.com/products/docker-desktop/
    pause
    exit /b 1
)

REM Check if Docker Compose is available
docker-compose --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ Docker Compose is not available. Please ensure Docker Desktop is running.
    pause
    exit /b 1
)

echo ✅ Prerequisites check passed!

REM Install frontend dependencies
echo 📦 Installing frontend dependencies...
cd frontend
call npm install
if %errorlevel% neq 0 (
    echo ❌ Failed to install frontend dependencies
    pause
    exit /b 1
)
cd ..

echo ✅ Frontend dependencies installed!

REM Use MS SQL Server configuration for Windows
echo 🪟 Using MS SQL Server configuration for Windows
set COMPOSE_FILE=docker-compose.yml

REM Start the application
echo 🐳 Starting application with Docker Compose...
docker-compose -f %COMPOSE_FILE% up --build -d

if %errorlevel% neq 0 (
    echo ❌ Failed to start Docker containers
    pause
    exit /b 1
)

echo ✅ Application started successfully!

REM Wait a moment for services to be ready
echo ⏳ Waiting for services to be ready...
timeout /t 10 /nobreak >nul

echo.
echo 🎉 Setup completed successfully!
echo.
echo 📱 Access your application:
echo    Frontend: http://localhost:4200
echo    Backend API: http://localhost:5001
echo    Swagger UI: http://localhost:5001/swagger
echo.
echo 🔧 Useful commands:
echo    View logs: docker-compose -f %COMPOSE_FILE% logs
echo    Stop app: docker-compose -f %COMPOSE_FILE% down
echo    Restart: docker-compose -f %COMPOSE_FILE% restart
echo.
echo 📚 For more information, see README.md
pause 