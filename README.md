# Description

Project for a URL shortener with React and .NET. Also using .NET Aspire for local development and testing of both backend and frontend (short example).
Testing involves in memory hosting of the app in one test project and Aspire hosting in the other project, with Playwright for UI tests and a generated HttpClient for the API tests.

## Running the React FE

``` bash
cd frontend
npm run dev
```

## Running the .NET backend

``` bash
cd Backend
dotnet build ".\UrlShortner.sln" --configuration Debug
cd UrlShortner
dotnet run --launch-profile https
```

## Running Aspire

Set AppHost as the startup project and press F5. You will need Docker installed and turned on in order to run AppHost and tests that use the Aspire AppHost for testing.
