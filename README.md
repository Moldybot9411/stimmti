# Stimmti

> An interactive live-survey tool best used for presentations of any kind

## Setting up the dev environment

```bash
# Setting up a development SQL Database
docker compose -f devdb.compose.yaml up -d

# Copy and adjust environment variables for the frontend
cp .env.example Frontend/.env

# Starting the Backend
cd Backend
dotnet restore
dotnet run

# Starting the Frontend
cd ../Frontend
npm i
npm run dev
```

Frontend environment values are documented in `.env.example`.

**Where to access what**
|PhpMyAdmin|Swagger Docs|Frontend|
|:--|:--|:--|
|http://localhost:8080|http://localhost:5202/swagger/index.html|http://localhost:5173|

The development DB user is `root` with the password `root`.
