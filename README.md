# Running the React FE
cd frontend
npm run dev

# Running the .NET backend
cd Backend
dotnet build ".\UrlShortner.sln" --configuration Debug
cd UrlShortner
dotnet run --launch-profile https
