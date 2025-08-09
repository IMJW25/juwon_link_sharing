const express = require('express');
const http = require('http');
const cors = require('cors');
const mongoose = require('mongoose');
const { Server } = require('socket.io');

const app = express();
const server = http.createServer(app);
const io = new Server(server, {
  cors: { origin: '*' }  // 실제 운영시 도메인 제한 필요
});

const mongoURI = 'mongodb+srv://Juwon:YJSqhdks@jwcluster0.kqx5tvg.mongodb.net/?retryWrites=true&w=majority&appName=JWCluster0';

mongoose.connect(mongoURI, {
  useNewUrlParser: true,
  useUnifiedTopology: true,
}).then(() => {
  console.log('MongoDB 연결 성공');
}).catch(err => {
  console.error('MongoDB 연결 실패:', err);
});

// 링크 데이터 스키마 정의
const linkSchema = new mongoose.Schema({
  user: String,       // 지갑 주소 또는 닉네임
  link: String,
  timestamp: { type: Date, default: Date.now },
});

const Link = mongoose.model('Link', linkSchema);

app.use(cors());
app.use(express.json());

// 모든 링크 조회
app.get('/api/messages', async (req, res) => {
  try {
    const links = await Link.find().sort({ timestamp: 1 });
    res.json(links);
  } catch (err) {
    res.status(500).json({ error: 'DB 조회 실패' });
  }
});

// 새 링크 저장 및 실시간 이벤트 전파
app.post('/api/messages', async (req, res) => {
  try {
    const { user, link } = req.body;
    if (!user || !link) return res.status(400).json({ error: 'user와 link가 필요합니다.' });

    const newLink = new Link({ user, link });
    await newLink.save();

    io.emit('new-link', newLink);

    res.status(201).json(newLink);
  } catch (err) {
    res.status(500).json({ error: '링크 저장 실패' });
  }
});

io.on('connection', (socket) => {
  console.log('새 클라이언트 접속:', socket.id);
  socket.on('disconnect', () => {
    console.log('클라이언트 연결 종료:', socket.id);
  });
});

const PORT = process.env.PORT || 3000;
server.listen(PORT, () => {
  console.log(`서버 실행 중 - http://localhost:${PORT}`);
});


