# 🛒 Ecommerce Stacks - API Gateway Service

This project provides an API Gateway service for an ecommerce microservices architecture built with .NET 9.0 and modern technologies.

## 🏗️ Tech Stack

### API Gateway Service
- **.NET 9.0** - Latest .NET framework
- **YARP (Yet Another Reverse Proxy)** - High-performance reverse proxy
- **Serilog** - Structured logging framework
- **JWT Bearer Authentication** - Security layer
- **Health Checks** - Service monitoring
- **Swagger/OpenAPI** - API documentation

### Development & Deployment
- **GitHub Actions** - CI/CD pipeline with self-hosted runner
- **Systemd** - Linux service management
- **Docker** - Containerization support

## 🚀 Quick Start

### Prerequisites
- .NET 9.0 SDK
- Visual Studio 2022 or VS Code
- Docker Desktop (optional)

### 1. Clone & Setup
```bash
git clone <repository-url>
cd Ecommerce.Stacks
```

### 2. Environment Configuration
The gateway is configured to route to these services:
```yaml
# Core API Service
http://localhost:5504/

# Users API Service  
http://localhost:5502/
```

### 3. Run the Gateway
```bash
# Using .NET CLI
cd src/Ecom.Gateway.API
dotnet run

# Using Visual Studio
# Open Ecommerce.Stacks.sln and run the project

# Using Docker
docker build -t ecom-gateway .
docker run -p 5500:5500 ecom-gateway
```

### 4. Verify Service
```bash
# Check gateway health
curl http://localhost:5500/health

# Get gateway info
curl http://localhost:5500/api/gateway/info

# Access Swagger UI
# Open http://localhost:5500/swagger in browser
```

## 🔌 Service Endpoints

### Gateway Endpoints
```
http://localhost:5500          # API Gateway (HTTP)
http://localhost:5500/swagger  # API Documentation
http://localhost:5500/health   # Health Check
http://localhost:5500/api/gateway/info  # Gateway Information
```

### Proxy Routes
```yaml
# Core API Routes
/api/core/**  → http://localhost:5504/api/**

# Users API Routes  
/api/users/** → http://localhost:5502/api/**
```

## 🗂️ Project Structure

```
Ecommerce.Stacks/
├── .github/
│   └── workflows/
│       └── ci-cd.yml              # CI/CD Pipeline
├── src/
│   └── Ecom.Gateway.API/          # API Gateway (.NET 9.0)
│       ├── Controllers/
│       │   └── GatewayController.cs
│       ├── Properties/
│       │   └── launchSettings.json
│       ├── appsettings.json       # Production config
│       ├── appsettings.Development.json
│       ├── Program.cs             # Application entry point
│       ├── Ecom.Gateway.API.csproj
│       └── logs/                  # Application logs (auto-created)
├── Ecommerce.Stacks.sln          # .NET Solution file
└── README.md                     # This file
```

## 🚀 API Gateway Features

### Reverse Proxy Configuration
- **YARP Integration** - High-performance proxy with load balancing
- **Health Checks** - Active health monitoring for backend services
- **Path Transformation** - Intelligent routing and path rewriting
- **CORS Support** - Cross-origin resource sharing enabled

### Logging & Monitoring
- **Structured Logging** - JSON-based logs with Serilog
- **File & Console Logging** - Dual output for debugging and monitoring
- **Health Check Endpoints** - Built-in health monitoring
- **Request Tracing** - Detailed request/response logging

### Security
- **JWT Bearer Authentication** - Ready for token-based security
- **CORS Configuration** - Flexible cross-origin policies
- **HTTPS Support** - SSL/TLS termination capability

## 🔧 Configuration

### Environment-Specific Settings

**Development (appsettings.Development.json):**
```json
{
  "Kestrel": {
    "Endpoints": {
      "Http": {
        "Url": "http://+:5500"
      }
    }
  }
}
```

**Docker Configuration:**
```yaml
# Container ports
ASPNETCORE_HTTPS_PORTS: "8081"
ASPNETCORE_HTTP_PORTS: "8080"
```

### Adding New Routes
To add a new microservice route, update `appsettings.json`:

