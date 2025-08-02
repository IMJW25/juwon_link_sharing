const express = require('express');
const http = require('http');
const Gun = require('gun');

const app = express();

app.use(require('cors')());
app.use(Gun.serve);

const server = http.createServer(app);
Gun({ web: server, file: 'data' });

server.listen(8765, () => {
  console.log('Gun relay server listening on http://localhost:8765');
});