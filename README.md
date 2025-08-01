# Dungeon Crawler
A text-based RPG game built with Blazor Server.

## Deployment
The project is deployed on a VPS server and accessible at http://104.197.255.122:5000/


## Tech Stack
- ASP.NET Core 8.0
- Blazor Server
- C#

## Project Structure
```
Dungeon Crawler/
├── Components/
│   ├── Layout/              # UI layouts
│   ├── Models/              # Game entities
│   ├── Pages/               # Blazor pages
│   ├── Services/            # Business logic
│   ├── App.razor            # Router
│   ├── Routes.razor         # Route configuration  
│   └── _Imports.razor       # Global imports
├── Properties/              # Launch settings
├── wwwroot/                 # Static files
└── Program.cs               # Application entry point
```

## Architecture
Uses Blazor Server with component-based architecture, service injection for game logic, and real-time UI updates via SignalR.

