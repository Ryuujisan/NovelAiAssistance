# 📖 Novel AI Assistance

**Novel AI Assistance** is a web-based platform that integrates AI models with a user-friendly interface for creative writing support.  
It allows users to generate, edit, and organize text with the help of advanced AI while keeping full control over their data and workflow.

---

## 🚀 Features
- ✨ Modern **.NET 9** backend with PostgreSQL support
- 🖥️ React frontend with TailwindCSS for a clean UI
- 🔐 Secure authentication and role-based access
- 📂 Project & document management system
- ⚡ Fast API responses for interactive AI writing
- 🛠️ Easy local setup with PostgreSQL + pgAdmin

---

## 🛠️ Tech Stack
- **Backend**: .NET 9 (ASP.NET Core Web API)
- **Frontend**: React (Vite + TypeScript + TailwindCSS)
- **Database**: PostgreSQL (managed with EF Core Migrations)
- **Dev Tools**: Git, Docker (optional), pgAdmin

---

## 📦 Installation & Setup

### 1. Clone the repository
```bash
git clone git@github.com:Ryuujisan/NovelAiAssistance.git
cd NovelAiAssistance
```

### 2. Backend setup
```bash
cd API
dotnet restore
dotnet ef database update
dotnet run
```

### 3. Frontend setup
```bash
cd client
npm install
npm run dev
```

### 4. Database (PostgreSQL)
Create a database `ainovel_db` (or use your own name).  
Update your connection string in `appsettings.Development.json`:

```json
"ConnectionStrings": {
  "Postgres": "Host=localhost;Port=5432;Database=ainovel_db;Username=izumi;Password=1234"
}
```

---

## 📚 Usage
1. Start the backend server:
   ```bash
   dotnet run
   ```
2. Start the frontend:
   ```bash
   npm run dev
   ```
3. Open [http://localhost:5173](http://localhost:5173) in your browser
4. Begin creating and managing your AI-assisted writing projects ✍️

---

## 🤝 Contribution
Contributions are welcome!

1. Fork the repo
2. Create a feature branch:
   ```bash
   git checkout -b feature/amazing-feature
   ```
3. Commit your changes:
   ```bash
   git commit -m "Add amazing feature"
   ```
4. Push the branch:
   ```bash
   git push origin feature/amazing-feature
   ```
5. Open a Pull Request

---

## 📜 License
This project is licensed under the **MIT License**.  
See the [LICENSE](LICENSE) file for details.

---

## 💡 Credits
Created with ❤️ by **Ryuujisan**