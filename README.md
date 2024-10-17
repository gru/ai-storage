# AI Storage Microservice

## Overview
AI Storage is a microservice designed to handle file storage operations using Amazon S3 or compatible storage services. It provides a simple API for uploading and downloading files, with metadata storage in a PostgreSQL database.

## Table of Contents
- [Features](#features)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
    - [Installation](#installation)
    - [Configuration](#configuration)
- [Usage](#usage)
- [API Documentation](#api-documentation)
- [Database](#database)
- [Testing](#testing)
- [Deployment](#deployment)
- [Monitoring and Logging](#monitoring-and-logging)
- [Contributing](#contributing)
- [License](#license)

## Features
- File upload to S3-compatible storage
- File download from S3-compatible storage
- Metadata storage in PostgreSQL database
- API versioning
- Swagger documentation
- Integration tests

## Prerequisites
- .NET 8 SDK
- PostgreSQL
- S3-compatible storage service (e.g., Amazon S3, MinIO)
- Docker and Docker Compose (for local development)

## Getting Started

### Installation
1. Clone the repository:
   ```
   git clone git@github.com:gru/ai-storage.git
   ```
2. Navigate to the project directory:
   ```
   cd AI.Storage
   ```
3. Restore dependencies:
   ```
   dotnet restore
   ```

### Configuration
1. Update the connection string in `appsettings.json` or use user secrets for local development.
2. Configure AWS S3 settings in `appsettings.json`:
   ```json
   "AWS": {
     "AccessKey": "your-access-key",
     "SecretKey": "your-secret-key",
     "ServiceURL": "https://s3.amazonaws.com",
     "BucketName": "your-bucket-name"
   }
   ```
3. For local development, you can use MinIO as an S3-compatible storage. Run the provided docker-compose file:
   ```
   docker-compose up -d
   ```

## Usage
To run the microservice:

```
dotnet run --project AI.Storage.Host.csproj
```

## API Documentation
The API documentation is available via Swagger UI when running the application in development mode. Access it at:

```
http://localhost:5000/swagger
```

## Database
This microservice uses Entity Framework Core with PostgreSQL. To set up the database:

1. Ensure PostgreSQL is installed and running.
2. Update the connection string in `appsettings.json`.
3. Run migrations:
   ```
   dotnet run --project AI.Storage.Migrations.csproj
   ```

## Testing
To run the tests:

```
dotnet test
```

The project includes integration tests that cover file upload and download operations.

## Deployment
[Provide instructions or links to deployment guides for various environments (staging, production, etc.)]

## Monitoring and Logging
This microservice uses Serilog for logging. Logs are written to both console and file.

## Contributing
[Include guidelines for contributing to the project, code of conduct, etc.]

## License
[Specify the license under which your microservice is released]