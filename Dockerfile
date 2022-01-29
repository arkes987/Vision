FROM balenalib/raspberry-pi-alpine-node:14-3.13 as builder

WORKDIR /app
COPY ./frontend/package*.json ./
RUN npm install --no-optional

ARG REACT_APP_BUILD

ENV REACT_APP_BUILD $REACT_APP_BUILD

COPY ./frontend/ ./
RUN npm run build

COPY ./frontend/.env ./build

FROM nginx:stable

COPY ./frontend/nginx.conf /etc/nginx/conf.d/default.conf

COPY --from=builder /app/build /usr/share/nginx/html
COPY --from=builder /app/build/.env /usr/share/nginx/html

EXPOSE 80
EXPOSE 443

CMD /bin/sh -c "cd /usr/share/nginx/html && nginx -g 'daemon off;'"