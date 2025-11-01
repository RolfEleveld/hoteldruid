#!/bin/bash
# Test Docker Setup Script

echo "🧪 Testing HotelDruid Docker Setup..."
echo "===================================="

# Check if containers are running
echo "1. Checking container status..."
docker-compose ps

echo ""
echo "2. Testing database connection..."
echo "Waiting for database to be ready..."
sleep 5

# Test database connection
docker-compose exec -T hoteldruid-db mysql -u hoteldruid_user -photeldruid_pass_2024 hoteldruid -e "SELECT 'Database connection successful!' as test_result;"

if [ $? -eq 0 ]; then
    echo "✅ Database connection test passed!"
else
    echo "❌ Database connection test failed!"
    exit 1
fi

echo ""
echo "3. Testing web server..."
echo "Checking if Apache is responding..."

# Test web server (wait a bit for it to start)
sleep 5
response=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:8080)

if [ "$response" = "200" ]; then
    echo "✅ Web server is responding (HTTP $response)"
elif [ "$response" = "000" ]; then
    echo "⚠️  Web server is not accessible. Check if containers are running."
    echo "   Try: docker-compose logs hoteldruid-web"
else
    echo "⚠️  Web server responded with HTTP $response"
fi

echo ""
echo "4. Testing phpMyAdmin..."
response=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:8081)

if [ "$response" = "200" ]; then
    echo "✅ phpMyAdmin is responding (HTTP $response)"
else
    echo "⚠️  phpMyAdmin responded with HTTP $response or is not accessible"
fi

echo ""
echo "5. Showing container logs (last 10 lines)..."
echo "--- HotelDruid Web Server Logs ---"
docker-compose logs --tail=10 hoteldruid-web

echo ""
echo "--- Database Logs ---"
docker-compose logs --tail=10 hoteldruid-db

echo ""
echo "🎯 Test Summary:"
echo "   HotelDruid:  http://localhost:8080"
echo "   phpMyAdmin:  http://localhost:8081"
echo ""
echo "📝 To view full logs: docker-compose logs -f [service-name]"
echo "🛑 To stop: docker-compose down"