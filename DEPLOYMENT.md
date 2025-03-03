# Recommended Deployment Strategy for QA Platform

This document outlines the recommended deployment process for the **QA Platform**, which consists of:
- A **React Frontend**
- An **ASP.NET Core API Backend**
- A **PostgreSQL Database**
- **NGINX Reverse Proxy**
- **Cloudflare DNS & Tunnels** for securing and managing traffic
- **Docker & Docker Compose** for containerized deployment

This setup is designed for a **staging environment** that can be extended for production use
and have been tested on [Debian Bookworm](https://www.debian.org/releases/bookworm/).

---

#### **Why Cloudflare?**

Cloudflare enhances security and performance through **DNS management and tunnels**.

- **DNS Management:** Handles domain resolution efficiently.
- **Tunnels:** Securely expose services without opening ports.
- **TLS/SSL Protection:** Provides **Full (Strict) SSL encryption**.

---

## Deployment Instructions

### **1. Prerequisites**
Ensure you have the following installed on your server:
- **A registered domain or sub-domain for the front-end (e.g., `qa.stridsmaskinerna.online`)** 
- **A registered domain or sub-domain for the back-end (e.g., `qa-api.stridsmaskinerna.online`)** 
- **Docker** & **Docker Compose**
- **NGINX**
- **Cloudflare Account**
- **A GitHub personal access token (GAT)** 
- **A Linux environment like `Debian Bookworm`**

---

### **2. Update Cloudflare to manage your domains**

Doc: [Cloudflare DNS Setup](https://developers.cloudflare.com/dns/zone-setups/full-setup/setup/)

### **3. Deploying with Docker Compose**
The **QA Platform** consists of three services:
1. **Front-end**: React app served by NGINX.
2. **Back-end**: ASP.NET Core API.
3. **Database**: PostgreSQL.

#### **3.1. Update environments**

Ensure the correct environment variables are set in front-end `.env`:
```ini
# Frontend Environment Variables
VITE_BASE_URL=https://<YOUR_BACKEND_DOMAIN>/api
```

Ensure the correct environment variables are set in
[GitHub Secrets](https://github.com/stridsmaskinerna/QA-Platform-Backend/settings/secrets/actions).

- `[JWT_AUDIENCE]: https://<YOUR_FRONTEND_DOMAIN>`
- `[JWT_ISSUER]: https://<YOUR_BACKEND_DOMAIN>`
- `[POSTGRES_CON]: Server=qa-platform-db;Port=5432;Database=<POSTGRES_DB>;Username=<POSTGRES_USER>;Password=<POSTGRES_PASSWORD>`  
- `[SECRET_KEY]: A_secret_key_usedfor_JWT_token_min_128bit`
- `[SEED_ADMIN_MAIL]: A Default Admin mail`
- `[JWT_AUDIENCE]: A Default Admin password`

**OBS!** The values of `GitHub Secrets` will not be displayed after updating or setting these values.

**OBS!** Use your `GitHub Secrets` values later in your `docker-compose.yml` file for deployment.

#### **3.2. Build container images through GitHub pull-requests**

Make a GitHub pull request to the `Deployment` branches of the front-end and back-end repositories. The pull request will
build tagged docker images in [GitHub Packages](https://github.com/orgs/stridsmaskinerna/packages) that should
later be used for deployment, e.g.:

`docker pull ghcr.io/stridsmaskinerna/qa-platform-client:pr-bcd9199dc0bfedf2388f1561a4b2fdb436c33b5e`

`docker pull ghcr.io/stridsmaskinerna/qa-platform-api:pr-35270c87b51a4af929f1333936dd7a145cb6685b`

#### **3.3. Update docker compose**

Use [docker-compose-example-production.yml](docker-compose-example-production.yml) as a example file to create a `docker-compose.yml` for deployment.

- Set container tags; needs to be the same tags as the built images in `GitHub Packages`.

- Set `POSTGRES_USER`, `POSTGRES_PASSWORD`, and `POSTGRES_DB`; needs to be matching values from `GitHub Secrets`.

#### **3.4. Deploy Services**
Run the following commands to start the deployment:

- `docker login ghcr.io -u <your-github-username>`, then enter your GitHub personal access token.
- `docker compose -f docker-compose.yml up -d`

This will:
- Pull the **React frontend** and **ASP.NET backend** images from `GitHub Packages`.
- Start the **PostgreSQL database** and persist data using Docker volumes.
- Expose ports for NGINX to serve traffic.

#### **3.5. Verify Running Containers**
Check that all services are running:
```sh
docker ps
```

---

### **4. Configuring NGINX Reverse Proxy**
NGINX is used to **route traffic** from your domains to the correct docker services.


#### **4.1. Frontend NGINX Configuration**

Create file `/etc/nginx/sites-available/<YOUR_DOMAIN_>.conf`
```nginx
server {
    listen 80;
    listen [::]:80;
    server_name <YOUR_FRONTEND_DOMAIN>;

    location / {
        proxy_pass http://localhost:3333;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto https;
    }
}
```

#### **4.2. Backend NGINX Configuration**

Create file `/etc/nginx/sites-available/<YOUR_BACKEND_DOMAIN>.conf`
```nginx
server {
    listen 80;
    listen [::]:80;
    server_name <YOUR_BACKEND_DOMAIN>;

    location / {
        proxy_pass http://localhost:5051;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto https;
    }
}
```

#### **4.3. Link Configurations files**

After configuring NGINX, link configuration files:

- `sudo ln -s /etc/nginx/sites-available/<YOUR_FRONTEND_DOMAIN>.conf /etc/nginx/sites-enabled/<YOUR_FRONTEND_DOMAIN>.conf`

- `sudo ln -s /etc/nginx/sites-available/<YOUR_BACKEND_DOMAIN>.conf /etc/nginx/sites-enabled/<YOUR_BACKEND_DOMAIN>.conf`


#### **4.4. Start Nginx**

After linking configuration files, restart the service:
`sudo systemctl restart nginx`

Ensure NGINX is running:
`sudo systemctl status nginx`

---

### **5. Setting Up Cloudflare Tunnels & TLS/SSL Protection**

#### **5.1. Set Up Cloudflare Tunnel**

Doc: [Cloudflare Tunnel](https://developers.cloudflare.com/cloudflare-one/connections/connect-networks/)

#### **5.2 Enable SSL/TLS Full (Strict)**

1. In **Cloudflare Dashboard**, go to **SSL/TLS**.

2. Set mode to **Full (Strict)** for end-to-end encryption.

---

### **6. Running the Deployment**
After setting up **Docker, NGINX, and Cloudflare**, restart all services:
```sh
docker compose down && docker compose up -d
sudo systemctl restart nginx
sudo systemctl restart cloudflared
```

### **7. Verify Deployment**
1. Visit `https://<YOUR_FRONTEND_DOMAIN>`
2. Call the API at `https://<YOUR_BACKEND_DOMAIN>/api`
3. Ensure database connectivity by checking logs:
```sh
docker logs qa-platform-db
```

---

### **8. Final Notes**
- **Scaling:** This setup can be extended for production by adding for example **load balancing**, **replication**, and **monitoring**.
- **Security:** Always keep **Cloudflare Tunnel** running to avoid exposing open ports.
- **Troubleshooting:** Check logs using:
  ```sh
  docker logs <container_name>
  journalctl -u nginx --no-pager | tail -n 50
  ```

---
