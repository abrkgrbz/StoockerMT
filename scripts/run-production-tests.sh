#!/bin/bash

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${GREEN}=== StoockerMT Production Test Suite ===${NC}"

# 1. Environment Setup
echo -e "\n${YELLOW}1. Setting up production environment...${NC}"
docker-compose down -v
docker-compose up -d

# Wait for services to be ready
echo -e "${YELLOW}Waiting for services to be ready...${NC}"
sleep 30

# 2. Database Setup
echo -e "\n${YELLOW}2. Setting up databases...${NC}"
docker exec stoockermt_sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong@Password123 -Q "CREATE DATABASE StoockerMT_Master"
docker exec stoockermt_sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong@Password123 -Q "CREATE DATABASE StoockerMT_Tenant"

# 3. Run Migrations
echo -e "\n${YELLOW}3. Running migrations...${NC}"
dotnet ef database update --context MasterDbContext --project StoockerMT.Persistence
dotnet ef database update --context TenantDbContext --project StoockerMT.Persistence

# 4. Seed Test Data
echo -e "\n${YELLOW}4. Seeding test data...${NC}"
docker exec stoockermt_api dotnet StoockerMT.API.dll --seed

# 5. Create Test Tenants
echo -e "\n${YELLOW}5. Creating test tenants...${NC}"
for i in {1..5}
do
  curl -X POST http://localhost:5000/api/tenants/register \
    -H "Content-Type: application/json" \
    -d "{
      \"name\": \"Test Tenant $i\",
      \"code\": \"TENANT00$i\",
      \"adminEmail\": \"admin@tenant00$i.com\",
      \"adminPassword\": \"Admin123!\"
    }"
done

# 6. Health Check
echo -e "\n${YELLOW}6. Running health checks...${NC}"
curl -s http://localhost:5000/health | jq .

# 7. Performance Test
echo -e "\n${YELLOW}7. Running performance tests...${NC}"
docker run --rm -i --network=stoockermt_network \
  -v "$PWD/tests:/scripts" \
  grafana/k6 run /scripts/load-test.js

# 8. Stress Test
echo -e "\n${YELLOW}8. Running stress tests...${NC}"
docker run --rm -i --network=stoockermt_network \
  -v "$PWD/tests:/scripts" \
  grafana/k6 run --vus 500 --duration 5m /scripts/load-test.js

# 9. Chaos Testing
echo -e "\n${YELLOW}9. Running chaos tests...${NC}"
# Simulate network latency
docker exec stoockermt_api tc qdisc add dev eth0 root netem delay 100ms

# Test with network latency
curl -w "@curl-format.txt" -o /dev/null -s http://localhost:5000/api/customers

# Remove network latency
docker exec stoockermt_api tc qdisc del dev eth0 root

# 10. Security Testing
echo -e "\n${YELLOW}10. Running security tests...${NC}"
# SQL Injection test
curl -X GET "http://localhost:5000/api/customers?search='; DROP TABLE Customers;--"

# XSS test
curl -X POST http://localhost:5000/api/customers \
  -H "Content-Type: application/json" \
  -d '{"name": "<script>alert(\"XSS\")</script>"}'

# 11. Multi-tenant Isolation Test
echo -e "\n${YELLOW}11. Testing multi-tenant isolation...${NC}"
# Get token for Tenant 1
TOKEN1=$(curl -s -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -H "X-Tenant-ID: TENANT001" \
  -d '{"email": "admin@tenant001.com", "password": "Admin123!"}' | jq -r .token)

# Get token for Tenant 2
TOKEN2=$(curl -s -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -H "X-Tenant-ID: TENANT002" \
  -d '{"email": "admin@tenant002.com", "password": "Admin123!"}' | jq -r .token)

# Try to access Tenant 2 data with Tenant 1 token
curl -X GET http://localhost:5000/api/customers \
  -H "Authorization: Bearer $TOKEN1" \
  -H "X-Tenant-ID: TENANT002"

# 12. Backup Test
echo -e "\n${YELLOW}12. Testing backup functionality...${NC}"
curl -X POST http://localhost:5000/api/admin/tenants/1/backup \
  -H "Authorization: Bearer $TOKEN1"

# 13. Monitoring Check
echo -e "\n${YELLOW}13. Checking monitoring systems...${NC}"
echo "Prometheus: http://localhost:9090"
echo "Grafana: http://localhost:3000 (admin/admin)"
echo "Kibana: http://localhost:5601"
echo "Jaeger: http://localhost:16686"

# 14. Generate Report
echo -e "\n${YELLOW}14. Generating test report...${NC}"
docker logs stoockermt_api > logs/api.log
docker stats --no-stream > logs/docker-stats.log

echo -e "\n${GREEN}=== Production tests completed! ===${NC}"
echo -e "${YELLOW}Check the logs directory for detailed results.${NC}"