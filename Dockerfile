FROM python:3.9-slim

# https://x.com/i/grok?conversation=1911893787481092579

# Install system dependencies
RUN apt-get update && apt-get install -y \
    git \
    && rm -rf /var/lib/apt/lists/*

# Install Python dependencies
RUN pip install --no-cache-dir \
    optimum[exporters] \
    transformers \
    torch \
    accelerate \
    onnx

# Set working directory
WORKDIR /app

# Command to keep container running interactively
CMD ["/bin/bash"]