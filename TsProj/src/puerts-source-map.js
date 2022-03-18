var csharp = require('csharp');
var puerts = require('puerts');

puerts.registerBuildinModule('path', {
    dirname(path) {
        return csharp.System.IO.Path.GetDirectoryName(path).replace(/\\/g, '/');
    },
    resolve(dir, url) {
        url = url.replace(/\\/g, '/');
        while (url.startsWith('../')) {
            dir = csharp.System.IO.Path.GetDirectoryName(dir);
            url = url.substr(3);
        }
        return csharp.System.IO.Path.Combine(dir, url).replace(/\\/g, '/');
    },
});

puerts.registerBuildinModule('fs', {
    existSync(path) {
        return csharp.System.IO.File.Exists(path);
    },
    readFileSync(path) {
        return csharp.System.IO.File.ReadAllText(path);
    },
});

(function () {
    let global = this ?? globalThis;
    global['Buffer'] = global['Buffer'] ?? require('buffer').Buffer;
})();

require('source-map-support').install();