import http = require('http');
import url = require('url');

import * as router from './router';
import * as handler from './handler';

const port = 80;

router.routers.push(new router.Router(router.ALL, handler.getUnityFile));

let server = http.createServer((req: http.IncomingMessage, res: http.ServerResponse) => {
    let obj = url.parse(req.url || './404.html');
    router.route(decodeURI(obj.pathname || '/'), res);
});

server.listen(port);
