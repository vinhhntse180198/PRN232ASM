FROM node:20-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build

FROM node:20-alpine AS final
WORKDIR /app
COPY --from=build /app/dist ./dist
RUN printf 'const http=require("http"),fs=require("fs"),path=require("path"),url=require("url");const root=path.join(__dirname,"dist");const types={".html":"text/html",".js":"application/javascript",".css":"text/css",".svg":"image/svg+xml",".json":"application/json",".png":"image/png",".ico":"image/x-icon",".woff2":"font/woff2"};http.createServer((req,res)=>{const u=url.parse(req.url);let p=path.join(root,u.pathname);if(!p.startsWith(root)){res.writeHead(403);return res.end();}if(fs.existsSync(p)&&fs.statSync(p).isDirectory()){p=path.join(p,"index.html");}fs.readFile(p,(err,data)=>{if(err){res.writeHead(404);return res.end("Not found");}const ext=path.extname(p).toLowerCase();const ct=types[ext]||"application/octet-stream";res.writeHead(200,{"Content-Type":ct,"Cache-Control":"no-store, no-cache, must-revalidate, max-age=0","Pragma":"no-cache","Expires":"0"});res.end(data);});}).listen(5173,()=>console.log("FE on 5173"));' > server.js
EXPOSE 5173
CMD ["node", "server.js"]
