FROM node:16

# Create app directory
WORKDIR /usr/src/app

# Install app dependencies
# A wildcard is used to ensure both package.json and package-lock.json are copied
COPY ./webapp/package*.json ./

RUN npm install
# For production:
# RUN npm ci --only=production

# Bundle app source
COPY ./app ./

EXPOSE 3000

CMD [ "npm", "run", "dev" ]
