# MatchmakingAPI

An in progress backend API for a skill-based matchmaking system, built with ASP.NET Core (.NET 10). Players register and log in, the server tracks their match stats, and players queue up and get matched into 2v2 games with others of a similar skill level.

The game itself doesn't exist. The project is about the backend: authentication, persistence, and the matchmaking logic and real-time notifications.

**What it demonstrates**

- ASP.NET Core Web API with controllers and a service layer
- EF Core with SQLite, two separate `DbContext`s, and migrations
- JWT authentication: password hashing, token issuing, and `[Authorize]`-protected endpoints that identify the player from the token
- *(in progress)* In-memory matchmaking with concurrency control, and SignalR for pushing matches to clients

## Status

This is a work in progress.

- [x] Project setup, EF Core + SQLite
- [x] Player accounts: register, login, JWT
- [x] Player stats storage and lookup
- [x] Skill calculation from match results
- [ ] Submitting match results
- [x] Matchmaking rooms
- [ ] Handle disconnection while searching for a match
- [x] SignalR notifications
- [ ] Tests

## Running it

**Requirements:** [.NET 10 SDK](https://dotnet.microsoft.com/download)

```bash
git clone https://github.com/Agustin-E-Garcia/MatchmakingAPI.git
cd MatchmakingAPI

# Restore the local EF Core CLI tool
dotnet tool restore

# Set the JWT signing key (kept in user secrets, not in the repo).
# HMAC-SHA256 needs at least 32 characters.
dotnet user-secrets set "Jwt:Key" "replace-with-a-long-random-string-of-32+-chars"

# Create both databases
dotnet ef database update --context PlayerAuthDbContext
dotnet ef database update --context StatsDbContext

dotnet run
```

The API listens on `http://localhost:5095`.

## Endpoints

| Method | Route | Auth | Description |
| --- | --- | --- | --- |
| `POST` | `/api/auth/register` | — | Create an account. Returns a JWT. |
| `POST` | `/api/auth/login` | — | Log in. Returns a JWT. |
| `GET` | `/api/auth/me` | ✔ | The logged-in player's ID and username, read from the token. |
| `GET` | `/api/playerStats/{id}` | — | A player's stats. |
| `POST` | `/api/playerStats` | ✔ | *(in progress)* Submit match results. |

Authenticated endpoints expect the token in a header: `Authorization: Bearer <token>`.

**Example**

```bash
curl -X POST http://localhost:5095/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"username": "player1", "password": "password123"}'
```

Usernames must be 1–20 characters and unique (409 if taken). Passwords must be at least 8 characters.

## How matchmaking works (planned design)

1. A logged-in player opens a SignalR connection and automatically joins the queue.
2. The server looks for an open **room** that is not full (eventually it will use an elo band value). If none fits, it creates a new room.
3. When a room reaches 4 players, it launches. Everyone in the room receives the room name over SignalR.
4. After the match, each client submits its stats. The server updates the database and recalculates the player's skill value.

Rooms live in memory, in a Singleton service. Finding a room, adding a player and launching a full room happen as one locked step, so two players taking the last slot at the same time can't produce a 7-player room. A player who disconnects is removed from their room, and empty rooms are deleted.

## Design decisions

- **Player identity comes from the token.** Endpoints read the player ID from the JWT's `sub` claim (via a `ClaimsPrincipal` extension method), never from the request body, so a client can't act as another player.
- **Passwords are hashed** with ASP.NET Core Identity's `PasswordHasher`. Usernames have a unique index in the database.
- **The JWT signing key lives in user secrets**, not in `appsettings.json`, so it's never committed.
- **Credentials and stats are in separate databases**, each with its own `DbContext` and migrations, keeping account data apart from gameplay data.
- **Matchmaking state is in memory.** A queue only matters while players are connected, so it doesn't need to survive a restart. If the server restarts, clients reconnect and queue again.
- **The server pushes matches to clients with SignalR.** The client opens the connection, because a server can't reliably connect to clients behind home routers and firewalls.

## Known simplifications

These are deliberate scope choices for a portfolio project:

- **Clients report their own match stats.** In a real game, a trusted game server would submit them. Building one is out of scope here.
- **Skill is based on KDA.** It's simple to calculate, not an accurate measure of skill. The formula is meant to be isolated so it's easy to replace (with Elo, for example).
- **Teams are split randomly** once a room fills, instead of being balanced.
- **Queue state isn't persisted** (see above).
