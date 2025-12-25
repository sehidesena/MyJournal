# MyJournal

**MyJournal** is an AI-powered mental health journaling platform designed to help users track their emotional well-being, get personalized insights, and receive supportive AI suggestions. Built with modern web technologies and the ABP (ASP.NET Boilerplate) Framework, MyJournal provides a secure and user-friendly experience.

## 🎯 Features

- **Smart Journaling**: Write journal entries and track your thoughts and feelings
- **AI-Powered Insights**: Get intelligent suggestions and analysis of your journal entries using Gemini AI
- **Mental Health Focus**: A dedicated platform designed with mental wellness in mind
- **User Authentication**: Secure OAuth2.0 authentication with OpenIddict
- **Multi-language Support**: Support for multiple languages (Czech, German, Hindi, Icelandic, Italian, and more)
- **Responsive Design**: Works seamlessly on desktop and mobile devices
- **Real-time Updates**: Built with Angular 20 and ASP.NET Core 10

## 📋 Prerequisites

Before you begin, make sure you have the following installed:

- [.NET 10.0+ SDK](https://dotnet.microsoft.com/download/dotnet)
- [Node.js v18 or v20](https://nodejs.org/en)
- [PostgreSQL](https://www.postgresql.org/download/) (for database)
- [Docker and Docker Compose](https://www.docker.com/) (optional, for containerized deployment)

## 🚀 Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/MyJournal.git
cd MyJournal
```

### 2. Install Client-Side Dependencies

```bash
abp install-libs
```

This installs all NPM packages for the Angular UI. If you don't have ABP CLI installed, you can install it first:

```bash
dotnet tool install -g Volo.Abp.Cli
```

### 3. Configure User Secrets (IMPORTANT!)

This project uses **.NET User Secrets** to store sensitive configuration data like API keys and passwords. **You must configure these before running the application.**

#### Required Secrets:

Run the following commands from the project root directory:

```bash
cd Mentalfull

# Initialize User Secrets (if not already done)
dotnet user-secrets init

# Set Azure OpenAI Configuration
dotnet user-secrets set "Ai:ApiKey" "YOUR_AZURE_OPENAI_API_KEY"
dotnet user-secrets set "Ai:Endpoint" "YOUR_AZURE_OPENAI_ENDPOINT"

# Set Pinecone Configuration
dotnet user-secrets set "Pinecone:ApiKey" "YOUR_PINECONE_API_KEY"
dotnet user-secrets set "Pinecone:Host" "YOUR_PINECONE_HOST_URL"

# Set Database Connection String
dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Port=5432;Database=Mentalfull;User ID=postgres;Password=YOUR_PASSWORD;"

# Set Certificate PassPhrase
dotnet user-secrets set "AuthServer:CertificatePassPhrase" "YOUR_CERTIFICATE_PASSPHRASE"

# Set String Encryption Key
dotnet user-secrets set "StringEncryption:DefaultPassPhrase" "YOUR_ENCRYPTION_PASSPHRASE"
```

#### How to Get API Keys:

- **Azure OpenAI**: Sign up at [Azure Portal](https://portal.azure.com/) → Create OpenAI resource → Get API Key and Endpoint
- **Pinecone**: Sign up at [Pinecone.io](https://www.pinecone.io/) → Create project → Get API Key and Host URL

#### Verify Your Secrets:

```bash
dotnet user-secrets list
```

### 4. Setup Database

The application uses PostgreSQL. Make sure PostgreSQL is running and create a database:

```sql
CREATE DATABASE Mentalfull;
```

**Note:** Your database connection string is already configured in User Secrets (step 3)

### 5. Run Database Migrations

```bash
cd Mentalfull
dotnet ef database update
```

Or use the provided migration script:

```powershell
.\migrate-database.ps1
```

### 6. Generate Signing Certificate

```bash
cd Mentalfull
dotnet dev-certs https -v -ep openiddict.pfx -p YOUR_CERTIFICATE_PASSPHRASE
```

**Note:** Use the same passphrase you set in User Secrets (step 3)

## 🏃 Running the Application

### Development Mode

**Backend** (from `Mentalfull` folder):
```bash
dotnet run
```

The backend will run at `https://localhost:44376`

**Frontend** (from `Mentalfull/angular` folder):
```bash
npm start
```

The frontend will run at `http://localhost:4200`

### Production Build

**Backend**:
```bash
dotnet publish -c Release
```

**Frontend**:
```bash
npm run build:prod
```

## 🐳 Docker Deployment

Build and run with Docker Compose:

```bash
cd etc/docker
.\run-docker.ps1
```

To stop Docker containers:

```bash
.\stop-docker.ps1
```

## 📁 Project Structure

```
MyJournal/
├── Mentalfull/                 # Main ASP.NET Core application
│   ├── Controllers/            # API controllers
│   ├── Entities/               # Domain entities (JournalEntry, AiSuggestions, etc.)
│   ├── Services/               # Business logic
│   ├── Data/                   # Database context and migrations
│   ├── Localization/           # Multi-language support
│   └── Permissions/            # Authorization policies
├── Mentalfull.Tests/          # Unit tests
├── angular/                    # Angular 20 frontend application
│   ├── src/
│   │   ├── app/               # Application components
│   │   ├── assets/            # Static assets
│   │   └── environments/      # Environment configurations
│   └── package.json
├── etc/                        # Configuration and scripts
│   ├── docker/                # Docker setup
│   └── scripts/               # Utility scripts
└── Mentalfull.sln            # Visual Studio solution
```

## 🔧 Configuration

### Environment Variables

All sensitive configuration is stored using **.NET User Secrets** (configured in step 3 of Getting Started).

**Never commit the following files to Git:**
- `appsettings.Development.json` (if it contains real secrets)
- `appsettings.*.json` (any environment-specific configs with secrets)
- `secrets.json`

The `appsettings.json` file in the repository contains only placeholder values. Real values must be set via User Secrets.

### Required Configuration Values:

| Key | Description | Example |
|-----|-------------|---------|
| `Ai:ApiKey` | Azure OpenAI API Key | `sk-...` |
| `Ai:Endpoint` | Azure OpenAI Endpoint | `https://your-resource.openai.azure.com/` |
| `Pinecone:ApiKey` | Pinecone Vector DB API Key | `pcsk_...` |
| `Pinecone:Host` | Pinecone Host URL | `https://your-index.pinecone.io` |
| `ConnectionStrings:Default` | PostgreSQL Connection String | `Host=localhost;Port=5432;...` |
| `AuthServer:CertificatePassPhrase` | OpenIddict Certificate Password | Any secure string |
| `StringEncryption:DefaultPassPhrase` | Encryption Key | Any secure string |

## 🧪 Testing

Run unit tests:

```bash
cd Mentalfull.Tests
dotnet test
```

Run Angular tests:

```bash
cd Mentalfull/angular
npm test
```

## 📚 Key Technologies

- **Backend**: ASP.NET Core 10, Entity Framework Core, OpenIddict
- **Frontend**: Angular 20, TypeScript, SCSS, ABP Framework
- **Database**: PostgreSQL
- **AI Integration**: Azure OpenAI, Pinecone Vector Database
- **Authentication**: OAuth 2.0 with OpenIddict
- **Deployment**: Docker, Docker Compose

## 🔐 Security Features

- Secure OAuth 2.0 authentication with OpenIddict
- HTTPS enforced in production
- **User Secrets** for sensitive data (API keys never in source code)
- Encrypted sensitive data with configurable encryption keys
- Role-based access control (RBAC)
- `.gitignore` configured to prevent accidental credential commits

## 📖 API Documentation

Once the application is running, access the Swagger UI at:

```
https://localhost:44376/swagger
```

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🆘 Support

For issues, questions, or feedback, please open an issue on the GitHub repository.

## 📞 Contact

- Email: support@myjournal.app
- Website: [www.myjournal.app](https://www.myjournal.app)

## 🙏 Acknowledgments

- [ABP Framework](https://abp.io) - Modern application development framework
- [Angular](https://angular.io) - Frontend framework
- [ASP.NET Core](https://dotnet.microsoft.com/en-us/apps/aspnet) - Backend framework
- [Google Gemini AI](https://ai.google.dev) - AI capabilities

---

**Happy Journaling! 📔✨**

Last Updated: December 2024

Navigate to [etc/build](./etc/build) folder and run the `build-images-locally.ps1` script. You can examine the script to set **image tag** for your images. It is `latest` by default.

#### Running the Docker images using Docker-Compose

Navigate to [etc/docker](./etc/docker) folder and run the `run-docker.ps1` script. The script will generate developer certificates (if it doesn't exist already) with `dotnet dev-certs` command to use HTTPS. Then, the script runs the provided docker-compose file on detached mode.

> Not: Developer certificate is only valid for **localhost** domain. If you want to deploy to a real DNS in a production environment, use LetsEncrypt or similar tools.

#### Stopping the Docker containers

Navigate to [etc/docker](./etc/docker) folder and run the `stop-docker.ps1` script. The script stops and removes the running containers.

### Additional resources

You can see the following resources to learn more about your solution and the ABP Framework:

* [Application (Single Layer) Startup Template](https://abp.io/docs/latest/startup-templates/application-single-layer/index)
