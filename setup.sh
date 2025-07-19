#!/bin/bash

# Talabeyah Order Management System - Setup Script
# This script helps new users set up the project after cloning

echo "🚀 Setting up Talabeyah Order Management System..."

# Check if Node.js is installed
if ! command -v node &> /dev/null; then
    echo "❌ Node.js is not installed. Please install Node.js v18 or higher."
    echo "   Download from: https://nodejs.org/"
    exit 1
fi

# Check if Docker is installed
if ! command -v docker &> /dev/null; then
    echo "❌ Docker is not installed. Please install Docker Desktop."
    echo "   Download from: https://www.docker.com/products/docker-desktop/"
    exit 1
fi

# Check if Docker Compose is available
if ! docker-compose --version &> /dev/null; then
    echo "❌ Docker Compose is not available. Please ensure Docker Desktop is running."
    exit 1
fi

echo "✅ Prerequisites check passed!"

# Install frontend dependencies
echo "📦 Installing frontend dependencies..."
cd frontend
npm install
if [ $? -ne 0 ]; then
    echo "❌ Failed to install frontend dependencies"
    exit 1
fi
cd ..

echo "✅ Frontend dependencies installed!"

# Detect OS and choose appropriate Docker Compose file
if [[ "$OSTYPE" == "darwin"* ]]; then
    echo "🍎 Detected macOS - Using Azure SQL Edge configuration"
    COMPOSE_FILE="docker-compose.azure-sql-edge.yml"
else
    echo "🪟 Detected Windows/Linux - Using MS SQL Server configuration"
    COMPOSE_FILE="docker-compose.yml"
fi

# Start the application
echo "🐳 Starting application with Docker Compose..."
docker-compose -f $COMPOSE_FILE up --build -d

if [ $? -ne 0 ]; then
    echo "❌ Failed to start Docker containers"
    exit 1
fi

echo "✅ Application started successfully!"

# Wait a moment for services to be ready
echo "⏳ Waiting for services to be ready..."
sleep 10

echo ""
echo "🎉 Setup completed successfully!"
echo ""
echo "📱 Access your application:"
echo "   Frontend: http://localhost:4200"
echo "   Backend API: http://localhost:5001"
echo "   Swagger UI: http://localhost:5001/swagger"
echo ""
echo "🔧 Useful commands:"
echo "   View logs: docker-compose -f $COMPOSE_FILE logs"
echo "   Stop app: docker-compose -f $COMPOSE_FILE down"
echo "   Restart: docker-compose -f $COMPOSE_FILE restart"
echo ""
echo "📚 For more information, see README.md" 