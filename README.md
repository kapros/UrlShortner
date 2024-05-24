Project for a URL shortener with React and .NET. Also using .NET Aspire for local development and testing of both backend and frontend (short example).

# Running the React FE
cd frontend
npm run dev

# Running the .NET backend
cd Backend
dotnet build ".\UrlShortner.sln" --configuration Debug
cd UrlShortner
dotnet run --launch-profile https