```json
{
  "ReverseProxy": {
    "Routes": {
      "new-service-route": {
        "ClusterId": "new-service-cluster",
        "Match": {
          "Path": "/api/newservice/{**catch-all}"
        },
        "Transforms": [
          { "PathPattern": "/api/{**catch-all}" }
        ]
      }
    },
    "Clusters": {
      "new-service-cluster": {
        "Destinations": {
          "destination1": {
            "Address": "http://localhost:5505/"
          }
        },
        "HealthCheck": {
          "Active": {
            "Enabled": "true",
            "Interval": "00:00:30",
            "Timeout": "00:00:05",
            "Policy": "ConsecutiveFailures",
            "Path": "/health"
          }
        }
      }
    }
  }
}
```

## 🚀 CI/CD Pipeline

The project includes a complete GitHub Actions pipeline for:

### Build & Test
- **Automated Builds** - .NET 9.0 compilation and testing
- **NuGet Caching** - Faster builds with dependency caching
- **Artifact Management** - Build output preservation

### Deployment
- **Self-Hosted Runner** - Direct deployment to development server
- **Systemd Integration** - Linux service management
- **Zero-Downtime Deployment** - Graceful service updates
- **Health Monitoring** - Post-deployment verification

### Service Management Commands
```bash
# Check service status
sudo systemctl status ecommerce-gateway

# View real-time logs
sudo journalctl -u ecommerce-gateway -f

# Restart service
sudo systemctl restart ecommerce-gateway

# Stop/Start service
sudo systemctl stop ecommerce-gateway
sudo systemctl start ecommerce-gateway
```

## 💡 Development Tips

### Local Development
1. **Hot Reload** - Use `dotnet watch run` for development
2. **Environment Variables** - Set `ASPNETCORE_ENVIRONMENT=Development`
3. **Logging** - Check `logs/gateway-.txt` for application logs
4. **Health Checks** - Monitor `/health` endpoint for service status

### Debugging
```bash
# Enable detailed logging
export ASPNETCORE_ENVIRONMENT=Development

# Check application logs
tail -f src/Ecom.Gateway.API/logs/gateway-*.txt

# Monitor health checks
curl -w "\n%{http_code}\n" http://localhost:5500/health
```

### Performance Monitoring
- Monitor proxy performance through YARP metrics
- Check backend service health via active health checks
- Use structured logs for request tracing

## 🐛 Troubleshooting

### Common Issues

**Port 5500 already in use:**
```bash
# Find process using port
netstat -ano | findstr :5500
# Kill process or change port in appsettings.json
```

**Backend services not responding:**
- Verify target services are running on configured ports
- Check health check logs in application logs
- Ensure firewall allows connections

**Logging issues:**
- Verify `logs/` directory permissions
- Check disk space for log files
- Review Serilog configuration in appsettings.json

### Health Check Failures
```bash
# Check individual backend health
curl http://localhost:5502/health  # Users service
curl http://localhost:5504/health  # Core service

# Check gateway routing
curl -v http://localhost:5500/api/users/test
curl -v http://localhost:5500/api/core/test
```

## 🔄 Service Integration

### Adding New Microservices
1. Update `appsettings.json` with new routes and clusters
2. Ensure the new service has a `/health` endpoint
3. Update the CI/CD pipeline if needed
4. Test routing with curl or Postman

### Service Discovery
The gateway currently uses static configuration. For dynamic service discovery:
- Consider integrating with Consul or etcd
- Implement service registry pattern
- Add automatic route configuration

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/new-feature`)
3. Make your changes
4. Test thoroughly (unit tests + integration tests)
5. Update documentation if needed
6. Submit a pull request

### Code Standards
- Follow .NET coding conventions
- Add XML documentation for public APIs
- Include unit tests for new features
- Update appsettings for configuration changes

## 📄 License

This project is licensed under the MIT License.

---

**Happy Coding! 🚀**

For issues or questions, please create an issue in the repository or contact the development team.

### Next Steps
- Add authentication middleware
- Implement rate limiting
- Add monitoring dashboard
- Create additional microservices
- Set up production deployment pipeline
4. **Health Monitoring**: Tất cả services có health checks
5. **Scalability**: Có thể scale từng service độc lập

## 🤝 Contributing

1. Fork the repository
2. Create feature branch
3. Make changes
4. Test thoroughly
5. Submit pull request

## 📄 License

This project is licensed under the MIT License.

---

**Happy Coding! 🚀**

Nếu gặp vấn đề gì, hãy tạo issue trong repository hoặc liên hệ team phát triển.
