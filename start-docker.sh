#!/bin/bash
# HotelDruid Docker Startup Script

echo "🏨 Starting HotelDruid LAMP Stack..."

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    echo "❌ Docker is not running. Please start Docker first."
    exit 1
fi

# Check if .env file exists, if not copy from example
if [ ! -f .env ]; then
    echo "📋 Creating .env file from example..."
    cp .env.example .env
    echo "✅ .env file created. Please review and modify as needed."
fi

# Create logs directory if it doesn't exist
mkdir -p logs/apache

# Build and start the containers
echo "🔨 Building HotelDruid container..."
docker-compose build

echo "🚀 Starting services..."
docker-compose up -d

# Wait a moment for services to start
sleep 10

# Check if services are running
echo "🔍 Checking service status..."
docker-compose ps

# Show connection information
echo ""
echo "🎉 HotelDruid LAMP Stack is starting up!"
echo ""
echo "📱 Application URLs:"
echo "   HotelDruid:  http://localhost:8080"
echo "   phpMyAdmin:  http://localhost:8081"
echo ""
echo "🗄️  Database Connection:"
echo "   Host: localhost:3306"
echo "   Database: hoteldruid"
echo "   Username: hoteldruid_user"
echo "   Password: hoteldruid_pass_2024"
echo ""
echo "📊 Container Status:"
docker-compose ps --format "table {{.Name}}\t{{.Status}}\t{{.Ports}}"
echo ""
echo "📝 To view logs: docker-compose logs -f"
echo "🛑 To stop: docker-compose down"
echo "🔄 To restart: docker-compose restart"
echo ""
echo "⏳ Please wait a few moments for all services to fully initialize..."