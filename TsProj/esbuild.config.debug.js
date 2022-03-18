require('esbuild').buildSync({
    bundle: true,
    target: 'es2020',
    entryPoints: ['src/launcher.ts'],
    tsconfig: "./tsconfig.json",
    allowOverwrite: true,
    outfile: '../UnityProject/Assets/AddressablesRoot/Scripts/bundle.js.txt',
    external: ['csharp', 'puerts', 'path', 'fs'],
    minify: false,
    sourcemap: "inline",
    sourceRoot: process.cwd()
});
