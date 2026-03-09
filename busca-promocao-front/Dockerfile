# Build stage (Node)
FROM node:20-alpine AS builder
WORKDIR /app

# Instalação de dependências
COPY package*.json ./
RUN npm ci

# Copia código-fonte e builda
COPY . .
RUN npm run build

# Runtime stage (Nginx)
FROM nginx:alpine AS runtime

# Remove o HTML padrão do nginx
RUN rm -rf /usr/share/nginx/html/*

# Copia os arquivos minificados do Angular
COPY --from=builder /app/dist/busca-promocao-front/browser /usr/share/nginx/html

# Configuração customizada do NGINX para SPA (Single Page Application - redireciona rotas p/ index.html)
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
