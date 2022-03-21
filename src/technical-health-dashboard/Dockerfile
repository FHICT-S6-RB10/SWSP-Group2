FROM node:16 AS builder

# Create app directory
WORKDIR /app

# Install app dependencies
# A wildcard is used to ensure both package.json and package-lock.json are copied
COPY ./webapp/package*.json .

# Installs all node packages
RUN npm install
# For production:
# RUN npm ci --only=production

# Copies everything over to Docker environment
COPY ./webapp ./
RUN npm run build

FROM nginx:1.19.0

# Nginx config
RUN rm -rf /etc/nginx/conf.d
COPY ./webapp/conf /etc/nginx

# Static build
COPY --from=builder /app/dist /usr/share/nginx/html/

# Default port exposure
EXPOSE 80

# Copy .env file and shell script to container
WORKDIR /usr/share/nginx/html
COPY ./webapp/env.sh .
COPY ./webapp/.env .

# Add bash
RUN apk add --no-cache bash

# Make our shell script executable
RUN chmod +x env.sh

# Start Nginx server
CMD ["/bin/bash", "-c", "/usr/share/nginx/html/env.sh && nginx -g \"daemon off;\""]
