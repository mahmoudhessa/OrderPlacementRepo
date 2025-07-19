#!/bin/bash

# Database initialization script
# This script waits for SQL Server to be ready

set -e

echo "Waiting for SQL Server to be ready..."

# Wait for SQL Server to be ready (max 60 seconds)
for i in {1..60}; do
    if nc -z sqlserver 1433 2>/dev/null; then
        echo "SQL Server is ready!"
        echo "Database initialization completed!"
        exit 0
    fi
    echo "Waiting for SQL Server... ($i/60)"
    sleep 1
done

echo "SQL Server did not become ready in time"
exit 1 