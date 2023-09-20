﻿# Power Plant Production Plan API

This API calculates the production plan for a set of power plants based on load, fuel costs, and other parameters.

## Prerequisites

Before building and launching the API, make sure you have the following prerequisites installed:

- [.NET 6](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Your preferred code editor (e.g., Visual Studio Code, Visual Studio)]

## Getting Started

Follow these steps to build and launch the API:

1. Get the Repository:

2. Navigate to the API Directory:
```bash 
    cd PowerPlantProductionPlanAPI
```
3. Build the API:
```bash
	dotnet build
```
3. Run the API:
```bash
	dotnet run --project Challenge.Api
```

The API should now be running locally.

## API Endpoints
The API exposes the following endpoint:

- `POST /productionplan`: Calculates the production plan based on the provided payload.

## Usage
To use the API, send a POST request to the `/productionplan` endpoint with a payload in the request body.
The API will return a JSON response with the production plan.

## Docker (Optional)
To deploy the API using Docker, you can use the provided Dockerfile.
Make sure you have Docker installed, and then build and run the Docker image.
```bash
	# Build the Docker image
	docker build -t power-plant-api .

	# Run the Docker container
	docker run -d -p 8888:8888 power-plant-api
```
The API will be accessible on port 8888 within the Docker container.
