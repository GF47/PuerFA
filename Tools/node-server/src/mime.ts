let mime: { [key: string]: string; } = {
    ".html": "text/html",
    ".css": "text/css",
    ".js": "application/x-javascript; charset=UTF-8",
    ".jsgz": "application/x-javascript; charset=UTF-8",

    ".png": "image/png",
    ".ico": "image/x-ico",

    ".json": "text/plain",
    ".config": "text/plain",
    ".csv": "text/plain",
    ".txt": "text/plain",

    ".*": "application/octet-stream"
}

export { mime };