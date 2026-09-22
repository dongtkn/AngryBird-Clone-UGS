# Angry Birds 2D Clone — Unity 6

A fully functional 2D Angry Birds clone built with **Unity 6**, featuring **Google Authentication** and real-time global **Leaderboards** powered by **Unity Gaming Services (UGS)**.

![Gameplay Demo](docs/gameplay.gif)

---

## Project Overview
This project showcases core 2D game development practices, physical trajectory mechanics, and cloud backend integration. Designed with clean architecture, decoupled event-driven modules, and asynchronous programming in C#.

---

## Tech Stack
* **Game Engine:** Unity 6 (2D Physics & URP)
* **Language:** C# (.NET / Asynchronous Programming)
* **Backend Services:**
  * **Unity Gaming Services (UGS):** Authentication, Player Accounts, Leaderboard Service
  * **OAuth 2.0:** Google Auth Integration
* **Architecture & Patterns:**
  * **Singleton Pattern:** Global state management (`GameManager`, `ScoreManager`)
  * **Observer Pattern (C# Events):** Decoupled UI updates and authentication event handling
  * **Async / Await:** Non-blocking network requests for cloud services

---

## Key Features
* **2D Trajectory & Sling Physics:** Real-time trajectory prediction using `Rigidbody2D` and `SpringJoint2D`.
* **Google OAuth & UGS Auth:** Player sign-in flow with fallback profile setup.
* **Global Leaderboard:** Automatic high-score submissions upon winning and real-time top-rank fetching.
* **Score System:** Dynamic scoring algorithm calculated based on shot efficiency.

---

## Getting Started

### Prerequisites
* **Unity Engine:** `6000.x` or higher
* Internet connection for UGS cloud authentication and leaderboard updates

### Installation
1. **Clone the repository:**
   ```bash
   git clone [https://github.com/dongtkn/AngryBird-Clone-UGS.git](https://github.com/dongtkn/AngryBird-Clone-UGS.git)
