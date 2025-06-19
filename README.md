# 🛒 Ecommerce Stacks - Microservices Infrastructure

Dự án này cung cấp một stack đầy đủ cho hệ thống ecommerce với các công nghệ hiện đại sử dụng Docker Compose.

## 🏗️ Tech Stack

### Core Services
- **PostgreSQL** - Relational Database (latest)
- **MongoDB** - NoSQL Database (latest)  
- **Redis** - In-Memory Cache & Session Store (latest)
- **Apache Kafka** - Message Broker & Event Streaming (latest)
- **LocalStack** - AWS Services Emulator (latest)

### Management Tools
- **Adminer** - PostgreSQL Database Management
- **Mongo Express** - MongoDB Management Interface
- **Redis Commander** - Redis Management Tool
- **Kafka UI** - Kafka Cluster Management

### Supporting Services
- **Zookeeper** - Kafka Coordination Service

## 🚀 Quick Start

### Prerequisites
- Docker Desktop
- Docker Compose V2

### 1. Clone & Setup
```bash
git clone <repository-url>
cd Ecommerce.Stacks
```

### 2. Environment Configuration
Cấu hình các biến môi trường trong file `.env`:
```env
# PostgreSQL
POSTGRES_DB=ecommerce_db
POSTGRES_USER=ecommerce_user
POSTGRES_PASSWORD=ecommerce_password

# MongoDB
MONGO_INITDB_ROOT_USERNAME=admin
MONGO_INITDB_ROOT_PASSWORD=admin_password

# Redis
REDIS_PASSWORD=redis_password

# Kafka
KAFKA_PORT=9092

# LocalStack
LOCALSTACK_SERVICES=s3,sqs,sns,dynamodb,lambda,apigateway
```

### 3. Start Services
```bash
# Start tất cả services
docker-compose up -d

# Hoặc start từng service cụ thể
docker-compose up -d postgres mongodb redis
```

### 4. Verify Services
```bash
# Kiểm tra trạng thái các container
docker-compose ps

# Xem logs
docker-compose logs -f [service-name]
```

## 📊 Management Interfaces

Sau khi start thành công, bạn có thể truy cập các giao diện quản lý:

| Service | URL | Credentials |
|---------|-----|-------------|
| **Adminer** (PostgreSQL) | http://localhost:8081 | Server: `postgres`, User: `ecommerce_user` |
| **Mongo Express** | http://localhost:8082 | User: `admin`, Pass: `admin_password` |
| **Redis Commander** | http://localhost:8083 | Password: `redis_password` |
| **Kafka UI** | http://localhost:8080 | No auth required |
| **LocalStack** | http://localhost:4566 | AWS CLI endpoint |

## 🔌 Service Endpoints

### Database Connections
```yaml
# PostgreSQL
Host: localhost
Port: 5432
Database: ecommerce_db
Username: ecommerce_user
Password: ecommerce_password

# MongoDB
Connection String: mongodb://admin:admin_password@localhost:27017/ecommerce_mongo

# Redis
Host: localhost
Port: 6379
Password: redis_password
```

### Message Broker
```yaml
# Kafka Bootstrap Server
localhost:9092

# Zookeeper
localhost:2181
```

### AWS Services (LocalStack)
```bash
# AWS CLI Configuration
aws configure set aws_access_key_id test
aws configure set aws_secret_access_key test
aws configure set region us-east-1
aws configure set output json

# Endpoint URL
--endpoint-url=http://localhost:4566
```

## 🗂️ Project Structure

```
Ecommerce.Stacks/
├── docker-compose.yml     # Service definitions
├── .env                   # Environment variables
├── README.md             # Documentation
└── data/                 # Persistent data (auto-created)
    ├── postgres/
    ├── mongo/
    ├── redis/
    ├── kafka/
    └── localstack/
```

## 💡 Usage Examples

### PostgreSQL
```sql
-- Connect và tạo bảng mẫu
CREATE TABLE products (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    price DECIMAL(10,2),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

### MongoDB
```javascript
// Connect và tạo collection
use ecommerce_mongo
db.users.insertOne({
    name: "John Doe",
    email: "john@example.com",
    createdAt: new Date()
})
```

### Redis
```bash
# Test Redis connection
redis-cli -h localhost -p 6379 -a redis_password
> SET user:1 "John Doe"
> GET user:1
```

### Kafka
```bash
# Tạo topic
docker exec ecommerce-kafka kafka-topics --create --topic orders --bootstrap-server localhost:9092

# Producer
docker exec -it ecommerce-kafka kafka-console-producer --topic orders --bootstrap-server localhost:9092

# Consumer
docker exec -it ecommerce-kafka kafka-console-consumer --topic orders --from-beginning --bootstrap-server localhost:9092
```

### LocalStack (AWS S3)
```bash
# Tạo S3 bucket
aws --endpoint-url=http://localhost:4566 s3 mb s3://ecommerce-bucket

# Upload file
aws --endpoint-url=http://localhost:4566 s3 cp file.txt s3://ecommerce-bucket/

# List objects
aws --endpoint-url=http://localhost:4566 s3 ls s3://ecommerce-bucket/
```

## 🔧 Common Commands

### Service Management
```bash
# Start specific services
docker-compose up -d postgres redis

# Stop all services
docker-compose down

# Stop and remove volumes
docker-compose down -v

# Restart service
docker-compose restart kafka

# View logs
docker-compose logs -f postgres
```

### Health Checks
```bash
# Check all containers status
docker-compose ps

# Check specific service health
docker-compose exec postgres pg_isready
docker-compose exec mongodb mongosh --eval "db.adminCommand('ping')"
docker-compose exec redis redis-cli ping
```

### Data Management
```bash
# Backup PostgreSQL
docker-compose exec postgres pg_dump -U ecommerce_user ecommerce_db > backup.sql

# Backup MongoDB
docker-compose exec mongodb mongodump --uri="mongodb://admin:admin_password@localhost:27017/ecommerce_mongo"

# Clear all data (⚠️ DESTRUCTIVE)
docker-compose down -v
```

## 🐛 Troubleshooting

### Common Issues

**Port conflicts:**
```bash
# Check port usage
netstat -ano | findstr :5432
```

**Memory issues:**
```bash
# Increase Docker memory limit in Docker Desktop settings
# Recommended: 4GB+ RAM
```

**Permission issues:**
```bash
# On Windows, ensure Docker Desktop is running as Administrator
# On Linux, add user to docker group
sudo usermod -aG docker $USER
```

### Service-specific Issues

**PostgreSQL connection failed:**
- Verify credentials in `.env`
- Check if port 5432 is available
- Wait for health check to pass

**Kafka not starting:**
- Ensure Zookeeper is running first
- Check if ports 9092/2181 are available
- Increase memory allocation

**LocalStack issues:**
- Verify Docker socket mounting
- Check LocalStack logs for service startup
- Ensure required AWS CLI version

## 📝 Development Tips

1. **Environment Isolation**: Mỗi service chạy trong container riêng biệt
2. **Data Persistence**: Tất cả data được lưu trong Docker volumes
3. **Service Discovery**: Các service có thể communicate qua container names
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
