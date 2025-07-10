import http from 'k6/http';
import { check, sleep } from 'k6';
import { Rate } from 'k6/metrics';

// Custom metrics
const errorRate = new Rate('errors');
const successRate = new Rate('success');

// Test configuration
export const options = {
  stages: [
    { duration: '2m', target: 10 },   // Ramp up to 10 users
    { duration: '5m', target: 50 },   // Ramp up to 50 users
    { duration: '10m', target: 100 }, // Ramp up to 100 users
    { duration: '5m', target: 200 },  // Ramp up to 200 users
    { duration: '10m', target: 200 }, // Stay at 200 users
    { duration: '5m', target: 0 },    // Ramp down to 0 users
  ],
  thresholds: {
    http_req_duration: ['p(95)<500'], // 95% of requests must complete below 500ms
    http_req_failed: ['rate<0.1'],    // Error rate must be below 10%
    errors: ['rate<0.1'],             // Custom error rate below 10%
  },
};

const BASE_URL = 'http://localhost:5000/api';
const TENANTS = ['TENANT001', 'TENANT002', 'TENANT003', 'TENANT004', 'TENANT005'];

// Helper function to get random tenant
function getRandomTenant() {
  return TENANTS[Math.floor(Math.random() * TENANTS.length)];
}

// Setup function - login and get tokens
export function setup() {
  const tokens = {};
  
  TENANTS.forEach(tenant => {
    const loginRes = http.post(`${BASE_URL}/auth/login`, JSON.stringify({
      email: `admin@${tenant.toLowerCase()}.com`,
      password: 'Admin123!',
      tenantCode: tenant
    }), {
      headers: { 
        'Content-Type': 'application/json',
        'X-Tenant-ID': tenant
      }
    });
    
    if (loginRes.status === 200) {
      tokens[tenant] = JSON.parse(loginRes.body).token;
    }
  });
  
  return tokens;
}

export default function (tokens) {
  const tenant = getRandomTenant();
  const token = tokens[tenant];
  
  const params = {
    headers: {
      'Authorization': `Bearer ${token}`,
      'X-Tenant-ID': tenant,
      'Content-Type': 'application/json',
    },
  };

  // Scenario 1: Health Check
  const healthRes = http.get(`${BASE_URL.replace('/api', '')}/health`);
  check(healthRes, {
    'health check status is 200': (r) => r.status === 200,
  });

  // Scenario 2: Get Customers
  const customersRes = http.get(`${BASE_URL}/customers?page=1&size=20`, params);
  check(customersRes, {
    'get customers status is 200': (r) => r.status === 200,
    'get customers response time < 500ms': (r) => r.timings.duration < 500,
  });
  errorRate.add(customersRes.status !== 200);
  successRate.add(customersRes.status === 200);

  sleep(1);

  // Scenario 3: Get Products
  const productsRes = http.get(`${BASE_URL}/products?page=1&size=20`, params);
  check(productsRes, {
    'get products status is 200': (r) => r.status === 200,
    'get products response time < 500ms': (r) => r.timings.duration < 500,
  });
  errorRate.add(productsRes.status !== 200);
  successRate.add(productsRes.status === 200);

  sleep(1);

  // Scenario 4: Create Order
  const orderData = {
    customerId: Math.floor(Math.random() * 100) + 1,
    orderDate: new Date().toISOString(),
    items: [
      {
        productId: Math.floor(Math.random() * 50) + 1,
        quantity: Math.floor(Math.random() * 5) + 1,
        unitPrice: 99.99
      }
    ]
  };

  const createOrderRes = http.post(`${BASE_URL}/orders`, JSON.stringify(orderData), params);
  check(createOrderRes, {
    'create order status is 201': (r) => r.status === 201,
    'create order response time < 1000ms': (r) => r.timings.duration < 1000,
  });
  errorRate.add(createOrderRes.status !== 201);
  successRate.add(createOrderRes.status === 201);

  sleep(2);

  // Scenario 5: Complex Search
  const searchRes = http.get(`${BASE_URL}/products/search?q=test&minPrice=10&maxPrice=1000&inStock=true`, params);
  check(searchRes, {
    'search status is 200': (r) => r.status === 200,
    'search response time < 1000ms': (r) => r.timings.duration < 1000,
  });
  errorRate.add(searchRes.status !== 200);
  successRate.add(searchRes.status === 200);

  sleep(1);
}

// Teardown function
export function teardown(data) {
  console.log('Test completed!');
}