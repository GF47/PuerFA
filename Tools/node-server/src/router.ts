import http = require('http');

const ALL: string = "*";

class Router {

    public pathname: string;
    public handler: (pathname: string, res: http.ServerResponse) => void;

    constructor(pathname: string, handler: (pathname: string, res: http.ServerResponse) => void) {
        this.pathname = pathname;

        let handler_ = (pathname: string, res: http.ServerResponse) => {
            res.writeHead(404, {
                'Content-Type': 'text/html; charset=UTF-8'
            });
            res.end('404, url cannot be reached <' + pathname + '>');
        };
        this.handler = handler || handler_;
    }
}

let routers: Router[] = [];

function route(pathname: string, res: http.ServerResponse) {

    console.log(pathname);

    for (let i = 0; i < routers.length; i++) {
        const r = routers[i];
        
        // if (pathname == r.pathname) {
        if (comparePath(pathname, r)) {
            r.handler(pathname, res);
            return;
        }
    }
}

function comparePath(apath: string, router: Router): boolean {
    return router.pathname === ALL || router.pathname === apath;
}

export { Router, routers, route, ALL };