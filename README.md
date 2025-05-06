[![build and test](https://github.com/clashmar/WhatMunch/actions/workflows/dotnet.yml/badge.svg)](https://github.com/clashmar/WhatMunch/actions/workflows/dotnet.yml)
[![Django CI](https://github.com/clashmar/WhatMunch/actions/workflows/django.yml/badge.svg)](https://github.com/clashmar/WhatMunch/actions/workflows/django.yml)

# 🍽️ WhatMunch
WhatMunch is a cross platform mobile application that makes it faster to find good eats on the move, tailored to your preferences, designed for speed and simplicity.

---

### 🧭 .NET MAUI
- Built using **MVVM architecture** with `CommunityToolkit.Mvvm`
- **Google Places API** integration to scan nearby restaurants based on your saved preferences
- Mark favorite locations to revisit later
- Local **SQLite** database for offline storage of favorites and preferences
- Fully localized to **Dutch (nl-NL)** with `.resx` resource localization
- Includes **OAuth login with Google** for easy sign-in
- Uses **XUnit** for frontend unit and integration testing

### 🛠️ Django Backend
- Built with **Django REST Framework**
- Uses **PostgreSQL** for persistent data storage
- Fully **containerized with Docker Compose**
- Auth powered by **django-allauth** and **JWT tokens** for secure API access
- Includes:
  - **Health check** endpoint for container orchestration
  - Centralized **logging middleware** for request tracking and debugging
- Tested with **Pytest** and `pytest-django`
