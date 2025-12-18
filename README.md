# PDF Market 

PDF Market er en digital markedsplads til upload, køb og download af PDF-dokumenter.  
Projektet er udviklet som semesterprojekt på **3. semester Datamatiker (EASV)** og demonstrerer anvendelsen af **3-tier arkitektur**, **Clean Architecture**, **JWT-baseret sikkerhed** og **Docker-baseret backend-opsætning**.

Dette repository indeholder **backend, administrationsklient, tests samt Postman-tests**.  
React-webklienten ligger i et **separat repository**.

---

##  Indholdsfortegnelse
- [Systemoverblik](#systemoverblik)
- [Arkitektur](#arkitektur)
- [Funktionalitet](#funktionalitet)
- [Teknologier](#teknologier)
- [Projektstruktur](#projektstruktur)
- [Opsætning og kørsel](#opsætning-og-kørsel)
- [Postman tests](#postman-tests)
- [Test og kvalitet](#test-og-kvalitet)
- [Projektkontekst](#projektkontekst)
- [Forfatter](#forfatter)

---

## Systemoverblik

PDF Market er opdelt i tre tiers:

### Tier 1 – Klienter
- **WPF Admin Client (C# / MVVM)**  
  Administrationsklient til moderation, brugerstyring og systemoverblik.
- **React Web Client (TypeScript)**  
  Brugerklient til browsing, køb, upload og download af PDF’er  
  *(ligger i separat repository)*

### Tier 2 – Backend
- **.NET 8 Minimal API**
- RESTful endpoints
- JWT-baseret autentifikation og autorisation
- Clean Architecture-opbygning

### Tier 3 – Database
- **MongoDB**
- **GridFS** til lagring af PDF-filer
- Docker-containeriseret database

---

## Arkitektur

Projektet følger principperne fra **Clean Architecture**:

- **Domain**  
  Kerneforretningslogik og entiteter uden afhængighed til eksterne teknologier.
- **Application**  
  Use cases, services og interfaces (repositories, security m.m.).
- **Infrastructure**  
  Implementering af MongoDB, JWT, GridFS og andre tekniske detaljer.
- **Contracts**  
  DTO’er og API-kontrakter anvendt af klienter.
- **Interface Adapters (API)**  
  Endpoints og routing til klienterne.

Afhængigheder peger udelukkende indad mod Domain-laget.

---

## Funktionalitet

### Gæster
- Browse PDF-katalog
- Se PDF-detaljer
- Søge og filtrere indhold

### Brugere
- Opret konto og log ind
- Uploade PDF-filer
- Købe PDF’er via pointsystem
- Downloade købte PDF’er
- Se egne uploads og køb

### Administratorer
- Moderere PDF-uploads
- Aktivere/deaktivere PDF’er
- Administrere brugere
- Se systemstatistik

---

## Teknologier

**Backend**
- .NET 8 Minimal API
- JWT Authentication
- Swagger / OpenAPI

**Admin Client**
- WPF
- MVVM
- C#

**Database & Drift**
- MongoDB
- GridFS
- Docker
- Docker Compose

**Test & Kvalitet**
- Postman
- Unit tests
- Cyclomatic Complexity analyse

---

## Projektstruktur

```text
PdfMarket
│
├── PdfMarket.API              // Web API (Minimal API)
├── PdfMarket.AdminClient      // WPF Admin Client (MVVM)
├── PdfMarket.Contracts        // DTO’er og API-kontrakter
├── PdfMarket.Tests            // Unit tests
│
├── postman                    // Postman collections & environments
├── GitHub Actions             // CI / workflow-konfiguration
├── Solution Items
│
└── PdfMarket.sln

🔗 React Web Client
Ligger i separat repository og kommunikerer med samme Web API.

Opsætning og kørsel
Forudsætninger
Docker Desktop

.NET 8 SDK

Visual Studio

Start backend og database
bash
Kopier kode
docker compose up --build
API: http://localhost:8080

Swagger UI: http://localhost:8080/swagger

Start WPF Admin Client
Åbn solution i Visual Studio

Start PdfMarket.AdminClient

Log ind med administratorbruger (seedet)

Postman tests
Postman-collections og environments ligger i mappen:

text
Kopier kode
/postman
Her findes tests for:

Auth (login / register)

PDF browse og detaljer

Upload, køb og download

Admin-funktioner

Tests anvendes både til manuel verifikation og dokumentation af API-funktionalitet.

Test og kvalitet
API’et er dokumenteret via Swagger / OpenAPI

Funktionalitet er testet via Postman

Udvalgte metoder er analyseret med Cyclomatic Complexity

Unit tests er udført på metoder med høj kompleksitet

Projektkontekst
Projektet er udviklet som en del af 3. semester eksamen på Datamatikeruddannelsen (EASV).
Fokus har været på arkitektur, softwaredesign og integration mellem flere klienttyper.

Avancerede databasefunktioner som MongoDB-transaktioner er bevidst fravalgt for at holde fokus på semesterets læringsmål.

Forfatter
Emil Rosholm
Datamatiker, 3. semester – EASV
