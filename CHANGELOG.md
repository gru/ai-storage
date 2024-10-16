# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.1.0] - 2024-10-16

### Added
- Initial release of the AI Storage Microservice
- Implemented file upload functionality to S3-compatible storage
- Implemented file download functionality from S3-compatible storage
- Created ContentEntity for storing file metadata in PostgreSQL database
- Set up API versioning
- Implemented Swagger documentation
- Added integration tests for file upload and download operations
- Configured Serilog for logging
- Set up Docker Compose file for local development with MinIO
- Implemented error handling and ProblemDetails responses
- Created database migration project