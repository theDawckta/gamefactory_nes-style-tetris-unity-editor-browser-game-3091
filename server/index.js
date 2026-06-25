const express = require('express');
const cors = require('cors');
const bodyParser = require('body-parser');
const fs = require('fs');
const path = require('path');

const app = express();
const PORT = 3000;
const SCORES_FILE = path.join(__dirname, 'scores.json');
const MAX_ENTRIES = 5;

const PLACEHOLDER = { initials: '---', score: 0 };

app.use(cors());
app.use(bodyParser.json());

function loadScores() {
    if (!fs.existsSync(SCORES_FILE)) {
        const initial = Array.from({ length: MAX_ENTRIES }, () => ({ ...PLACEHOLDER }));
        fs.writeFileSync(SCORES_FILE, JSON.stringify(initial), 'utf8');
        return initial;
    }
    return JSON.parse(fs.readFileSync(SCORES_FILE, 'utf8'));
}

function saveScores(scores) {
    fs.writeFileSync(SCORES_FILE, JSON.stringify(scores), 'utf8');
}

function sortDescending(scores) {
    return scores.slice().sort((a, b) => b.score - a.score);
}

app.get('/leaderboard', (req, res) => {
    const scores = loadScores();
    res.json({ scores: sortDescending(scores) });
});

app.post('/leaderboard', (req, res) => {
    const { initials, score } = req.body;

    if (typeof initials !== 'string' || typeof score !== 'number') {
        return res.status(400).json({ error: 'initials (string) and score (number) are required' });
    }

    const scores = loadScores();
    const sorted = sortDescending(scores);
    const lowestScore = sorted[sorted.length - 1].score;
    const realEntries = sorted.filter(e => e.initials !== '---').length;

    if (realEntries >= MAX_ENTRIES && score <= lowestScore) {
        return res.json({ scores: sorted });
    }

    sorted.push({ initials, score });
    const updated = sortDescending(sorted).slice(0, MAX_ENTRIES);

    while (updated.length < MAX_ENTRIES) {
        updated.push({ ...PLACEHOLDER });
    }

    saveScores(updated);
    res.json({ scores: updated });
});

app.listen(PORT, () => {
    console.log('Leaderboard server running on port ' + PORT);
});
