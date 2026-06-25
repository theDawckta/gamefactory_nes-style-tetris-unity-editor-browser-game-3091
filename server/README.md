# NES Tetris Leaderboard Server

Node.js Express server that persists the global top 5 scores to a local JSON file.

## Setup

```
npm install
node index.js
```

Server listens on port 3000.

## Endpoints

### GET /leaderboard

Returns the current top 5 scores sorted descending by score.

Response:
```json
{"scores": [{"initials": "AAA", "score": 12345}, ...]}
```

### POST /leaderboard

Submit a new score. Inserts the entry if it qualifies for the top 5.

Request body:
```json
{"initials": "AAA", "score": 12345}
```

Response: updated scores list (same format as GET).

A score that does not beat the current lowest entry is rejected and the list is returned unchanged.

## Data File

Scores are persisted to `scores.json` in the same directory. The file is created automatically with 5 placeholder entries if it does not exist.
