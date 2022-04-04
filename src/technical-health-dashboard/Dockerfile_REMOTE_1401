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
# Copies React to the container directory
# Set working directory to nginx resources directory
WORKDIR /usr/share/nginx/html

# Remove default nginx static resources
RUN rm -rf ./*

# Copies static resources from builder stage
COPY --from=builder ./app/dist .

# Containers run nginx with global directives and daemon off
ENTRYPOINT ["nginx", "-g", "daemon off;"]
