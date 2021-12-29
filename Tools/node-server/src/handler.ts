import http = require('http');
import fs = require('fs');
import path = require('path');

import { mime } from './mime';

function getUnityFile(pathname: string, res: http.ServerResponse) {
    let fileurl = path.normalize(`./ServerData/${pathname}`);
    let fileext = path.extname(pathname || '.html');

    fs.readFile(fileurl, (error: Error | null, data: Buffer) => {
        if (error) {
            res.writeHead(404, {
                'Content-Type': 'text/plain; charset=UTF-8'
            });
            res.end('error 404');
        } else {
            let extmime = mime[fileext] || 'text/plain';
            res.writeHead(200, {
                'Content-Type': extmime
            });
            res.end(data);
        }
    });
}

export {
    getUnityFile
}
