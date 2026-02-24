set -e

cd music-rand-frontend

pnpm run build 

cd ..
docker compose up -d --build